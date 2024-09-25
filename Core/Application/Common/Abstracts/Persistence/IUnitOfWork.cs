using Adahi.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Adahi.Application.Common.Abstracts.Persistence
{
    public interface IUnitOfWork
    {
        public IRepository<Product> ProductRepository { get; }
        public IRepository<Ritual> RitualRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
