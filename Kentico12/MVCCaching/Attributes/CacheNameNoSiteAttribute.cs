using System;

namespace MVCCaching
{
    /// <summary>
    /// When generating the call's Cache Name, do not include the current site name.  Should be used for calls that are for global objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheNameNoSiteAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheNameNoSiteAttribute"/> class.
        /// </summary>
        /// <example>
        /// The following example shows how to use the <see cref="CacheNameNoSiteAttribute"/> with the first method argument.
        /// <code>
        /// [CacheNameNoCulture]
        /// public TreeNode GetTreeNode(Guid nodeGUID)
        /// {
        ///     ...
        /// }
        /// </code>
        /// </example>
        public CacheNameNoSiteAttribute()
        {
        }
    }
}