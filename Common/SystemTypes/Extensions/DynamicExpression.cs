using System.Linq.Expressions;

namespace Common.SystemTypes.Extensions
{
    public static class DynamicLambdaExpression
    {
        public static Expression<Func<TEntity, bool>>? CreateEqualExpression<TEntity, TValue>(string propertyName, TValue value) where TEntity : class
        {
            if (typeof(TEntity).GetProperty(propertyName) != null)
            {
                var newParam = Expression.Parameter(typeof(TEntity));
                return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(Expression.Convert(Expression.Property(newParam, propertyName),
                                                          typeof(TValue)), Expression.Constant(value)), newParam);
            }
            return null;
        }
        public static Expression<Func<TEntity, bool>>? CreateEqualExpression<TEntity, TValue>(MemberExpression member, TValue value) where TEntity : class
        {
            if (member != null)
            {
                var newParam = Expression.Parameter(typeof(TEntity));
                var body = Expression.Equal(member, Expression.Constant(value, member.Type));

                return Expression.Lambda<Func<TEntity, bool>>(body, newParam);
            }
            return null;

        }

        public static Expression<Func<TEntity, bool>>? CreateEqualExpressionFromString<TEntity>(string stringExpression, dynamic value) where TEntity : class
        {
            if (!string.IsNullOrEmpty(stringExpression))
            {
                var memberList = stringExpression.Split('.');
                var newParam = Expression.Parameter(typeof(TEntity), memberList[0]);
                var member = Expression.Property(newParam, memberList[1]);

                if (memberList.Length > 2)
                {
                    for (int i = 2; i < memberList.Length; i++)
                    {
                        member = AddToExpression(member, memberList[i]);
                    }
                }
                var body = Expression.Equal(member, Expression.Constant(value, member.Type));

                static MemberExpression AddToExpression(MemberExpression member,
                                                        string peroperty) => Expression.Property(member, peroperty);

                return Expression.Lambda<Func<TEntity, bool>>(body, newParam);
            }
            return null;
        }

        public static dynamic ConvertToType(object value, string type)
        {
            return Convert.ChangeType(value, Type.GetType(type) ?? throw new ArgumentException($"the {type} is not a type name"));
        }

    }
}
