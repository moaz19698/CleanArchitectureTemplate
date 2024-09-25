using Microsoft.EntityFrameworkCore;

namespace Common.ORM.EntityFramework
{
    public static class DbContextExtension
    {
        //public static IQueryable<object>? GetDbSetByEntityName(this DbContext dbContext, string entityName)
        //{
        //    Type? entityType = dbContext.Model.GetEntityTypes()
        //        .FirstOrDefault(e => e.ClrType.Name == entityName)?
        //        .ClrType;

        //    return entityType != null
        //        ? (IQueryable<object>?)(typeof(DbContextExtension)?
        //        .GetMethod(nameof(GetDbset))?
        //        .MakeGenericMethod(entityType)
        //        .Invoke(null, new object[] { dbContext }) as dynamic)
        //        : null;
        //}

        //public static IQueryable<T> GetDbset<T>(DbContext dbContext) where T : class
        //{
        //    return dbContext.Set<T>().AsNoTracking().AsQueryable();
        //}




    }
}
