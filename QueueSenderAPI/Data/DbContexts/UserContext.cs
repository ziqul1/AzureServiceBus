using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QueueSenderAPI.Models;

namespace QueueSenderAPI.Data.DbContexts
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext(DbContextOptions options) : base(options)
        {

        }
    }
}
