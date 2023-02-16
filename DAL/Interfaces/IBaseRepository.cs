using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IBaseRepository<T> 
		where T : class

	{
        Task<bool> Add(T entity);

        Task<T> GetValueByid(int id);

		Task<T> GetFirstDefault();

		Task<IEnumerable<T>> GetAll();

		Task<bool> Update(T entity);

		Task<bool> Delete(T entity);

    }
}
