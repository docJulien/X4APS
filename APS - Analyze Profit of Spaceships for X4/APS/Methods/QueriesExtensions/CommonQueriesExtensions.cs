using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace APS.Methods.QueriesExtensions
{

    public static class ExtensionClass
    {
        /// <summary>
        /// Maps the source object to target object.
        /// </summary>
        /// <typeparam name="T">Type of target object.</typeparam>
        /// <typeparam name="TU">Type of source object.</typeparam>
        /// <param name="target">Target object.</param>
        /// <param name="source">Source object.</param>
        /// <returns>Updated target object.</returns>
        public static T Map<T, TU>(this T target, TU source)
        {
            // get property list of the target object.

            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                // check whether source object has the the property
                var sp = source.GetType().GetProperty(prop.Name);
                var tp = target.GetType().GetProperty(prop.Name);
                if (sp != null)
                {
                    // if yes, copy the value to the matching property
                    if (tp.CanWrite)
                    {
                        if (tp.PropertyType == sp.PropertyType)
                        {
                            var value = sp.GetValue(source, null);
                            tp.SetValue(target, value, null);
                        }
                    }
                }
            }

            return target;
        }
    }

    public static class CommonQueriesExtensions
    {
        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> query) where T : class
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.GetGetMethod().IsVirtual)
                {
                    query = query.Include(property.Name);
                }
            }

            return query;
        }
    }
}