using DAL.EF;
using DAL.Entities;
using MainServer.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MainServer.DAL.Repositories
{
	public class UserRepository : IUserRepository 
	{
		private readonly ApplicationDbContext _db;

		public IQueryable<User> Users =>_db.Users;
		public bool AutoSaveChanges { get; set; } = true;

		public UserRepository(ApplicationDbContext db)
		{
			_db = db;
		}
		

		public async Task<User> FindByName(string _login)
		{
			return await _db.Users.SingleOrDefaultAsync(t=>t.Login==_login).ConfigureAwait(false); 
		}

		public async Task<bool> AddNewUser(User _user)
		{
			await _db.AddAsync<User>(_user).ConfigureAwait(false);

			if (AutoSaveChanges)
				await SaveChanges().ConfigureAwait(false);

			return true;
		}

		public async Task<int> SaveChanges()
		{
			return await _db.SaveChangesAsync().ConfigureAwait(false);
		}
		public async Task<SequrityQuestion> GetSequrityQuestionById(int _id)
		{
			return await _db.SecurityQuestions.SingleOrDefaultAsync(t => t.id == _id).ConfigureAwait(false);
		}



		public async Task<IEnumerable<User>> Select()
		{
			return await _db.Users.ToListAsync();
		}

		public async Task<bool> Exist(string _Login)
		{
			return await _db.Users.AnyAsync(t => t.Login == _Login).ConfigureAwait(false);
		}

		public Task<bool> Create(User entity)
		{
			throw new NotImplementedException();
		}

		public Task<User> GetValue(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Delete(User entity)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> Update(User _user)
		{
			if (_user is null)
				throw new ArgumentNullException(nameof(_user));

			_db.Update(_user);

			if (AutoSaveChanges)
				await SaveChanges().ConfigureAwait(false);

			return true;
		}



		public async Task<IEnumerable<User>> GetAll()
		{
			return await _db.Users.ToListAsync();
		}


	}
}
