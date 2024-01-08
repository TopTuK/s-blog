using Blog.Repositary.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Blog.Repositary
{
    // https://stackoverflow.com/questions/38705694/add-migration-with-different-assembly
    /*
     * https://jasonwatmore.com/post/2022/09/05/net-6-connect-to-sqlite-database-with-entity-framework-core
     * dotnet ef migrations add InitialCreate
     * dotnet ef database update
    */
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext> _logger;
        private readonly IConfiguration _configuration;

        public DbSet<DbUser> Users { get; set; }

        public AppDbContext(IConfiguration configuration, ILogger<AppDbContext> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger.LogInformation("AppDbContext::OnConfiguring: Start configuration database context");

            optionsBuilder.UseSqlite(_configuration.GetConnectionString("BlogSqlLiteDb"));
            base.OnConfiguring(optionsBuilder);

            _logger.LogInformation("AppDbContext::OnConfiguring: End configuration database context");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
