using System.Data.Common;
using System.Reflection;

namespace Common.ORM.Data
{
    public static class DataReaderExtensions
    {
        #region Map Stored Procedure ToList

        public static List<T>? MapDbDataReaderToList<T>(this DbDataReader dr) where T : new()
        {
            if (dr != null && dr.HasRows)
            {
                var entity = typeof(T);
                var entities = new List<T>();
                var props = entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var propDict = props.ToDictionary(p => p.Name.ToUpper(), p => p);

                while (dr.Read())
                {
                    T newObject = new();

                    for (int index = 0; index < dr.FieldCount; index++)
                        if (propDict.ContainsKey(dr.GetName(index).ToUpper()))
                        {
                            var info = propDict[dr.GetName(index).ToUpper()];
                            if (info != null && info.CanWrite)
                            {
                                var val = dr.GetValue(index);
                                info.SetValue(newObject, val == DBNull.Value ? null : val, null);
                            }
                        }

                    entities.Add(newObject);
                }

                return entities;
            }

            return null;
        }

        #endregion
    }
}
