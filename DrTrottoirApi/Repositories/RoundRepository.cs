using DrTrottoirApi.Entities;
using DrTrottoirApi.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Policy;
using DrTrottoirApi.Exceptions;
using Task = System.Threading.Tasks.Task;
using DrTrottoirApi.Models;
using TaskStatus = DrTrottoirApi.Entities.TaskStatus;

namespace DrTrottoirApi.Repositories
{
    public class RoundRepository : IRoundRepository
    {
        private readonly DrTrottoirDbContext _context;

        public RoundRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddRound(CreateRoundRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
                throw new UserNotFoundException();

            var round = new Round()
            {
                StartTime = request.StartTime, EndTime = request.EndTime, Name = request.Name, User = user
            };
            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();
            return round.Id;
        }
        public async Task UpdateRound(Guid id, Round round)
        {
            var roundExists = await _context.Rounds.AnyAsync(r => r.Id == id);

            if (!roundExists)
                throw new RoundNotFoundException(round.Name);

            _ = _context.Rounds.Update(round);
            _ = _context.SaveChangesAsync();
        }
        public async Task DeleteRound(Guid id)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == id);

            if(round == null)
                throw new RoundNotFoundException();

            var roundTasks = await _context.Tasks.Where(x => x.RoundId == id).ToListAsync();

            _context.Tasks.RemoveRange(roundTasks);
            _context.Rounds.Remove(round);

            await _context.SaveChangesAsync();
        }
        public async Task<IList<BaseRoundResponse>> GetAllRounds()
        {
            var result = await _context.Rounds.Select(round => new BaseRoundResponse()
            {
                Id = round.Id,
                StartTime = round.StartTime,
                EndTime = round.EndTime,
                Status = round.Status,
                Name = round.Name,
                UserId = round.UserId
            }).ToListAsync();
            return result;
        }
        public async Task<BaseRoundResponse> GetRoundById(Guid guid)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(round => round.Id == guid) ?? throw new RoundNotFoundException();

            return new BaseRoundResponse
            {
                Id = round.Id,
                StartTime = round.StartTime,
                EndTime = round.EndTime,
                Status = round.Status,
                Name = round.Name,
                UserId = round.UserId
            };
        }
        public async Task<IList<Company>> GetCompaniesByRound(Guid roundId)
        {
            return await _context.Tasks.Where(x => x.RoundId == roundId).Select(c => c.Company).ToListAsync();
        }
        public async Task<int> GetProgressOfRound(Guid id)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == id);

            if(round == null)
                throw new RoundNotFoundException();

            var tasks = await _context.Tasks.Where(t => t.RoundId == id).ToListAsync();
            var numberOfTasks = tasks.Count;
            var numberOfTasksDone = 0;

            foreach (var task in tasks)
            {
                if(task.Status == TaskStatus.Finished)
                    numberOfTasksDone++;
            }

            return (int)(numberOfTasksDone / (double)numberOfTasks * 100);
        }
        public async Task<IList<GetTasksForRoundsResponse>> GetTasksForRound(Guid id)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(x => x.Id == id);

            if (round == null)
                throw new RoundNotFoundException();

            var tasksForRounds = await _context.Tasks.Where(t => t.RoundId == id).ToListAsync();
            var response = new List<GetTasksForRoundsResponse>();

            foreach (var task in tasksForRounds)
            {
                // Normally there should only be max 2 gc one for inside and for outside
                var collectionsForCompany = await _context.CompanyGarbageCollections
                    .Where(cg => cg.CompanyId == task.Company.Id && cg.GarbageCollection.CollectionTime.Date == round.StartTime.Date)
                    .Select(x => x.GarbageCollectionId)
                    .ToListAsync();

                var garbageTypesInside = _context.GarbageCollectionGarbageTypes
                    .Where(g => collectionsForCompany.Contains(g.GarbageCollectionId) && g.GarbageCollection.HasToBeBroughtOutside)
                    .Select(g => g.GarbageType)
                    .ToList();

                var garbageTypesOutside = _context.GarbageCollectionGarbageTypes
                    .Where(g => collectionsForCompany.Contains(g.GarbageCollectionId) && !g.GarbageCollection.HasToBeBroughtOutside)
                    .Select(g => g.GarbageType)
                    .ToList();

                response.Add(new GetTasksForRoundsResponse()
                {
                    CompanyId = task.Company.Id,
                    CompanyName = task.Company.Name,
                    Status = task.Status,
                    GarbageTypesInside = garbageTypesInside,
                    GarbageTypesOutside = garbageTypesOutside,
                });
            }

            return response;
        }
        public async Task StartRound(Guid id)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == id);

            if (round == null)
                throw new RoundNotFoundException();

            round.Status = RoundStatus.InProgress;
            await SetFirstTaskInProgress(id);
            await _context.SaveChangesAsync();
        }
        public async Task<IList<GetRemarksOfRoundResponse>> GetRemarksOfRound(Guid id)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == id);

             if (round == null)
                 throw new RoundNotFoundException();
             
             return await _context.Tasks
                 .Where(t => t.RoundId == id)
                 .Select(t => new GetRemarksOfRoundResponse() { CompanyName = t.Company.Name, Remark = t.Remark })
                 .ToListAsync();
        }
        private async Task SetFirstTaskInProgress(Guid id)
        {
            var firstTask = await _context.Tasks.Where(t => t.RoundId == id).OrderBy(t => t.OrderNumber).FirstAsync();
            firstTask.StartTime = DateTime.UtcNow;
            firstTask.Status = TaskStatus.InProgress;
            await _context.SaveChangesAsync();
        }
    }
}
