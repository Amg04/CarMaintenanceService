using BLLProject.Specifications;
using DALProject.Models.BaseClasses;
using System.Collections.Generic;
namespace BLLProject.Interfaces
{
    public interface IGenericRepository<T> where T : class, IAllowedEntity
    {
        void Add(T entity);
        void Delete(T entity);
        T Get(int Id);
        IEnumerable<T> GetAll();
        void Update(T entity);
        T GetEntityWithSpec(ISpecification<T> spec);
        IEnumerable<T> GetAllWithSpec(ISpecification<T> spec);
        void RemoveRange(IEnumerable<T> entities);
    }
}
