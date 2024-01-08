using Blog.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Domain.Services.User
{
    public interface IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemeName"></param>
        /// <param name="claims"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        Task<IUserProfile> AuthenticateAsync(string schemeName,
            IEnumerable<Claim> claims, IDictionary<string, string> metadata);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IUserProfile?> GetUserByIdAsync(int userId);
    }
}
