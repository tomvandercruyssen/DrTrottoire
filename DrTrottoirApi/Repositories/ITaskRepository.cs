using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public interface ITaskRepository
    {
        Task<IList<BaseTaskResponse>> GetAllTasks();
        Task<BaseTaskResponse> GetTaskById(Guid id);
        Task<Guid> AddTask(CreateTaskRequest task);
        Task DeleteTask(Guid id);
        Task UpdateTask(Guid id, CreateTaskRequest task);
        Task CompleteTask(Guid id);
    }
}
