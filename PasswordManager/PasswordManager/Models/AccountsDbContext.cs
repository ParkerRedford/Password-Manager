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
            ob.UseSqlServer("Server=.;Database=db;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
