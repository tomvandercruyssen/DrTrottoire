using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public class SyndicRepository : ISyndicRepository
    {
        private readonly DrTrottoirDbContext _context;

        public SyndicRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateSyndic(CreateSyndicRequest request)
        {
            var syndic = new Syndic()
            {
                FirstName = request.FirstName, LastName = request.LastName, TelephoneNumber = request.TelephoneNumber
            };

            _context.Syndics.Add(syndic);
            await _context.SaveChangesAsync();
            return syndic.Id;
        }

        public async Task<IList<Syndic>> GetAllSyndics()
        {
            return await _context.Syndics.ToListAsync();
        }

        public async Task<Syndic> GetSyndicById(Guid id)
        {
            var syndic = await _context.Syndics.FirstOrDefaultAsync(s => s.Id == id);

            return syndic ?? throw new SyndicNotFoundException();
        }

        public async Task UpdateSyndic(Guid id, CreateSyndicRequest request)
        {
            var syndicExists = await _context.Syndics.AnyAsync(r => r.Id == id);

            if (syndicExists == false)
                throw new SyndicNotFoundException(request.FirstName);

            var syndic = new Syndic()
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                TelephoneNumber = request.TelephoneNumber
            };

            _context.Syndics.Update(syndic);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSyndic(Guid id)
        {
            var syndic = await _context.Syndics.FirstOrDefaultAsync(x => x.Id == id);

            if (syndic == null)
                throw new SyndicNotFoundException();

            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Syndic.Id == id);

            if (company != null)
            {
                company.Syndic = null;
                await _context.SaveChangesAsync();
            }
           
            _context.Syndics.Remove(syndic);
            
            await _context.SaveChangesAsync();
        }
    }
}
