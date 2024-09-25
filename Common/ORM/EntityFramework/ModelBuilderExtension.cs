using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Common.ORM.EntityFramework
{
    public static class ModelBuilderExtension
    {
        public static void ApplyGlobalFilter<T>(this ModelBuilder modelBuilder, string propertyName, T value)
        {
            modelBuilder.Model.GetEntityTypes()
                .Where(x => x.FindProperty(propertyName) != null)
                .Select(x => x.ClrType)
                .ToList()
                .ForEach(entityType =>
                {
                    var newParam = Expression.Parameter(entityType);
                    var filter = Expression.Lambda(Expression.Equal(Expression.Convert(Expression.Property(newParam, propertyName),
                                                                                       typeof(T)), Expression.Constant(value)), newParam);
                    modelBuilder.Entity(entityType).HasQueryFilter(filter);
                });
        }

        public static void ApplyGlobalFilter<T>(this ModelBuilder modelBuilder, Expression<Func<T, bool>> expressionFilter)
        {
            modelBuilder.Model
                        .GetEntityTypes()
                        .Select(e => e.ClrType)
                        .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsInterface)
                        .ToList()
                        .ForEach(entityType =>
                        {
                            var newParam = Expression.Parameter(entityType);
                            var body = ReplacingExpressionVisitor.Replace(expressionFilter.Parameters[0], newParam, expressionFilter.Body);
                            var lambdaExpression = Expression.Lambda(body, newParam);
                            modelBuilder.Entity(entityType).HasQueryFilter(lambdaExpression);
                        });
        }
    }
}
