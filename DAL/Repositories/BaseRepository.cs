using DAL.EF;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DAL.Repositories
{
	public class BaseRepository<TContext,T> : IBaseRepository<TContext, T> 
		where T : class, IEntity 
		where TContext : DbContext
	{

		private readonly TContext _db;
		protected readonly DbSet<T> _dbSet;
		protected IQueryable<T> table => _dbSet;
		public readonly ILogger _logger;

		public BaseRepository(TContext db, ILogger logger)
		{
			_db = db;
			_dbSet = db.Set<T>();

			_logger = logger;
		}

		public virtual async Task<bool> Add(T entity)
		{
			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			if (await Exist(entity))
				return false;

			await _db.AddAsync(entity).ConfigureAwait(false);

			return true;
		}

		public async Task<bool> Delete(T entity)
		{
			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			if (!await Exist(entity))
				return false;

			_db.Remove(entity);


			return true;
		}
		

		public async Task<IEnumerable<T>> GetAll()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<T> GetValueByid(int id)
		{
			return await table.SingleOrDefaultAsync(t=>t.id== id);
		}

		public async Task<T> GetFirstDefault()
		{
			return await table.FirstOrDefaultAsync();
		}

		public async Task<bool> Update(T entity)
		{
			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			_dbSet.Update(entity);

			return true;
		}

		public async Task<bool> Exist(T entity, CancellationToken cancel = default) =>
			entity is null ?
				throw new ArgumentNullException(nameof(entity)) :
				await table.AnyAsync(t=>t.id==entity.id).ConfigureAwait(false);
	}
}
