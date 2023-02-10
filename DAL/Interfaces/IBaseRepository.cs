using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainServer.DAL.Interfaces
{
    public interface IBaseRepository<T>
    {
        bool Create(T entity);

        T GetValue(int id);

        Task<IEnumerable<T>> Select();

        bool Delete(T entity);
    }
}
