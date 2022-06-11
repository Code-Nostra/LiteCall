using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using AuthorizationServ.DataBase;

namespace AuthorizationServ
{
    public class DB : DbContext
    {
        public DbSet<UserDB> Users { get; set; }
        public DbSet<ServerDB> Servers { get; set; }

        public DbSet<SecurityQuestions> SecurityQuestions { get; set; }
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
                modelBuilder.Entity<UserDB>().HasData(new UserDB{ id = 1, Login = "Admin", Password = pass, Role = "Admin" });
                modelBuilder.Entity<ServerDB>().HasData(new ServerDB { id = 1, Title= "LiteCall" });
                modelBuilder.Entity<SecurityQuestions>().HasData( new SecurityQuestions { id = 1,Questions= "Какое прозвище было у вас в детстве?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 2, Questions = "Как звали вашего лучшего друга детства?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 3, Questions = "На какой улице вы жили в третьем классе?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 4, Questions = "Какую школу вы посещали в шестом классе?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 5, Questions = "Какое имя и фамилия у вашего старшего двоюродного брата/сестры?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 6, Questions = "Как звали вашу первую плюшевую игрушку?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 7, Questions = "В каком месте встретились ваши родители?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 8, Questions = "Как звали вашего учителя в третьем классе?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 9, Questions = "В каком городе живет ваш ближайший родственник?" });
            }
        }
    }
}
