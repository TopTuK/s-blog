using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Models.User
{
    public interface IUserProfile
    {
        int Id { get; }
        string FirstName { get; }
        string LastName { get; }
        [EmailAddress]
        string Email { get; }
        bool IsAdministrator { get; }
    }
}
