using DAL.EF;
using DAL.Entities;
using MainServer.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MainServer.DAL.Repositories
{
	public class UserRepository : BaseRepository<User>, IUserRepository 
	{
		private readonly ApplicationDbContext _db;

		public IQueryable<User> Users => _dbSet;

		public bool AutoSaveChanges { get; set; } = true;

		public UserRepository(ApplicationDbContext db, ILogger logger) : base(db, logger) { }


		public async Task<User> FindByName(string _login)
		{
			return await _dbSet.SingleOrDefaultAsync(t => t.Login == _login).ConfigureAwait(false);
		}

		public async Task<IEnumerable<User>> Select()
		{
			return await _db.Users.ToListAsync();
		}
	}
}
