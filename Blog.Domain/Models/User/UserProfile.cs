using Blog.Repositary.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Models.User
{
    internal class UserProfile : IUserProfile
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public bool IsAdministrator { get; init; } = false;

        public UserProfile(DbUser dbUser)
        {
            Id = dbUser.Id;
            Email = dbUser.Email!;
            FirstName = dbUser.FirstName;
            LastName = dbUser.LastName ?? string.Empty;

            IsAdministrator = dbUser.IsAdmin;
        }
    }
}
