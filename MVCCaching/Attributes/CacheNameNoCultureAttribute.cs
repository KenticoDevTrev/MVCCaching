using System;

namespace MVCCaching
{
    /// <summary>
    /// When generating the call's Cache Name, do not include the current culture.  Should be used for calls that do not involve culture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheNameNoCultureAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheNameNoCultureAttribute"/> class.
        /// </summary>
        /// <example>
        /// The following example shows how to use the <see cref="CacheNameNoCultureAttribute"/> with the first method argument.
        /// <code>
        /// [CacheNameNoCulture]
        /// public TreeNode GetTreeNode(Guid nodeGUID)
        /// {
        ///     ...
        /// }
        /// </code>
        /// </example>
        public CacheNameNoCultureAttribute()
        {
        }
    }
}