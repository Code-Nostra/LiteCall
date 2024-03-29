﻿using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork.ServerAuthorization
{
    public interface IUnitOfWorkAuth
    {
        IUserRepository Users { get; }

        IBaseRepository<SequrityQuestion> SequrityQuestions { get; }

		IServerRepository Servers { get; }

		Task<int> SaveChangesAsync();
    }
}
