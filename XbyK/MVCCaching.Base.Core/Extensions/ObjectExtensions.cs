using System.Collections.Generic;
using System.Linq;

namespace MVCCaching
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets a unique Cache Name identifier.  string.Empty if null, ICacheKey.GetCacheKey() if it implements ICacheKey, or it's normal .ToString().  If it's an array, it combines children in a pipe separated list of each object's .ToCacheNameIdentifier()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToCacheNameIdentifier(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else if (obj is ICacheKey)
            {
                return ((ICacheKey)obj).GetCacheKey();
            }
            else if (obj is IEnumerable<object>)
            {
                return string.Join("|", ((IEnumerable<object>)obj).Select(y => y.ToCacheNameIdentifier()));
            }
            else
            {
                return obj.ToString();
            }
        }
    }
}
