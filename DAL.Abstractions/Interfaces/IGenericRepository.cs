using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstractions.Interfaces
{
    public interface IGenericRepository<T>
    {
        public Task<List<T>> DeserializeAsync(Func<T, bool> predicate = null);

        public Task<T> SerializeAsync(T entity);

        public Task<T?> UpdateAsync(T entity);

        public Task<bool> DeleteAsync(T entity);
    }
}
