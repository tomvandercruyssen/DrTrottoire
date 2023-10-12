using DrTrottoirApi.Entities;
using DrTrottoirApi.Exceptions;
using DrTrottoirApi.Helpers;
using DrTrottoirApi.Models;
using DrTrottoirApi.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Task = System.Threading.Tasks.Task;

namespace DrTrottoirApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        private readonly DrTrottoirDbContext _context;

        public UserRepository(SignInManager<User> signInManager, DrTrottoirDbContext context,
            IOptions<AppSettings> options)
        {
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _appSettings = options.Value;
            _context = context;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new AuthenticationFailedException(request.Email);

            var signInResult =
                await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
                throw new AuthenticationFailedException(request.Email);

            string jwtToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = GenerateRefreshToken(ipAddress);

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            return new AuthenticateResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }
        public async Task DeactivateToken(string token, string ipAddress)
        {
            User user = await _userManager.Users.Include(y => y.RefreshTokens)
                .FirstOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new UserNotFoundException();

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new TokenExpiredException();

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _userManager.UpdateAsync(user);
        }
        public async Task<AuthenticateResponse> RenewToken(string token, string ipAddress)
        {
            var user = await _userManager.Users.Include(y => y.RefreshTokens)
                .FirstOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new UserNotFoundException();

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new TokenExpiredException();

            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            var jwtToken = await GenerateJwtToken(user);
            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            return new AuthenticateResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken.Token,
                Roles = roles,
            };
        }
        public async Task<IList<BaseUserResponse>> GetAllUsers()
        {
            var usersResponse = new List<BaseUserResponse>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                usersResponse.Add(new BaseUserResponse()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PictureUrl = user.PictureUrl,
                    TelephoneNumber = user.TelephoneNumber,
                    Roles = await _userManager.GetRolesAsync(user),
                    WorkArea = user.WorkArea
                });
            }

            return usersResponse;
        }
        public async Task<BaseUserResponse> GetUserById(Guid guid)
        {
            var user = await _userManager.Users.FirstAsync(x => x.Id == guid);

            if (user == null)
                throw new UserNotFoundException();

            var roles = await _userManager.GetRolesAsync(user);

            return new BaseUserResponse()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PictureUrl = user.PictureUrl,
                TelephoneNumber = user.TelephoneNumber,
                Roles = roles,
                WorkArea = user.WorkArea
            };
        }
        public async Task DeleteUserById(Guid id)
        {
            // First find and delete all FK constraints
            var userRounds = _context.Rounds.Where(x => x.UserId == id);
            var userRefreshTokens = _context.RefreshTokens.Where(x => x.UserId == id);
            var userRoles = _context.UserRoles.Where(x => x.UserId == id);

            _context.Rounds.RemoveRange(userRounds);
            _context.RefreshTokens.RemoveRange(userRefreshTokens);
            _context.UserRoles.RemoveRange(userRoles);

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null) throw new UserNotFoundException();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) throw new ContextActionFailedException("Delete");
        }
        public async Task<IList<TimeRecordsByStudentResponse>> GetTimeRecordsOfStudent(Guid id)
        {
            var rounds = await _context.Rounds.ToListAsync();
            var roundsOfUser = rounds.Where(r => r.UserId == id).ToList();
            var allRounds = rounds.DistinctBy(r => r.Name).ToList();
            var response = new List<TimeRecordsByStudentResponse>();

            foreach (var round in allRounds)
            {
                var roundDurations = roundsOfUser.Where(r => r.Id == round.Id).Select(r=> r.RoundDuration).ToList();
                var averageRoundDuration = roundDurations.Any() ? TimeSpan.FromMinutes(roundDurations.Average(ts => ts.TotalMinutes)) : TimeSpan.Zero; // Student hasn't finished this round

                response.Add(new TimeRecordsByStudentResponse()
                    { Duration = averageRoundDuration, RoundId = round.Id, RoundName = round.Name });
            }

            return response;
        }
        public async Task<Guid> CreateUser(CreateUserRequest request)
        {
            var user = new User()
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                WorkAreaId = request.WorkAreaId,
                TelephoneNumber = request.TelephoneNumber,
                Email = request.Email,
                PictureUrl = request.PictureUrl,
                
            };
            
            if (RoleValidator.CorrectRoles(request.Roles))
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                    throw new ContextActionFailedException("Create user");

                foreach (var role in request.Roles)
                    await _userManager.AddToRoleAsync(user, role);
            }
            else
                throw new InvalidRolesException();
            return user.Id;
        }
        public async Task UpdateUser(Guid id, BaseUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                throw new UserNotFoundException(request.FirstName);

            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.WorkAreaId = request.WorkAreaId;
            user.TelephoneNumber = request.TelephoneNumber;
            user.PictureUrl = request.PictureUrl;

            await _userManager.UpdateAsync(user);
        }
        public async Task<IList<BaseRoundResponse>> GetRoundsByUserId(Guid guid)
        {
            var monday = GetDateOfMonday();
            var saturday = monday.AddDays(6);
            var response = await _context.Rounds.Where(x => x.UserId == guid && x.StartTime > monday.ToUniversalTime() && x.EndTime < saturday.ToUniversalTime())
                .Select(round => new BaseRoundResponse()
                {
                    Id = round.Id,
                    StartTime = round.StartTime,
                    EndTime = round.EndTime,
                    Status = round.Status,
                    Name = round.Name,
                    UserId = round.UserId
                }).ToListAsync();
            return response ?? throw new RoundNotFoundException();
        }
        public async Task<IList<GetUserByRoleResponse>> GetUsersByRole(Roles role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.ToString());

            return users.Select(u => new GetUserByRoleResponse()
                { UserId = u.Id, FirstName = u.FirstName, LastName = u.LastName }).ToList();
        }

        private DateTime GetDateOfMonday()
        {
            var daysSinceMonday = (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday;
            return DateTime.Today.AddDays(-daysSinceMonday);
        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var roleNames = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("UserName", user.UserName)
            };

            foreach (string roleName in roleNames)
                claims.Add(new Claim(ClaimTypes.Role, roleName));

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = "DrTrottoir Web API",
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(1),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}
