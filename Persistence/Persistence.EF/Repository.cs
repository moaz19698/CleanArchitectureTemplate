using Adahi.Application.Common.Abstracts.Persistence;
using Adahi.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adahi.Persistence.EF.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public Repository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }
        #endregion

        #region Context

        /// <summary>
        /// Gets the context.
        /// </summary>
        protected internal ApplicationDbContext Context { get; }
        #endregion

        #region DbSet
        /// <summary>
        /// Gets the db set.
        /// </summary>
        protected internal DbSet<T> DbSet { get; }


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
        public virtual async Task<T> AddAsync(T entity)
        {
            var addedEntity = await DbSet.AddAsync(entity);

            return addedEntity.Entity;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        public void AddAsync(IEnumerable<T> entities)
        {
            DbSet.AddRangeAsync(entities);
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
            //DbSet.Attach(entityToUpdate);
            //Context.Entry(entityToUpdate).State = EntityState.Modified;
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
            var entity = DbSet.Find(id);

            return Delete(entity ?? throw new ArgumentException($"the Id:{id} isn't exist in {DbSet.EntityType.GetTableName()}"));
        }


        #endregion

        #region Read Method
        public IDataQuery<T> GetQuery()
        {
            return new DataQuery<T>(DbSet);
        }
            
        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public virtual T? GetById(object id)
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
        public async Task<T?> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }
        #endregion





    }

}

