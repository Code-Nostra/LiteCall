using DAL.EF;
using DAL.Entities;
using MainServer.DAL.Interfaces;
using MainServer.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MainServer.DAL.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly ApplicationDbContext _db;
		private readonly ILogger _logger;


		private IUserRepository _userRepository;
		public IUserRepository Users => 
			_userRepository ??= new UserRepository(_db, _logger);

		private IBaseRepository<SequrityQuestion> _sequrityQuestion;
		public IBaseRepository<SequrityQuestion> SequrityQuestions=>
			_sequrityQuestion??= new BaseRepository<SequrityQuestion>(_db, _logger);

		#region Подробнее
		//public IBaseRepository<SequrityQuestion> SequrityQuestions
		//{
		//	get
		//	{

		//		if (_sequrityQuestion== null)
		//		{
		//			_sequrityQuestion = new BaseRepository<SequrityQuestion>(_db,_logger);
		//		}
		//		return _sequrityQuestion;
		//	}
		//}
		#endregion


		public UnitOfWork(ApplicationDbContext db, ILoggerFactory loggerFactory)
		{
			_db = db;

			_logger = loggerFactory.CreateLogger("logs");

			//Users = new UserRepository(db, _logger);

			//SequrityQuestions = new BaseRepository<SequrityQuestion>(db, _logger);
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _db.SaveChangesAsync();
		}

		public void Dispose()
		{
			_db.Dispose();
		}
	}
}
