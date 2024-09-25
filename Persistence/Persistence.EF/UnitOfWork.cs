using Adahi.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Adahi.Application.Common.Abstracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adahi.Persistence.EF.Repositories;

namespace Adahi.Persistence.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext DbContext { get; set; }
        public IServiceProvider ServiceProvider { get; }

        #region Constructor
        public UnitOfWork(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            ServiceProvider = serviceProvider;
        }
        #endregion

        #region Repositories
        public IRepository<Product> ProductRepository => ServiceProvider.GetService<IRepository<Product>>() ?? throw new Exception($"Service {nameof(IRepository<Product>)}  not registerd");
        public IRepository<Ritual> RitualRepository => ServiceProvider.GetService<IRepository<Ritual>>() ?? throw new Exception($"Service {nameof(IRepository<Ritual>)} not registerd");


        #endregion

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
