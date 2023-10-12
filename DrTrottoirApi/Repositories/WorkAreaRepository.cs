using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public class WorkAreaRepository : IWorkAreaRepository
    {
        private readonly DrTrottoirDbContext _context;

        public WorkAreaRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }

        public async Task<IList<WorkArea>> GetAllWorkAreas()
        {
            return await _context.WorkAreas.ToListAsync();
        }
        public async Task<WorkArea> GetWorkAreasById(Guid id)
        {
            var workArea = await _context.WorkAreas.FirstOrDefaultAsync(w => w.Id == id);

            return workArea ?? throw new WorkAreaNotFoundException();
        }
        public async Task<Guid> CreateWorkArea(CreateWorkAreaRequest request)
        {
            var workArea = new WorkArea() { City = request.City };

            _context.WorkAreas.Add(workArea);
            await _context.SaveChangesAsync();

            return workArea.Id;
        }
        public async Task DeleteWorkArea(Guid id)
        {
            var workArea = await _context.WorkAreas.FirstOrDefaultAsync(w => w.Id == id);

            if (workArea == null)
                throw new WorkAreaNotFoundException();

            _context.WorkAreas.Remove(workArea);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateWorkArea(Guid id, CreateWorkAreaRequest request)
        {
            var syndicExists = await _context.WorkAreas.AnyAsync(r => r.Id == id);

            if (syndicExists == false)
                throw new WorkAreaNotFoundException(request.City);

            var workArea = new WorkArea() { City = request.City };
            
            _context.WorkAreas.Update(workArea);

            await _context.SaveChangesAsync();
        }
        public async Task<IList<GeneralUserInfoResponse>> GetUsersByWorkArea(Guid id)
        {
            return await _context.Users.Where(u => u.WorkAreaId == id).Select(x => new GeneralUserInfoResponse()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Id = x.Id,
                PictureUrl = x.PictureUrl,
                TelephoneNumber = x.TelephoneNumber
            }).ToListAsync();
        }
    }
}
