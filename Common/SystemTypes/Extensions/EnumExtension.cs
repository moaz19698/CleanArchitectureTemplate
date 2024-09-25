namespace CleanArchitecture.Common.SystemTypes.Extensions
{
    public static class EnumExtension
    {
        public static IEnumerable<TEnum> ToValuesEnumerable<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                                       .Cast<TEnum>()
                                       .AsEnumerable();

        }
    }
}
