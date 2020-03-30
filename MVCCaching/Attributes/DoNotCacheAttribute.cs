using System;

namespace MVCCaching
{
    /// <summary>
    /// Tells the built in Repository Caching to not use automatic caching.  You can implement your own caching instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DoNotCacheAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DoNotCacheAttribute"/> class.
        /// </summary>
        /// <example>
        /// The following example shows how to use the <see cref="DoNotCacheAttribute"/> with the first method argument.
        /// <code>
        /// [DoNotCache]
        /// public TreeNode GetTreeNode(Guid nodeGUID)
        /// {
        ///     ...
        /// }
        /// </code>
        /// </example>
        public DoNotCacheAttribute()
        {
        }
    }
}