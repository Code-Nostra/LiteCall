using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
	public class BaseRepositoryGeneric<TContext,T> : BaseRepository<TContext, T> 
		where T : class, IEntity 
		where TContext : DbContext
	{
		private readonly TContext _db;

		public IQueryable<T> Users => _dbSet;

		public bool AutoSaveChanges { get; set; } = true;

		public BaseRepositoryGeneric(TContext db, ILoggerFactory loggerFactory) : base(db, loggerFactory.CreateLogger("logs")) 
		{
			
		}

	}
}
