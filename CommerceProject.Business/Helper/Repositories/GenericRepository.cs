using CommerceProject.Business.Custom;
using CommerceProject.Business.Entities;
using CommerceProject.Business.Helper.Caching;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace CommerceProject.Business.HelperClasses
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        DbEntities _entities;

        public GenericRepository()
        {
            _entities = new DbEntities();
        }

        public virtual IQueryable<T> GetAll(bool asNoTracking = false, string[] includeTableList = null)
        {
            IQueryable<T> query = _entities.Set<T>();

            if (includeTableList != null)
            {
                foreach (var includeTable in includeTableList)
                    query = query.Include(includeTable);
            }

            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string[] includeTableList = null)
        {
            IQueryable<T> query = _entities.Set<T>();

            if (includeTableList != null)
            {
                foreach (var includeTable in includeTableList)
                    query = query.Include(includeTable);
            }

            if (asNoTracking)
                query = query.AsNoTracking().Where(predicate);
            else
                query = query.Where(predicate);

            return query;
        }

        public virtual T GetSingle(Expression<Func<T, bool>> predicate = null, bool asNoTracking = false, string[] includeTableList = null)
        {
            IQueryable<T> query = _entities.Set<T>();
            T dataObject = null;

            if (includeTableList != null)
            {
                foreach (var includeTable in includeTableList)
                    query = query.Include(includeTable);
            }

            if (predicate == null)
            {
                if (asNoTracking)
                    dataObject = query.AsNoTracking().SingleOrDefault();
                else
                    dataObject = query.SingleOrDefault();
            }
            else
            {
                if (asNoTracking)
                    dataObject = query.AsNoTracking().SingleOrDefault(predicate);
                else
                    dataObject = query.SingleOrDefault(predicate);
            }

            return dataObject;
        }

        public virtual T GetFirst(Expression<Func<T, bool>> predicate = null, bool asNoTracking = false, string[] includeTableList = null)
        {
            IQueryable<T> query = _entities.Set<T>();
            T dataObject = null;

            if (includeTableList != null)
            {
                foreach (var includeTable in includeTableList)
                    query = query.Include(includeTable);
            }

            if (predicate == null)
            {
                if (asNoTracking)
                    dataObject = query.AsNoTracking().FirstOrDefault();
                else
                    dataObject = query.FirstOrDefault();
            }
            else
            {
                if (asNoTracking)
                    dataObject = query.AsNoTracking().FirstOrDefault(predicate);
                else
                    dataObject = query.FirstOrDefault(predicate);
            }

            return dataObject;
        }

        public virtual T GetLast(Expression<Func<T, bool>> predicate = null, bool asNoTracking = false, string[] includeTableList = null)
        {
            IQueryable<T> query = _entities.Set<T>();
            T dataObject = null;

            if (includeTableList != null)
            {
                foreach (var includeTable in includeTableList)
                    query = query.Include(includeTable);
            }

            if (predicate == null)
            {
                if (asNoTracking)
                    dataObject = query.AsNoTracking().LastOrDefault();
                else
                    dataObject = query.LastOrDefault();
            }
            else
            {
                if (asNoTracking)
                    dataObject = query.AsNoTracking().LastOrDefault(predicate);
                else
                    dataObject = query.LastOrDefault(predicate);
            }

            return dataObject;
        }

        public virtual T GetById(int id, string[] includeTableList = null)
        {
            var entity = _entities.Set<T>();

            if (includeTableList != null)
            {
                foreach (var includeTable in includeTableList)
                    entity.Include(includeTable);
            }

            T dataObject = entity.Find(id);

            return dataObject;
        }

        public virtual T Add(T entity)
        {
            _entities.Set<T>().Add(entity);
            return entity;
        }

        public virtual T Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
            return entity;
        }

        public virtual T Edit(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual bool Save()
        {
            try
            {
                _entities.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Cache

        public virtual List<T> GetAllFromCache(CacheDataObj cacheObject, string[] includeTableList = null)
        {
            object cachedObject = CacheHelper.CacheRead(cacheObject.ToString());

            if (cachedObject == null)
            {
                IQueryable<T> query = _entities.Set<T>();

                if (includeTableList != null)
                {
                    foreach (var includeTable in includeTableList)
                        query = query.Include(includeTable);
                }

                query = query.AsNoTracking();
                CacheHelper.CacheWrite(cacheObject.ToString(), query.ToList());
                cachedObject = query.ToList();
            }

            return cachedObject as List<T>;
        }

        public virtual List<T> FindByFromCache(Expression<Func<T, bool>> predicate, CacheDataObj cacheObject, string[] includeTableList = null)
        {
            object cachedObject = GetAllFromCache(cacheObject, includeTableList).AsQueryable().Where(predicate).ToList();

            return cachedObject as List<T>;
        }

        public virtual T GetSingleFromCache(CacheDataObj cacheObject, Expression<Func<T, bool>> predicate = null, string[] includeTableList = null)
        {
            T cachedObject = null;

            if (predicate == null)
                cachedObject = GetAllFromCache(cacheObject, includeTableList).AsQueryable().SingleOrDefault();
            else
                cachedObject = GetAllFromCache(cacheObject, includeTableList).AsQueryable().SingleOrDefault(predicate);

            return cachedObject;
        }

        public virtual T GetFirstFromCache(CacheDataObj cacheObject, Expression<Func<T, bool>> predicate = null, string[] includeTableList = null)
        {
            T cachedObject = null;

            if (predicate == null)
                cachedObject = GetAllFromCache(cacheObject, includeTableList).AsQueryable().FirstOrDefault();
            else
                cachedObject = GetAllFromCache(cacheObject, includeTableList).AsQueryable().FirstOrDefault(predicate);

            return cachedObject;
        }

        #endregion

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_entities != null)
                _entities.Dispose();

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
