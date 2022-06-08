using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.IO;

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

            if (!File.Exists("LTdb_sqlite.db"))
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                int length = 8;
                while (0 < length--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }
                using var sha1 = new SHA1Managed();

                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(res.ToString()));

                string pass = string.Concat(hash.Select(b => b.ToString("x2")));
                Console.WriteLine("Login:Admin\nPassword:" + res.ToString());
                modelBuilder.Entity<UserDB>().HasData(new { id = 1, Login = "Admin", Password = pass, Role = "Admin" });
            }
            
            


        }
       

    }
}
