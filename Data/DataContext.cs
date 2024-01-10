//namespace dotnet_core_auth_api.Data

using dotnet_core_auth_api.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace dotnet_core_auth_api.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
    }
}
