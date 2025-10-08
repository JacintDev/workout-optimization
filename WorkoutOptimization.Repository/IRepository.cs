using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Repository
{
    public interface IRepository <T> where T:class
    {
        IQueryable<T> ReadAll();
        T Read(int id);
        void Create(T entity);
        public Task CreateAsync(T entity);
        void Delete(int id);
        void Update(T entity);
    }
}
