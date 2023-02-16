using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork.MainServer
{
    public interface IUnitOfWorkMain
    {
        IUserRepository<MainServerDbContext> Users { get; }

        IBaseRepository<MainServerDbContext,SequrityQuestion> SequrityQuestions { get; }

        IServerRepository<MainServerDbContext> Servers { get; }

        Task<int> SaveChangesAsync();
    }
}
