using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.Repositories
{
	public class UserRepository<TContext> : BaseRepository<TContext,User>, IUserRepository<TContext> where TContext : DbContext
	{
		private readonly TContext _db;

		public IQueryable<User> Users => _dbSet;

		public bool AutoSaveChanges { get; set; } = true;

		public UserRepository(TContext db, ILogger logger) : base(db, logger) { }


		public async Task<User> FindByName(string _login)
		{
			return await Users.SingleOrDefaultAsync(t => t.Login == _login).ConfigureAwait(false);
		}
	}
}
