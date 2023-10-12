using DrTrottoirApi.Models;

namespace DrTrottoirApi.CloudStorage
{
    public interface ICloudStorage
    {
        Task UploadTaskImage(UploadTaskImageRequest request);
        Task UploadCompanyImage(UploadCompanyImageRequest request);
        Task DeleteCompanyImage(Guid companyId);
        Task UploadManual(UploadCompanyManualRequest request);
    }
}
