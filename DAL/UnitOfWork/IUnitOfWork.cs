using DAL.Entities;
using MainServer.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.DAL.UnitOfWork
{
	public interface IUnitOfWork
	{
		IUserRepository Users { get; }

		IBaseRepository<SequrityQuestion> SequrityQuestions { get; }

		Task<int> SaveChangesAsync();
	}
}
