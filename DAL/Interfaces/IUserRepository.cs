using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserRepository<TContext> : IBaseRepository<TContext, User>
		where TContext : DbContext
	{
		IQueryable<User> Users { get; }
		Task<User> FindByName(string name);

	}
}
