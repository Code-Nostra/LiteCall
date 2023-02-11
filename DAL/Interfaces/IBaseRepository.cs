using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.DAL.Interfaces
{
    public interface IBaseRepository<T> where T : class
	{
        Task<bool> Create(T entity);

        Task<T> GetValue(int id);

		Task<IEnumerable<T>> GetAll();

		Task<bool> Update(T entity);

		Task<bool> Delete(T entity);

    }
}
