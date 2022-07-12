using CommerceProject.Business.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommerceProject.Business.HelperClasses
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll(bool asNoTracking = false, string[] includeTableList = null);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string[] includeList = null);

        T GetSingle(Expression<Func<T, bool>> predicate = null, bool asNoTracking = false, string[] includeTableList = null);

        T GetFirst(Expression<Func<T, bool>> predicate = null, bool asNoTracking = false, string[] includeTableList = null);

        T GetLast(Expression<Func<T, bool>> predicate = null, bool asNoTracking = false, string[] includeTableList = null);

        T GetById(int id, string[] includeTableList = null);

        T Add(T entity);

        T Delete(T entity);

        T Edit(T entity);

        bool Save();

        #region Cache

        List<T> GetAllFromCache(CacheDataObj cacheObject, string[] includeTableList = null);

        List<T> FindByFromCache(Expression<Func<T, bool>> predicate, CacheDataObj cacheObject, string[] includeTableList = null);

        T GetSingleFromCache(CacheDataObj cacheObject, Expression<Func<T, bool>> predicate = null, string[] includeTableList = null);

        T GetFirstFromCache(CacheDataObj cacheObject, Expression<Func<T, bool>> predicate = null, string[] includeTableList = null);

        #endregion
    }
}
