using System.Linq.Expressions;

namespace Common.Reflection.Extensions
{
    public static class ExpressionObjectExtension
    {
        public static string PropertyName<T>(this Expression<Func<T, object>> expression)
        {
            if (expression.Body is not MemberExpression body)
            {
                body = (MemberExpression)((UnaryExpression)expression.Body).Operand;
            }

            return body.Member.Name;
        }
        public static MemberExpression Member<T>(this Expression<Func<T, object>> expression)
        {
            if (expression.Body is not MemberExpression member)
            {
                // The property access might be getting converted to object to match the func
                // If so, get the operand and see if that's a member expression
                member = (MemberExpression)((UnaryExpression)expression.Body).Operand;
            }
            return member ?? throw new ArgumentException("Action must be a member expression.");
        }

    }
}
