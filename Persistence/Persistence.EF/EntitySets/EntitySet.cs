using CleanArchitecture.Application.Common.Abstracts.Persistence;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Common.Linq.Model;
using CleanArchitecture.Domain.Common;
using Common.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel;
using System.Linq.Expressions;
using static Common.Linq.IQueryableExtension;

namespace CleanArchitecture.Persistence.EF.EntitySets
{
    public class EntitySet<T> : IEntitySet<T> where T : Entity, IAggregateRoot
    {
        #region Properties

        protected internal DbSet<T> DbSet { get; set; }
        protected internal ApplicationDbContext Context { get; set; }
        protected internal IQueryable<T> EntityQuery { get; set; }

        #endregion

        #region Constructors
        public EntitySet(ApplicationDbContext dbContext)
        {
            Context = dbContext;
            DbSet = Context.Set<T>();
            EntityQuery = DbSet;
        }
        #endregion

        #region Write Methods 
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T Add(T entity)
        {
            return DbSet.Add(entity).Entity;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public void Add(IEnumerable<T> entities)
        {
            DbSet.AddRange(entities);
        }

        /// <summary>
        /// The add async.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var addedEntity = await DbSet.AddAsync(entity, cancellationToken);
            return addedEntity.Entity;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public async Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// Update by Specific Object 
        /// </summary>
        /// <param name="id">Key</param>
        /// <param name="t"> updated Object</param>
        public virtual void Update(object id, T entity)
        {
            _ = DbSet.Find(id);

            DbSet.Update(entity);
        }

        /// <summary>
        /// Updated
        /// </summary>
        /// <param name="entityToUpdate"> Updated Object</param>
        public virtual void Update(T updatedEntity)
        {
            if (!DbSet.Contains(updatedEntity))
            {
                DbSet.Attach(updatedEntity);
            }

            DbSet.Update(updatedEntity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool Delete(T entity)
        {
            return DbSet.Remove(entity).State == EntityState.Deleted;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool DeleteById(object id)
        {
            var entity = DbSet.Find(id) ?? throw new NotFoundException($"item with Id:{id} can not be found");

            return Delete(entity);
        }


        #endregion

        #region Read Methods
        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T? GetById(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(id, cancellationToken);
        }

        /// <summary>
        /// Track the eniities of the query
        /// </summary>
        /// <returns></returns>
        public virtual IEntitySet<T> AsTracking()
        {
            EntityQuery = EntityQuery.AsTracking();

            return this;
        }
        /// <summary>
        /// Track the eniities of the query
        /// </summary>
        /// <returns></returns>
        public virtual IEntitySet<T> AsNoTracking()
        {
            EntityQuery = EntityQuery.AsNoTracking();

            return this;
        }

        /// <summary>
        /// The where.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The result.</returns>
        public virtual IEntitySet<T> Where(Expression<Func<T, bool>> filter)
        {
            EntityQuery = EntityQuery.Where(filter);

            return this;
        }

        /// <summary>
        /// The WhereIf to assert from condition before applying filter
        /// </summary>
        /// <param name="ifCondition">the precondition to apply the filter or not</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual IEntitySet<T> WhereIf(bool ifCondition, Expression<Func<T, bool>> filter)
        {
            if (ifCondition && filter != null)
            {
                EntityQuery = EntityQuery.Where(filter);
            }

            return this;
        }

        /// <summary>
        /// Include string
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public virtual IEntitySet<T> Include(string include)
        {
            if (!string.IsNullOrEmpty(include))
            {
                EntityQuery = EntityQuery.Include(include);
            }

            return this;
        }
        /// <summary>
        /// Include 
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public virtual IEntitySet<T> Include(Expression<Func<T, object>> includeProperty)
        {
            EntityQuery = EntityQuery.Include(includeProperty);

            return this;
        }
        /// <summary>
        /// Include 
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public virtual IEntitySet<T> Include(Func<IQueryable<T>, IIncludableQueryable<T, object?>> includes)
        {
            if (includes is not null)
            {
                EntityQuery = includes(EntityQuery);
            }

            return this;
        }
        /// <summary>
        /// The order by.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The result.</returns>
        public virtual IEntitySet<T> OrderBy<Tkey>(Expression<Func<T, Tkey>> expression)
        {
            if (expression != null)
            {
                EntityQuery = EntityQuery.OrderBy(expression).AsQueryable();
            }

            return this;
        }

        /// <summary>
        /// The order by.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The result.</returns>
        public virtual IEntitySet<T> OrderByDescending<Tkey>(Expression<Func<T, Tkey>> expression)
        {
            if (expression != null)
            {
                EntityQuery = EntityQuery.OrderByDescending(expression).AsQueryable();
            }

            return this;
        }

        /// <summary>
        /// Dynamic Order
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sortDirection"></param>
        /// <param name="anotherLevel"></param>
        /// <returns></returns>
        public virtual IEntitySet<T> DynamicOrderBy(string propertyName,
                                                    ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                EntityQuery = EntityQuery.DynamicOrderBy(propertyName, sortDirection);
            }

            return this;
        }

        /// <summary>
        /// Dynamic Order
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sortDirection"></param>
        /// <param name="anotherLevel"></param>
        /// <returns></returns>
        public virtual IEntitySet<T> DynamicOrderBy(List<DynamicOrderFields>? orderPropertyList)
        {
            if (orderPropertyList is not null && orderPropertyList.Any())
            {
                EntityQuery = EntityQuery.DynamicOrderBy(orderPropertyList);
            }

            return this;
        }

        /// <summary>
        /// The fist or default
        /// </summary>
        /// <returns></returns>
        public virtual T? FirstOrDefault(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? EntityQuery.FirstOrDefault() : EntityQuery.FirstOrDefault(predicate);
        }

        /// <summary>
        /// The fist or default with project each element of sequence into a new form
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <returns>TResult</returns>
        public virtual TResult? FirstOrDefault<TResult>(Expression<Func<T, TResult>> selector,
                                                        Expression<Func<T, bool>>? predicate = null)
        {
            var query = predicate == null ? EntityQuery.Select(selector)
                                          : EntityQuery.Where(predicate).Select(selector);

            return query.FirstOrDefault();
        }

        /// <summary>
        /// The fist or default async
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
                                                          CancellationToken cancellationToken = default)
        {
            return predicate != null
                ? await EntityQuery.FirstOrDefaultAsync(predicate, cancellationToken)
                : await EntityQuery.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// The fist or default with project each element of sequence into a new form
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <returns>TResult</returns>
        public virtual async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> selector,
                                                                         CancellationToken cancellationToken = default)
        {
            return await EntityQuery.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }
        /// <summary>
        /// Any
        /// </summary>
        /// <returns></returns>
        public virtual bool Any()
        {
            return EntityQuery.Any();
        }

        /// <summary>
        /// AnyAsync
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await EntityQuery.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// AnyAsync
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await EntityQuery.AnyAsync(expression, cancellationToken);
        }

        /// <summary>
        /// The Top
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual List<T> Top(int count)
        {
            return EntityQuery.Take(count).ToList();
        }

        /// <summary>
        /// Get top result of sequence  
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual List<TResult> Top<TResult>(int count, Expression<Func<T, TResult>> selector)
        {
            return EntityQuery.Select(selector).Take(count).ToList();
        }

        /// <summary>
        /// The Top async
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> TopAsync(int count, CancellationToken cancellationToken = default)
        {
            return await EntityQuery.Take(count).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// The Top async
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<List<TResult>> TopAsync<TResult>(int count, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return await EntityQuery.Select(selector).Take(count).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// the Last get the last items 
        /// </summary>
        /// <param name="count">count of items</param>
        /// <returns></returns>
        public virtual List<T> Last(int count)
        {
            return EntityQuery.TakeLast(count).ToList();
        }

        /// <summary>
        /// The last  get the last elements
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual List<TResult> Last<TResult>(int count, Expression<Func<T, TResult>> selector)
        {
            return EntityQuery.Select(selector).TakeLast(count).ToList();
        }

        /// <summary>
        ///  the last items 
        /// </summary>
        /// <param name="count">count of items</param>
        /// <returns></returns>
        public virtual async Task<List<T>> LastAsync(int count, CancellationToken cancellationToken = default)
        {
            return await EntityQuery.TakeLast(count).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TResult>> LastAsync<TResult>(int count,
                                                                    Expression<Func<T, TResult>> selector,
                                                                    CancellationToken cancellationToken = default)
        {
            return await EntityQuery.Select(selector).TakeLast(count).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// the Count
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return EntityQuery.Count();
        }

        /// <summary>
        /// the CountAsync
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await EntityQuery.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Get Maximum value
        /// </summary>
        /// <returns></returns>
        public virtual T? Max()
        {
            return EntityQuery.Max();
        }

        /// <summary>
        /// MaxAsync
        /// </summary>
        /// <returns>Maximium value in operation</returns>
        public virtual async Task<T> MaxAsync(CancellationToken cancellationToken = default)
        {
            return await EntityQuery.MaxAsync(cancellationToken);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <returns></returns>
        public virtual T? Min()
        {
            return EntityQuery.Min();
        }

        /// <summary>
        /// Min value async 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T?> MinAsync(CancellationToken cancellationToken = default)
        {
            return await EntityQuery.MinAsync(cancellationToken);
        }

        /// <summary>
        /// The to list.
        /// </summary>
        /// <returns>The result.</returns>
        public virtual List<T> ToList()
        {
            return EntityQuery.ToList();
        }

        /// <summary>
        /// The to list.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual List<TResult> ToList<TResult>(Expression<Func<T, TResult>> selector)
        {
            return EntityQuery.Select(selector).ToList();
        }

        /// <summary>
        /// The to list async.
        /// </summary>
        /// <returns>The result.</returns>
        public virtual async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await EntityQuery.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// ToListAsync
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<List<TResult>> ToListAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return await EntityQuery.Select(selector).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// AsEnumerable
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> AsEnumerable()
        {
            return EntityQuery.AsEnumerable();
        }

        /// <summary>
        /// AsEnumerable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual IEnumerable<TResult> AsEnumerable<TResult>(Expression<Func<T, TResult>> selector)
        {
            return EntityQuery.Select(selector).AsEnumerable();
        }

        /// <summary>
        /// AsAsyncEnumerable
        /// </summary>
        /// <returns></returns>
        public virtual IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            return EntityQuery.AsAsyncEnumerable();
        }

        /// <summary>
        /// AsAsyncEnumerable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<TResult> AsAsyncEnumerable<TResult>(Expression<Func<T, TResult>> selector)
        {
            return EntityQuery.Select(selector).AsAsyncEnumerable();
        }

        /// <summary>
        /// The to pages list.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The result.</returns>
        public virtual (List<T>, int totalCount) ToPagedList(int pageIndex, int pageSize)
        {
            int totalCount = EntityQuery.Count();
            var result = EntityQuery.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return (result, totalCount);
        }

        /// <summary>
        /// The to pages list.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual (List<TResult>, int totalCount) ToPagedList<TResult>(int pageIndex,
                                                                            int pageSize,
                                                                            Expression<Func<T, TResult>> selector)
        {
            int totalCount = EntityQuery.Count();
            var result = EntityQuery.Select(selector).Skip(pageIndex * pageSize).Take(pageSize).ToList();

            return (result, totalCount);
        }

        /// <summary>
        /// The to pages list.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The result.</returns>
        public virtual async Task<(List<T>, int totalCount)> ToPagedListAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            (var results, var totalCount) = await EntityQuery.ToPagedListAsync(pageIndex, pageSize, cancellationToken);

            return (results, totalCount);
        }

        /// <summary>
        /// the ToPagedListAsync
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public virtual async Task<(List<TResult> Items, int totalCount)> ToPagedListAsync<TResult>(int pageIndex,
                                                                                                   int pageSize,
                                                                                                   Expression<Func<T, TResult>> selector,
                                                                                                   CancellationToken cancellationToken = default)
        {
            (var results, var totalCount) = await EntityQuery.ToPagedListAsync(pageIndex, pageSize, selector, cancellationToken);

            return (results, totalCount);
        }


        #endregion
    }
}
