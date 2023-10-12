using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public class GarbageTypeRepository : IGarbageTypeRepository
    {
        private readonly DrTrottoirDbContext _context;

        public GarbageTypeRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }

        public async Task CreateGarbageType(CreateGarbageTypeRequest request)
        {
            if(request.Name == null)
                throw new ArgumentNullException(nameof(request.Name));

            var typeAlreadyExists = await _context.GarbageTypes.AnyAsync(g => g.Name == request.Name);

            if (typeAlreadyExists)
                throw new AlreadyExistsException("GarbageType");

            var garbageType = new GarbageType() { Name = request.Name };

            _context.GarbageTypes.Add(garbageType);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GarbageTypeResponse>> GetAllGarbageTypes()
        {
            return await _context.GarbageTypes.Select(g => new GarbageTypeResponse() { Id = g.Id, Name = g.Name }).ToListAsync();
        }
    }
}
