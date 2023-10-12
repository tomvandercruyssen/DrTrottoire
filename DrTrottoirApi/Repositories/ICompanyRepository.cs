using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public interface ICompanyRepository
    {
        Task<IList<Company>> GetAllCompanies();
        Task<Company> GetCompanyById(Guid id);
        Task<Guid> AddCompany(CreateCompanyRequest company);
        Task DeleteCompany(Guid id);
        Task UpdateCompany(Guid id, CreateCompanyRequest company);
        Task<(double, double)> GetCoordinates(Guid id);
    }
}
