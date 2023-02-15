using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
	public interface IServerRepository:IBaseRepository<Server>
	{
		Task<Server> GetByTitle(string id);

		Task<Server> GetByIdent(string id);
	}
}
