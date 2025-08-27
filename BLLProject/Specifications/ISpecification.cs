using DALProject.Models.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BLLProject.Specifications
{

    public interface ISpecification<T> where T : class, IAllowedEntity
    {
        public Expression<Func<T, bool>> Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; protected set; }
        public List<Func<IQueryable<T>, IQueryable<T>>> ComplexIncludes { get; protected set; }
        Expression<Func<T, object>> OrderBy { get; protected set; }
        Expression<Func<T, object>> OrderByDescending { get; protected set; }
    }

}
