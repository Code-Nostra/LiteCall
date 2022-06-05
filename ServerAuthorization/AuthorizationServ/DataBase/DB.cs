using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServ
{
    public class DB : DbContext
    {
        public DbSet<UserDB> Users { get; set; }
        public DbSet<ServerDB> Servers { get; set; }
        public DB()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=LTdb_sqlite.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDB>().HasData(new UserDB { id=1,Login="admin",Password="asd",Role="admin"});
            modelBuilder.Entity<ServerDB>().HasData(new ServerDB { Id=1,Title= "LiteCall", Description= "Community LiteCall Server" });
        }


    }
}
