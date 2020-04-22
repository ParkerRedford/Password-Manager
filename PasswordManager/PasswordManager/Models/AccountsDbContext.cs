using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using PasswordGeneratorCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordGeneratorCore
{
    public class AccountsDbContext : DbContext
    {
        public DbSet<AccountModel> Passwords { get; set; }

        public AccountsDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            base.OnConfiguring(ob);
            ob.UseSqlServer("Data Source=.;Initial Catalog=db;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
