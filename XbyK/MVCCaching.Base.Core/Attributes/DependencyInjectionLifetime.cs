namespace MVCCaching
{
    public enum DependencyInjectionLifetime
    {
        /// <summary>
        /// New instance generated each time it's injected.
        /// </summary>
        Transient,
        /// <summary>
        /// New instance generated once per request.
        /// </summary>
        Scoped,
        /// <summary>
        /// One instance generated on application load and share across requests.
        /// </summary>
        Singleton,
    }
}
