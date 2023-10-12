using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Models;
using DrTrottoirApi.Validators;
using Geocoding.Google;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DrTrottoirDbContext _context;
        public CompanyRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddCompany(CreateCompanyRequest request)
        {
            var syndic = await _context.Syndics.FirstOrDefaultAsync(s => s.Id == request.SyndicId);

            if (syndic == null)
                throw new SyndicNotFoundException();

            if (!KboValidator.IsValid(request.IdKbo))
                throw new InvalidKboException();

            var (longitude, latitude) = await GetCoordinatesFromCompany(request.Address);

            var company = new Company
            {
                Address = request.Address,
                IdKbo = request.IdKbo,
                Name = request.Name,
                PictureUrl = request.PictureUrl,
                Syndic = syndic,
                Longitude = longitude,
                Latitude = latitude
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company.Id;
        }
        public async Task UpdateCompany(Guid id, CreateCompanyRequest request)
        {
            var companyExists = await _context.Companies.AnyAsync(r => r.Id == id);

            if (companyExists == false)
                throw new CompanyNotFoundException(request.Name);

            var syndic = await _context.Syndics.FirstOrDefaultAsync(s => s.Id == request.SyndicId);
            if (syndic == null)
                throw new SyndicNotFoundException();

            var (longitude, latitude) = await GetCoordinatesFromCompany(request.Address);

            var company = new Company
            {
                Id = id,
                Address = request.Address,
                IdKbo = request.IdKbo,
                Name = request.Name,
                PictureUrl = request.PictureUrl,
                Syndic = syndic,
                Longitude = longitude,
                Latitude = latitude
            };

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCompany(Guid id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                throw new CompanyNotFoundException();

            var companyGarbage = await _context.CompanyGarbageCollections.Where(x => x.CompanyId == id).ToListAsync();
            var companyTasks = await _context.Tasks.Where(x => x.Company.Id == id).ToListAsync();

            _context.Companies.Remove(company);
            _context.CompanyGarbageCollections.RemoveRange(companyGarbage);
            _context.Tasks.RemoveRange(companyTasks);

            await _context.SaveChangesAsync();
        }
        public async Task<IList<Company>> GetAllCompanies()
        {
            return await _context.Companies.ToListAsync();
        }
        public async Task<Company> GetCompanyById(Guid guid)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == guid);

            return company ?? throw new CompanyNotFoundException();
        }

        public async Task<(double, double)> GetCoordinates(Guid id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

            if(company == null)
                throw new CompanyNotFoundException();

            return (company.Longitude, company.Latitude);
        }
        private async Task<(double, double)> GetCoordinatesFromCompany(string address)
        {
            var geoCoder = new GoogleGeocoder() { ApiKey = Environment.GetEnvironmentVariable("MAPS_API_KEY") };

            var addresses = (await geoCoder.GeocodeAsync(address));

            if (!addresses.Any())
                throw new CoordinatesNotCalculatedException();

            var result = addresses.First();
            return (result.Coordinates.Longitude, result.Coordinates.Latitude);
        }
    }
}
