using Blog.Domain.Models.User;
using Blog.Repositary;
using Blog.Repositary.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Services.User
{
    public class UserService : IUserService
    {
        private record UserInfo
        {
            public string? Sub { get; set; }
            public string FirstName { get; set; } = "Anonymous";
            public string? LastName { get; set; }
            public string? Email { get; set; }
        }

        private readonly ILogger<IUserService> _logger;
        private readonly AppDbContext _dbContext;

        private readonly string _googleSchemeName;
        private readonly string _vas3kSchemeName;

        public UserService(AppDbContext dbContext, IConfiguration configuration,
            ILogger<IUserService> logger)
        {
            _dbContext = dbContext;

            _googleSchemeName = configuration["GoogleAuth:Scheme"] ?? "google";
            _vas3kSchemeName = configuration["Vas3kAuth:Scheme"] ?? "vas3k";

            _logger = logger;
        }

        // https://www.milanjovanovic.tech/blog/working-with-transactions-in-ef-core
        private async Task<IUserProfile> GetOrCreateUserAsync(string email, string firstName, string? lastName)
        {
            try
            {
                _logger.LogInformation("UserService::GetOrCreateUserAsync: Get or Create user with email: {}",
                    email);
                
                var dbUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
                _logger.LogInformation("UserService::GetOrCreateUserAsync: User with {} found result: {}",
                    email, (dbUser == null));

                if (dbUser == null) // Create user
                {
                    dbUser = new DbUser
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                    };

                    await _dbContext.Users.AddAsync(dbUser);
                    await _dbContext.SaveChangesAsync();

                    return new UserProfile(dbUser);
                }
                else
                {
                    _logger.LogInformation("UserService::GetOrCreateUserAsync: User exists: {} {} {} {}",
                        dbUser.Id, dbUser.Email, dbUser.FirstName, dbUser.LastName);
                    return new UserProfile(dbUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserService::GetOrCreateUserAsync: Can't get or create user. Msg: {}", ex.Message);
                throw;
            }
        }

        private static UserInfo GetGoogleUserInfo(IEnumerable<Claim> claims) => new()
        {
            Sub = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                .Value,
            FirstName = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?
                .Value ?? "GoogleUser",
            LastName = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Surname)?
                .Value,
            Email = claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?
                .Value,
        };

        private static UserInfo GetVas3kUserInfo(IEnumerable<Claim> claims)
        {
            var sub = claims
                .FirstOrDefault(c => c.Type == "sub")?
                .Value;
            var name = claims
                .FirstOrDefault(c => c.Type == "name")?
                .Value;
            var email = claims
                .FirstOrDefault(c => c.Type == "email")?
                .Value;

            if (name != null)
            {
                var splitName = name.Split(' ');
                var firstName = splitName[0];
                var lastName = string.Empty;

                if (splitName.Length > 1)
                {
                    lastName = splitName[1].Trim();
                }

                return new()
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Sub = sub,
                };
            }

            return new()
            {
                Email = email,
                FirstName = sub ?? "Vas3kUser",
                LastName = string.Empty,
                Sub = sub,
            };
        }

        private UserInfo GetUserInfo(string schemeName, IEnumerable<Claim> claims)
        {
            if (schemeName == _googleSchemeName)
            {
                return GetGoogleUserInfo(claims);
            }
            else if (schemeName == _vas3kSchemeName)
            {
                return GetVas3kUserInfo(claims);
            }
            else
            {
                _logger.LogCritical("UserService::GetUserInfo: Unknown scheme name \"{}\"", schemeName);
                throw new AuthenticationException($"Unknown scheme name \"{schemeName}\"");
            }
        }

        public async Task<IUserProfile> AuthenticateAsync(string schemeName,
            IEnumerable<Claim> claims, IDictionary<string, string> metadata)
        {
            _logger.LogInformation("UserService::AuthenticateAsync: Start authenticating user with scheme \"{}\"",
                schemeName);

            var userInfo = GetUserInfo(schemeName, claims);
            _logger.LogInformation("UserService::AuthenticateAsync: UserInfo: {} {} {}",
                userInfo.Email, userInfo.FirstName, userInfo.LastName);

            if (userInfo.Email == null)
            {
                _logger.LogError("UserService::AuthenticateAsync: GoogleAuthScheme: user email claim is null");
                throw new AuthenticationException("GoogleAuthScheme: user email claim is null");
            }

            var user = await GetOrCreateUserAsync(userInfo.Email, userInfo.FirstName, userInfo.LastName);
            return user;
        }

        public async Task<IUserProfile?> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("UserService::GetUserById: Get user by Id={}", userId);

            try
            {
                var dbUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (dbUser == null)
                {
                    _logger.LogWarning("UserService::GetUserById: User with Id={} is not found.", userId);
                    return null;
                }
                else
                {
                    _logger.LogInformation("UserService::GetUserById: Found user {} {} {}",
                        dbUser.Email, dbUser.FirstName, dbUser.LastName);
                    return new UserProfile(dbUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("UserService::GetUserById: Exception raised. Msg: {}", ex.Message);
                throw;
            }
        }
    }
}
