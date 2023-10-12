using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Helpers;
using DrTrottoirApi.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using TaskStatus = DrTrottoirApi.Entities.TaskStatus;

namespace DrTrottoirApi.Repositories
{
    public class TaskRepository: ITaskRepository
    {
        private readonly DrTrottoirDbContext _context;
        public TaskRepository(DrTrottoirDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> AddTask(CreateTaskRequest request)
        {
            var roundExists = await _context.Rounds.AnyAsync(r => r.Id == request.RoundId);
            if (!roundExists) throw new RoundNotFoundException();

            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId);
            if (company.Equals(null)) throw new CompanyNotFoundException();

            var task =  new Entities.Task
            {
                Status = TaskStatus.NotStarted,
                RoundId = request.RoundId,
                Company = company
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task.Id;
        }
        public async Task UpdateTask(Guid id, CreateTaskRequest request)
        {
            var taskExists = await _context.Tasks.AnyAsync(r => r.Id == id);
            var roundExists = await _context.Rounds.AnyAsync(r => r.Id == request.RoundId);
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == request.CompanyId);
            if (!taskExists) throw new TaskNotFoundException();
            if (!roundExists) throw new RoundNotFoundException();
            if(company == null) throw new CompanyNotFoundException();

            var task = new Entities.Task
            {
                Id = id,
                RoundId = request.RoundId,
                Company = company
            };

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if(task == null)
                throw new TaskNotFoundException();

            var taskPictures = await _context.Pictures.Where(x => x.TaskId == id).ToListAsync();

            _context.Tasks.Remove(task);
            _context.Pictures.RemoveRange(taskPictures);

           await _context.SaveChangesAsync();
        }
        public async Task CompleteTask(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            
            if (task == null)
                throw new TaskNotFoundException();

            task.Status = TaskStatus.Finished;
            task.EndTime = DateTime.UtcNow;
            task.TaskDuration = task.EndTime - task.StartTime;
            // Check all tasks are done -> if so then complete round.
            var allCompleted = await CheckAllTasksCompleted(task.RoundId);

            if (!allCompleted)
                await SetNextTaskInProgress(task.RoundId);

            await _context.SaveChangesAsync();
        }
        private async Task SetNextTaskInProgress(Guid roundId)
        {
            var sortedTasks = await _context.Tasks.Where(t => t.RoundId == roundId).OrderBy(t => t.OrderNumber)
                .ToListAsync();

            var nextTask = sortedTasks.FirstOrDefault(t => t.Status == TaskStatus.NotStarted);

            if (nextTask != null)
            {
                nextTask.StartTime = DateTime.UtcNow;
                nextTask.Status = TaskStatus.InProgress;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<IList<BaseTaskResponse>> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            var taskResponses = new List<BaseTaskResponse>();

            foreach (var task in tasks)
            {
                var taskPictures = await _context.Pictures.Where(p => p.TaskId == task.Id).ToListAsync();
                
                var taskResponse = new BaseTaskResponse
                {
                    Id = task.Id,
                    StartTime = task.StartTime,
                    EndTime = task.EndTime,
                    Remark = task.Remark,
                    Status = task.Status,
                    BasePictureResponses = taskPictures.Select(bp => new BasePictureResponse
                    {
                        PictureUrl = bp.PictureUrl,
                        PictureLabel = (int)bp.PictureLabel
                    }).ToList(),
                    RoundId = task.RoundId,
                    CompanyId = task.Company.Id
                };

                taskResponses.Add(taskResponse);
            }

            return taskResponses;
        }
        public async Task<BaseTaskResponse> GetTaskById(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            var taskPictures = await _context.Pictures.Where(p => p.TaskId == task.Id).ToListAsync();
            if (task == null)
                throw new TaskNotFoundException();

            return new BaseTaskResponse
            {
                Id = task.Id,
                StartTime = task.StartTime,
                EndTime = task.EndTime,
                Remark = task.Remark,
                Status = task.Status,
                BasePictureResponses = taskPictures.Select(bp => new BasePictureResponse
                {
                    PictureUrl = bp.PictureUrl,
                    PictureLabel = (int)bp.PictureLabel
                }).ToList(),
                RoundId = task.RoundId,
                CompanyId = task.Company.Id            
            };
        }
        private async Task<bool> CheckAllTasksCompleted(Guid roundId)
        {
            var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == roundId);

            if (round == null)
                throw new RoundNotFoundException();

            var allTasksOfRound = await _context.Tasks.Where(t => t.RoundId == roundId).ToListAsync();

            if (allTasksOfRound.Any(task => task.Status != TaskStatus.Finished))
                return false;

            round.Status = RoundStatus.Finished;
            round.EndTime = DateTime.UtcNow;
            round.RoundDuration = round.EndTime - round.StartTime;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
