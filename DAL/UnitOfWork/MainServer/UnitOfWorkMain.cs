using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.UnitOfWork.MainServer
{
    public class UnitOfWorkMain : IUnitOfWorkMain, IDisposable
    {
        private readonly MainServerDbContext _db;
        private readonly ILogger _logger;


        private IUserRepository<MainServerDbContext> _userRepository;
        public IUserRepository<MainServerDbContext> Users =>
            _userRepository ??= new UserRepository<MainServerDbContext>(_db, _logger);

        private IBaseRepository<MainServerDbContext,SequrityQuestion> _sequrityQuestions;
        public IBaseRepository<MainServerDbContext,SequrityQuestion> SequrityQuestions =>
            _sequrityQuestions ??= new BaseRepository<MainServerDbContext, SequrityQuestion>(_db, _logger);

        private IServerRepository<MainServerDbContext> _servers;
        public IServerRepository<MainServerDbContext> Servers=>_servers ??=new ServerRepository<MainServerDbContext>(_db, _logger);

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


        public UnitOfWorkMain(MainServerDbContext db, ILoggerFactory loggerFactory)
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
