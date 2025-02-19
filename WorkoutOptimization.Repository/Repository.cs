using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        WorkoutOptimizationDbContext _db;

        protected Repository(WorkoutOptimizationDbContext db)
        {
            _db = db;
        }

        public void Create(T entity)
        {
            _db.Set<T>().Add(entity);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            _db.Set<T>().Remove(Read(id));
            _db.SaveChanges();
        }

        public T Read(int id)
        {
            var entity= _db.Set<T>().Find(id);
            if (entity == null)
            {
                throw new InvalidOperationException(typeof(T).Name + " not found with: " + id);
            }
            return entity;
        }

        public IQueryable<T> ReadAll()
        {
            return _db.Set<T>();
        }

        public void Update(T entity)
        {
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Any()) ??
                typeof(T).GetProperties().FirstOrDefault(p => p.Name.ToLower().EndsWith("id"));

            if (keyProperty == null)
            {
                throw new InvalidOperationException("Missing key attribute");
            }

            var keyValue = keyProperty.GetValue(entity);
            if(keyValue==null)
            {
                throw new InvalidOperationException("Entity ID cannot be null");
            }

            var dbEntity = _db.Set<T>().Find(keyValue);
            if (dbEntity == null)
            {
                throw new InvalidOperationException("DBEntity not found!");
            }

            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.Name.ToString() != keyProperty.Name.ToString())
                {
                    prop.SetValue(dbEntity, prop.GetValue(entity));
                }
            }
            _db.SaveChanges();
        }
    }
}
