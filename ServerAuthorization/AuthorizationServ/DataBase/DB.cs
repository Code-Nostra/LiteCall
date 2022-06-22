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
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http;
using System.Net.Mime;

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
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "LTdb_sqlite.db")}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "LTdb_sqlite.db")))
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                Random rnd = new Random();
                int length = 8;
                while (0 < length--)
                {
                    res.Append(valid[rnd.Next(valid.Length)]);
                }
                Console.WriteLine("Login:Admin\nPassword:" + res.ToString());

                StringBuilder builder = new StringBuilder();
                Enumerable
                   .Range(65, 26)
                    .Select(e => ((char)e).ToString())
                    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                    .OrderBy(e => Guid.NewGuid())
                    .Take(20)
                    .ToList().ForEach(e => builder.Append(e));
                string _Ident = builder.ToString();


                //string currentDirectory = AppContext.BaseDirectory;
                //string keysAuth = Path.Combine(AppContext.BaseDirectory, @"..\files\Key");
                //string keyChat = Path.Combine(AppContext.BaseDirectory, @"..\..\ServerChat\files\Key");
                //const string privKey = @"\PrivateKey.json";
                //const string pubKey = @"\PublicKey.json";
                //Console.WriteLine("Ключи шифрования созданы");

                //try
                //{
                //    File.Delete(keysAuth);
                //    File.Delete(keyChat);
                //}
                //catch { }

                //using var key = RSA.Create();
                //var privateKey = Convert.ToBase64String(key.ExportRSAPrivateKey());
                //var publicKey = Convert.ToBase64String(key.ExportRSAPublicKey());

                //var privateKeyJson = JsonSerializer.Serialize(new
                //{
                //    Private = privateKey
                //}, new JsonSerializerOptions { WriteIndented = true });

                //var publicKeyJson = JsonSerializer.Serialize(new
                //{
                //    Public = publicKey
                //}, new JsonSerializerOptions { WriteIndented = true });

                //Directory.CreateDirectory(keysAuth);
                //Directory.CreateDirectory(keyChat);
                //try
                //{
                //    File.WriteAllText(keysAuth + privKey, privateKeyJson);
                //    File.WriteAllText(keyChat + pubKey, publicKeyJson);
                //}
                //catch { }









                modelBuilder.Entity<UserDB>().HasData(new UserDB{ id = 1, Login = "Admin", Password = res.ToString().GetSha1().GetSha1(), Role = "Admin" });
                modelBuilder.Entity<ServerDB>().HasData(new ServerDB { id = 1, Title= "LiteCall",Description= "Community Server" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 1,Questions= "Какое прозвище было у вас в детстве?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 2, Questions = "Как звали вашего лучшего друга детства?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 3, Questions = "На какой улице вы жили в третьем классе?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 4, Questions = "Какую школу вы посещали в шестом классе?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 5, Questions = "Как звали вашу первую плюшевую игрушку?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 6, Questions = "В каком месте встретились ваши родители?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 7, Questions = "Как звали вашего учителя в третьем классе?" });
                modelBuilder.Entity<SecurityQuestions>().HasData(new SecurityQuestions { id = 8, Questions = "В каком городе живет ваш ближайший родственник?" });
            }
        }
    }
}
