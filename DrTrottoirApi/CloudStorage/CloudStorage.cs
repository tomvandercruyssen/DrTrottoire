using DrTrottoirApi.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.Text;
using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Helpers;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.CloudStorage
{
    public class CloudStorage: ICloudStorage
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private DrTrottoirDbContext _context;
        
        public CloudStorage(DrTrottoirDbContext context)
        {
            _context = context;
            _storageClient = StorageClient.Create(GoogleCredential.GetApplicationDefault());
            _bucketName = Environment.GetEnvironmentVariable("PICTURE_BUCKET") ?? "test_picture_bucket";        // PRODUCTION ?? PIPELINE
        }

        public async Task UploadTaskImage(UploadTaskImageRequest request)
        {
            var fileName = request.PictureLabel + DateTimeHelper.GetBelgianTime().ToString("yyyyMMdd") + "/" + Path.GetRandomFileName();
            using var memoryStream = new MemoryStream();
            await request.Image.CopyToAsync(memoryStream);

            var company = _context.Companies.FirstOrDefault(x => x.Id == request.CompanyId) ??
                              throw new CompanyNotFoundException();

            await _storageClient.UploadObjectAsync(_bucketName, $"{company.Name}/{fileName}.jpeg", "Image/jpeg", memoryStream);

            var task = _context.Tasks.FirstOrDefault(x => x.Company.Id == company.Id && x.StartTime.Date == DateTime.UtcNow.Date);

            if (task == null)
                throw new TaskNotFoundException();

            var picture = new Picture()
            {
                PictureLabel = request.PictureLabel,
                PictureUrl = "https://storage.googleapis.com/" + _bucketName + $"/{company.Name}/{fileName}.jpeg",
                Task = task
            };

            await _context.Pictures.AddAsync(picture);
            await _context.SaveChangesAsync();
        }
        public async Task UploadCompanyImage(UploadCompanyImageRequest request)
        {
            using var memoryStream = new MemoryStream();
            await request.Image.CopyToAsync(memoryStream);

            var company = _context.Companies.FirstOrDefault(x => x.Id == request.CompanyId) ??
                          throw new CompanyNotFoundException("The company was not found");

            if (company.PictureUrl != null)
                await DeleteCompanyImage(company.Id);

            await _storageClient.UploadObjectAsync(_bucketName, $"{company.Name}/appearance.jpeg", "Image/jpeg", memoryStream);

            company.PictureUrl = "https://storage.googleapis.com/" + _bucketName + $"/{company.Name}/appearance.jpeg";

            await _context.SaveChangesAsync();
        }
        public async Task DeleteCompanyImage(Guid companyId)
        {
            var company = _context.Companies.FirstOrDefault(x => x.Id == companyId) ??
                          throw new CompanyNotFoundException();

            await _storageClient.DeleteObjectAsync(_bucketName, $"{company.Name}/appearance.jpeg");

            company.PictureUrl = null;

            await _context.SaveChangesAsync();
        }

        public async Task UploadManual(UploadCompanyManualRequest request)
        {
            var company = _context.Companies.FirstOrDefault(x => x.Id == request.CompanyId) ??
                          throw new CompanyNotFoundException();

            using var memoryStream = new MemoryStream();
            await request.Manual.CopyToAsync(memoryStream);

            var uploadedImage =
                await _storageClient.UploadObjectAsync(_bucketName, $"{company.Name}/Manual", "application/pdf", memoryStream);

            company.ManualUrl = "https://storage.googleapis.com/" + _bucketName + $"/{company.Name}/Manual";

            await _context.SaveChangesAsync();
        }
    }
}
