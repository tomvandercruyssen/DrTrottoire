using DrTrottoirApi.Entities;
using DrTrottoirApi.Models;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public interface IUserRepository
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, string ipAddress);
        Task DeactivateToken(string token, string ipAddress);
        Task<AuthenticateResponse> RenewToken(string token, string ipAddress);

        Task<IList<BaseUserResponse>> GetAllUsers();
        Task<BaseUserResponse> GetUserById(Guid guid);
        Task DeleteUserById(Guid id);
        Task<Guid> CreateUser(CreateUserRequest request);
        Task UpdateUser(Guid id, BaseUserRequest request);
        Task<IList<TimeRecordsByStudentResponse>> GetTimeRecordsOfStudent(Guid id);
        Task<IList<GetUserByRoleResponse>> GetUsersByRole(Roles role);
        Task<IList<BaseRoundResponse>> GetRoundsByUserId(Guid guid);
    }
}
