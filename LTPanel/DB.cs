using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.IO;


namespace LTPanel
{
    public class DB : DbContext
    {
        public DbSet<UserDB> Users { get; set; }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@$"Data Source=C:\Users\PC\source\repos\ServerSignalR\ServerAuthorization\AuthorizationServ\bin\Debug\net5.0\LTdb_sqlite.db");
        }

        
    }
}
