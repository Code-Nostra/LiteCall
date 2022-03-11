using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationServ
{
    public class UserAuth : DbContext
    {
        public UserAuth()
        : base(@"Data Source=DESKTOP-7D8D2UC\SQLEXPRESS;Initial Catalog=LiteCall;Integrated Security=True")
        { }

        public DbSet<UserDB> UsersDB { get; set; }
        public DbSet<ServerDB> ServerDB { get; set; }
    }
}
