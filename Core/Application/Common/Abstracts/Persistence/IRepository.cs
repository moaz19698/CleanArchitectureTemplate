using Adahi.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adahi.Application.Common.Abstracts.Persistence
{
    public interface IRepository<T> : IReadOnlyRepository<T> where T : Entity
    {

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
        /// The add async.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        void AddAsync(IEnumerable<T> entities);

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
        void Update(T entityToUpdate);

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
    }
}
