using System;
namespace MVCCaching
{
    /// <summary>
    /// Exception thrown when a Cache Dependency is being generated but it's missing the data (IE the Data query forgot to include the fields needed to generate the keys or some context is missing).
    /// </summary>
    internal class CacheDependencyMissingDataException : Exception
    {
        public CacheDependencyMissingDataException()
        {
        }

        public CacheDependencyMissingDataException(string message) : base(message)
        {
        }

        public CacheDependencyMissingDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
