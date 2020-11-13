using System;

namespace MVCCaching
{
    /// <summary>
    /// Allows you to specify the duration an item should be cached for.  If not provided it will use the default cache duration (App Settings Key or Kentico Sttings).  If provided and no CacheDependency specified, will cache without dependency keys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CacheDurationAttribute : Attribute
    {
        public readonly int DurationMinutes;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDurationAttribute"/> class.
        /// </summary>
        /// <example>
        /// The following example shows how to use the <see cref="CacheDurationAttribute"/>.
        /// <code>
        /// [CacheDependency("nodeguid|##SITENAME##|{0}")]
        /// [CacheDuration(60)]
        /// public TreeNode GetTreeNode(Guid nodeGUID)
        /// {
        ///     ...
        /// }
        /// </code>
        /// </example>
        /// <param name="DurationMinutes">The number of minutes this item should be cached for. 0 = do not cache</param>
        /// <exception cref="ArgumentException">Thrown when the minutes is less than 0.</exception>
        public CacheDurationAttribute(int DurationMinutes)
        {
            if(DurationMinutes < 0)
            {
                throw new ArgumentException("Must be greater than or equal to 0");
            }
            this.DurationMinutes = DurationMinutes;
        }
    }
}