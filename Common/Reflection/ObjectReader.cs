using System.Text;

namespace Common.Reflection
{
    public static class ObjectReader
    {
        public static string GetValueForCompainedFields(string[] filedNameArArray, object x)
        {
            StringBuilder result = new();

            foreach (var filed in filedNameArArray)
            {
                var value = x.GetType().GetProperty(filed.Trim())?.GetValue(x, null) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    result.Append(value);
                    result.Append(' ');
                }
            }

            return result.ToString();
        }
    }
}
