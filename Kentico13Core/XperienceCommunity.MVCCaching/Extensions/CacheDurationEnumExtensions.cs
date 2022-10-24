using System;

namespace MVCCaching
{
    /// <summary>
    /// Extension methods, this allows you to define an Enum of CacheDurations (with the int value = minutes) and leverage easily in caching.  ex: "CacheDuration.Short.ToDouble()" or "CacheDuration.Max.ToTimeSpan()"
    /// </summary>
    public static class CacheDurationEnumExtensions
    {
        /// <summary>
        /// Takes the given enum's int value and returns as a double
        /// </summary>
        /// <param name="cacheEnum"></param>
        /// <returns></returns>
        public static double ToDouble(this Enum cacheEnum)
        {
            var minutes = Convert.ToInt32(cacheEnum);
            return Convert.ToDouble(minutes);
        }

        /// <summary>
        /// Takes the given enum's int value and returns it as a Timespan (minutes)
        /// </summary>
        /// <param name="cacheEnum"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this Enum cacheEnum)
        {
            return TimeSpan.FromMinutes(cacheEnum.ToDouble());
        }
    }
}
