using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Websites;
using System.Collections.Generic;

namespace MVCCaching
{
    /// <summary>
    /// Simple DTO Wrapper to include additional dependencies that should be passed outside of the cached method and added to the general scope.  
    /// This is particularly useful when retrieving Linked Cache Dependencies since you need to add it to the IProgressiveCache's CacheBuilder, as well as pass it outside to the CacheDependencyBuilder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Result">The Result you want to return from your cached method</param>
    /// <param name="AdditionalDependencies">Additional Dependencies</param>
    public record DTOWithDependencies<T>
    {
        public DTOWithDependencies(T result) {
            Result = result;
        }
        public DTOWithDependencies(T result, List<string> additionalDependencies)
        {
            Result = result;
            AdditionalDependencies = additionalDependencies;
        }
        public DTOWithDependencies(T result, List<IContentItemFieldsSource> contentItems)
        {
            Result = result;
            ContentItems = contentItems;
        }
        public DTOWithDependencies(T result, List<IWebPageFieldsSource> webPageItems)
        {
            Result = result;
            WebPageItems = webPageItems;
        }
        public DTOWithDependencies(T result, List<BaseInfo> objects)
        {
            Result = result;
            Objects = objects;
        }

        public T Result { get; }
        public List<IWebPageFieldsSource> WebPageItems { get; set; } = [];
        public List<IContentItemFieldsSource> ContentItems { get; set; } = [];
        public List<string> AdditionalDependencies { get; set; } = [];
        public List<BaseInfo> Objects { get; set; } = [];
    }
}
