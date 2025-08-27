using BLLProject.Interfaces;
using BLLProject.Specifications;
using DALProject.Data;
using DALProject.Models.BaseClasses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace BLLProject.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IAllowedEntity
    {
        public readonly CarAppDbContext dbContect;

        public GenericRepository(CarAppDbContext dbContect)
        {
            this.dbContect = dbContect;
        }
        public void Add(T entity)
        {
            dbContect.Add(entity);

        }

        public void Delete(T entity)
        {
            dbContect.Remove(entity);

        }
        public void Update(T entity)
        {
            dbContect.Set<T>().Update(entity);

        }

        public T Get(int Id)
        {
            return dbContect.Set<T>().Find(Id);
        }

        public IEnumerable<T> GetAll()
        {
            return dbContect.Set<T>().ToList();
        }

        public T GetEntityWithSpec(ISpecification<T> spec) =>
             SpecificationEvalutor<T>.GetQuery(dbContect.Set<T>(), spec).FirstOrDefault();

        public IEnumerable<T> GetAllWithSpec(ISpecification<T> spec) =>
             SpecificationEvalutor<T>.GetQuery(dbContect.Set<T>(), spec).AsNoTracking().ToList();

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbContect.Set<T>().RemoveRange(entities);
        }
    }
}
