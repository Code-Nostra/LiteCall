using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.DAL.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
		IQueryable<User> Users { get; }
		Task<User> FindByName(string name);

		Task<bool> AddNewUser(User _user);
	}
}
