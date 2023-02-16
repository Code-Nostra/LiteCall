using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
	public interface IServerRepository<TContext>:IBaseRepository<TContext,Server> 
		where TContext:DbContext
	{
		Task<Server> GetByTitle(string id);

		Task<Server> GetByIdent(string id);
	}
}
