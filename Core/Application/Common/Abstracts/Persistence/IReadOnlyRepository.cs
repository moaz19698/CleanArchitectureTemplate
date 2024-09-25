using Adahi.Domain.Common;
using  Adahi.Application.Common.Abstracts.Persistence;

namespace Adahi.Application.Common.Abstracts.Persistence
{
    public interface IReadOnlyRepository<T> where T : Entity
    {
        IDataQuery<T> GetQuery();
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
        /// The get by id async.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<T?> GetByIdAsync(object id);
    }
}
