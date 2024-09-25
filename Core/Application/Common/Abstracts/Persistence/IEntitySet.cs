using CleanArchitecture.Common.Linq.Model;
using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CleanArchitecture.Application.Common.Abstracts.Persistence
{
    public interface IEntitySet<T> where T : Entity, IAggregateRoot
    {
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
        T Add(T entity);

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        void Add(IEnumerable<T> entities);

        /// <summary>
        /// The add .
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update by Specific Object 
        /// </summary>
        /// <param name="id">Key</param>
        /// <param name="t"> updated Object</param>
        void Update(object id, T entity);

        /// <summary>
        /// Updated
        /// </summary>
        /// <param name="entityToUpdate"> Updated Object</param>
        void Update(T updatedEntity);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Delete(T entity);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool DeleteById(object id);


        #endregion

        #region ReadOnly Methods

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T? GetById(object id);

        /// <summary>
        /// The get by id .
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Track the eniities of the query
        /// </summary>
        /// <returns></returns>
        IEntitySet<T> AsTracking();

        /// <summary>
        /// AsNoTracking
        /// </summary>
        /// <returns></returns>
        IEntitySet<T> AsNoTracking();

        /// <summary>
        /// The where.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The result.</returns>
        IEntitySet<T> Where(Expression<Func<T, bool>> filter);

        /// <summary>
        /// The WhereIf to assert from condition before applying filter
        /// </summary>
        /// <param name="ifCondition">the precondition to apply the filter or not</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEntitySet<T> WhereIf(bool ifCondition, Expression<Func<T, bool>> filter);

        /// <summary>
        /// IncludeAll levels
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IEntitySet<T> Include(string include);

        /// <summary>
        /// IncludeAll levels
        /// </summary>
        /// <param name="Include"></param>
        /// <returns></returns>
        IEntitySet<T> Include(Expression<Func<T, object>> includeProperty);

        /// <summary>
        /// Include
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IEntitySet<T> Include(Func<IQueryable<T>, IIncludableQueryable<T, object?>> includes);

        /// <summary>
        /// The order by.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The result.</returns>
        IEntitySet<T> OrderBy<Tkey>(Expression<Func<T, Tkey>> expression);

        /// <summary>
        /// The Order By Descending.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>The result.</returns>
        IEntitySet<T> OrderByDescending<Tkey>(Expression<Func<T, Tkey>> expression);

        /// <summary>
        /// Order by using dynamic sort
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="sortDirection"></param>
        /// <param name="anotherLevel"></param>
        /// <returns></returns>
        IEntitySet<T> DynamicOrderBy(string propertyName, ListSortDirection sortDirection = ListSortDirection.Ascending);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderPropertyList"></param>
        /// <returns></returns>
        IEntitySet<T> DynamicOrderBy(List<DynamicOrderFields>? orderPropertyList);

        /// <summary>
        /// The fist or default
        /// </summary>
        /// <returns></returns>
        T? FirstOrDefault(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// the FirstOrDefault
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TResult? FirstOrDefault<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// The fist or default with project each element of sequence into a new form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// The fist or default with project each element of sequence into a new form
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <param name="predicate"></param>
        /// <returns>TResult</returns>
        Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
        /// <summary>
        /// Any
        /// </summary>
        /// <returns></returns>
        bool Any();

        /// <summary>
        /// AnyAsync
        /// </summary>
        /// <returns></returns>
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// AnyAsync
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

        /// <summary>
        /// The Top
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        List<T> Top(int count);

        /// <summary>
        /// Get top result of sequence  
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        List<TResult> Top<TResult>(int count, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// The Top 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<T>> TopAsync(int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// The Top 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<List<TResult>> TopAsync<TResult>(int count, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

        /// <summary>
        /// the Last get the last items 
        /// </summary>
        /// <param name="count">count of items</param>
        /// <returns></returns>
        List<T> Last(int count);

        /// <summary>
        /// The last  get the last elements
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="count"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        List<TResult> Last<TResult>(int count, Expression<Func<T, TResult>> selector);

        /// <summary>
        ///  the last items 
        /// </summary>
        /// <param name="count">count of items</param>
        /// <returns></returns>
        Task<List<T>> LastAsync(int count, CancellationToken cancellationToken = default);

        Task<List<TResult>> LastAsync<TResult>(int count, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

        /// <summary>
        /// the Count
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// the CountAsync
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Maximum value
        /// </summary>
        /// <returns></returns>
        T? Max();

        /// <summary>
        /// MaxAsync
        /// </summary>
        /// <returns>Maximium value in operation</returns>
        Task<T> MaxAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Min value
        /// </summary>
        /// <returns></returns>
        T? Min();

        /// <summary>
        /// Min value  
        /// </summary>
        /// <returns></returns>
        Task<T?> MinAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The to list.
        /// </summary>
        /// <returns>The result.</returns>
        List<T> ToList();

        /// <summary>
        /// The to list.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        List<TResult> ToList<TResult>(Expression<Func<T, TResult>> selector);

        /// <summary>
        /// The to list .
        /// </summary>
        /// <returns>The result.</returns>
        Task<List<T>> ToListAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// ToListAsync
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<List<TResult>> ToListAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);

        /// <summary>
        /// AsEnumerable
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> AsEnumerable();

        /// <summary>
        /// AsEnumerable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        IEnumerable<TResult> AsEnumerable<TResult>(Expression<Func<T, TResult>> selector);

        /// <summary>
        /// AsAsyncEnumerable
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<T> AsAsyncEnumerable();

        /// <summary>
        /// AsAsyncEnumerable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        IAsyncEnumerable<TResult> AsAsyncEnumerable<TResult>(Expression<Func<T, TResult>> selector);

        /// <summary>
        /// The to pages list.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The result.</returns>
        (List<T>, int totalCount) ToPagedList(int pageIndex, int pageSize);

        /// <summary>
        /// The to pages list.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        (List<TResult>, int totalCount) ToPagedList<TResult>(int pageIndex, int pageSize, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// The to pages list.
        /// </summary>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The result.</returns>
        Task<(List<T>, int totalCount)> ToPagedListAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// the ToPagedListAsync
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<(List<TResult> Items, int totalCount)> ToPagedListAsync<TResult>(int pageIndex, int pageSize, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
        #endregion
    }
}
