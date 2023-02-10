using DAL.EF;
using DAL.Entities;
using MainServer.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.DAL.Repositories
{
	internal class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext db;

		public UserRepository(ApplicationDbContext _db)
		{
			db = _db;
		}

		public bool Create(User entity)
		{
			throw new NotImplementedException();
		}

		public bool Delete(User entity)
		{
			throw new NotImplementedException();
		}

		public User FindByName(string name)
		{
			throw new NotImplementedException();
		}

		public User GetValue(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<User>> Select()
		{
			return await db.Users.ToListAsync();
		}
	}
}
