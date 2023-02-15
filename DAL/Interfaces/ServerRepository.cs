using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
	public class ServerRepository<TContext> : BaseRepository<TContext, Server>, IServerRepository where TContext : DbContext
	{
		private readonly TContext _db;

		public ServerRepository(TContext db, ILogger logger) : base(db, logger){}

		public IQueryable<Server> Servers=> _dbSet;


		public async Task<Server> GetByTitle(string title)
		{
			return await Servers.FirstOrDefaultAsync(x => x.Title == title);
		}
		public async Task<Server> GetByIdent(string ident)
		{
			return await Servers.FirstOrDefaultAsync(x => x.Ident == ident);
		}

	}
}
