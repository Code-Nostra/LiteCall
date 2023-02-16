using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.UnitOfWork.ServerAuthorization
{
    public class UnitOfWorkAuth : IUnitOfWorkAuth, IDisposable
    {
        private readonly ServerAuthDbContext _db;
        private readonly ILogger _logger;


        private IUserRepository<ServerAuthDbContext> _userRepository;
        public IUserRepository<ServerAuthDbContext> Users =>
            _userRepository ??= new UserRepository<ServerAuthDbContext>(_db, _logger);

        private IBaseRepository<ServerAuthDbContext,SequrityQuestion> _sequrityQuestion;
        public IBaseRepository<ServerAuthDbContext,SequrityQuestion> SequrityQuestions =>
            _sequrityQuestion ??= new BaseRepository<ServerAuthDbContext, SequrityQuestion>(_db, _logger);

		private IServerRepository<ServerAuthDbContext> _servers;
		public IServerRepository<ServerAuthDbContext> Servers => _servers ??= new ServerRepository<ServerAuthDbContext>(_db, _logger);

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


		public UnitOfWorkAuth(ServerAuthDbContext db, ILoggerFactory loggerFactory)
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
