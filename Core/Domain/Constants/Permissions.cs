using CleanArchitecture.Common.SystemTypes.Extensions;

namespace CleanArchitecture.Domain.Constants
{
    public enum Modules
    {
        Cart,
        Product,
        User
    }
    public sealed class Permissions
    {
        public const string CLAIM_TYPE = "Permissions";
        public static List<string> GetAllPermissions()
        {
            List<string> premissions = [];
            Enum.GetValues<Modules>().ToList().ForEach(module =>
            {
                switch (module)
                {
                    case Modules.Cart:
                        premissions.AddRange(Cart.AllPermissions);
                        break;
                    case Modules.Product:
                        premissions.AddRange(Product.AllPermissions);
                        break;
                    case Modules.User:
                        premissions.AddRange(User.AllPermissions);
                        break;
                }
            });

            return premissions;
        }

        #region Module Permission
        public abstract class Module
        {
            protected static string GetPermissionName<TEnum>(Modules module, TEnum @enum) where TEnum : Enum
            {
                return $"{CLAIM_TYPE}.{module}.{@enum}";
            }
            internal static List<string> GetModulePermissions<TEnum>(Modules module) where TEnum : Enum
            {
                return EnumExtension.ToValuesEnumerable<TEnum>().Select(p => GetPermissionName(module, p)).ToList();
            }
        };

        #region Cart Permission
        public sealed class Cart : Module
        {
            private const string MODULE_NAME = nameof(Modules.Cart);
            private const string PERMISSION_PREFIX = $"{CLAIM_TYPE}.{MODULE_NAME}";
            private enum Permission
            {
                Add,
                Read,
                Update,
                Delete,
            }
            public const string Add = $"{PERMISSION_PREFIX}.{nameof(Permission.Add)}";
            public const string Read = $"{PERMISSION_PREFIX}.{nameof(Permission.Read)}";
            public const string Update = $"{PERMISSION_PREFIX}.{nameof(Permission.Update)}";
            public const string Delete = $"{PERMISSION_PREFIX}.{nameof(Permission.Delete)}";

            public static List<string> AllPermissions => GetModulePermissions<Permission>(Modules.Cart);
        }
        #endregion

        #region Product Permission
        public sealed class Product : Module
        {
            private const string MODULE_NAME = nameof(Modules.Product);
            private const string PERMISSION_PREFIX = $"{CLAIM_TYPE}.{MODULE_NAME}";

            private enum Permission
            {
                Add,
                Update,
                Delete,
                ReadPagedQuery,
                AddCategory,
                ReadCategories,
                ChangeProductItemAmount
            }

            public const string Add = $"{PERMISSION_PREFIX}.{nameof(Permission.Add)}";
            public const string Update = $"{PERMISSION_PREFIX}.{nameof(Permission.Update)}";
            public const string Delete = $"{PERMISSION_PREFIX}.{nameof(Permission.Delete)}";
            public const string ReadPagedQuery = $"{PERMISSION_PREFIX}.{nameof(Permission.ReadPagedQuery)}";
            public const string AddCategory = $"{PERMISSION_PREFIX}.{nameof(Permission.AddCategory)}";
            public const string ReadCategories = $"{PERMISSION_PREFIX}.{nameof(Permission.ReadCategories)}";
            public const string ChangeProductItemAmount = $"{PERMISSION_PREFIX}.{nameof(Permission.ChangeProductItemAmount)}";

            public static List<string> AllPermissions => GetModulePermissions<Permission>(Modules.Product);
        }


        #endregion

        #region User permission 
        public sealed class User : Module
        {
            private const string MODULE_NAME = nameof(Modules.User);
            private const string PERMISSION_PREFIX = $"{CLAIM_TYPE}.{MODULE_NAME}";

            private enum Permission
            {
                Add,
                Update,
                Delete,
            }

            public const string Add = $"{PERMISSION_PREFIX}.{nameof(Permission.Add)}";
            public const string Update = $"{PERMISSION_PREFIX}.{nameof(Permission.Update)}";
            public const string Delete = $"{PERMISSION_PREFIX}.{nameof(Permission.Delete)}";

            public static List<string> AllPermissions => GetModulePermissions<Permission>(Modules.User);
        }
        #endregion

        #endregion
    }
}
