using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.DAL.Interfaces
{
    internal interface IUserRepository : IBaseRepository<User>
    {
        User FindByName(string name);
    }
}
