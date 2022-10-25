using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCCaching
{
    public static class ICacheDependencyBuilderExtensions
    {
        public static ICacheDependencyBuilder PageTypeDefinition(this ICacheDependencyBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return builder;
            }

            return builder.AddKey($"{DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE}|byname|{className}");
        }

        public static ICacheDependencyBuilder ObjectType(this ICacheDependencyBuilder builder, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return builder;
            }

            return builder.AddKey($"{typeName}|all");
        }

        public static ICacheDependencyBuilder PageOrder(this ICacheDependencyBuilder builder)
        {
            return builder.AddKey("nodeorder");
        }

        public static ICacheDependencyBuilder SitePageType(this ICacheDependencyBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return builder;
            }

            return builder.AddKey($"nodes|{builder.SiteName()}|{className}|all");
        }

        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<TreeNode> pages)
        {
            return builder.AddKeys(pages.Select(p => $"documentid|{p.DocumentID}"));
        }

        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, int? id) => Object(builder, objectType, id.GetValueOrDefault());
        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, int id)
        {
            if (string.IsNullOrWhiteSpace(objectType) || id <= 0)
            {
                return builder;
            }

            builder.AddKey($"{objectType}|byid|{id}");

            return builder;
        }

        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, Guid? guid) => Object(builder, objectType, guid.GetValueOrDefault());
        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, Guid guid)
        {
            if (guid == default)
            {
                return builder;
            }

            return builder.AddKey($"{objectType}|byguid|{guid}");
        }

        public static ICacheDependencyBuilder Object(this ICacheDependencyBuilder builder, string objectType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return builder;
            }

            return builder.AddKey($"{objectType}|byname|{name}");
        }

        public static ICacheDependencyBuilder Objects(this ICacheDependencyBuilder builder, IEnumerable<BaseInfo> objects)
        {
            return builder.AddKeys(objects.Select(o => $"{o.TypeInfo.ObjectType}|byid|{o.Generalized.ObjectID}"));
        }

        public static ICacheDependencyBuilder PagePath(this ICacheDependencyBuilder builder, string path, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return builder;
            }

            switch (type)
            {
                case PathTypeEnum.Single:
                    builder.AddKey($"node|{builder.SiteName()}|{path}");
                    break;
                case PathTypeEnum.Children:
                    builder.AddKey($"node|{builder.SiteName()}|{path}|childnodes");
                    break;
                case PathTypeEnum.Section:
                    builder.AddKey($"node|{builder.SiteName()}|{path}");
                    builder.AddKey($"node|{builder.SiteName()}|{path}|childnodes");
                    break;
                case PathTypeEnum.Explicit:
                default:
                    if (path.EndsWith("/%"))
                    {
                        builder.AddKey($"node|{builder.SiteName()}|{path}|childnodes");
                    }
                    else
                    {
                        builder.AddKey($"node|{builder.SiteName()}|{path}");
                    }
                    break;
            }

            return builder;
        }

        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, int? documentId) => Page(builder, documentId.GetValueOrDefault());
        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, int documentId)
        {
            if (documentId <= 0)
            {
                return builder;
            }

            return builder.AddKey($"documentid|{documentId}");
        }

        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<int?> documentIds) => Pages(builder, documentIds.Where(x => x.HasValue).Select(x => x.Value));
        public static ICacheDependencyBuilder Pages(this ICacheDependencyBuilder builder, IEnumerable<int> documentIds)
        {
            return builder.AddKeys(documentIds.Select(id => $"documentid|{id}"));
        }

        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, int? nodeId) => Node(builder, nodeId.GetValueOrDefault());
        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, int nodeId)
        {
            if (nodeId <= 0)
            {
                return builder;
            }

            return builder.AddKey($"nodeid|{nodeId}");
        }

        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, Guid? nodeGUID) => Node(builder, nodeGUID.GetValueOrDefault());
        public static ICacheDependencyBuilder Node(this ICacheDependencyBuilder builder, Guid nodeGUID)
        {
            if (nodeGUID == default)
            {
                return builder;
            }

            return builder.AddKey($"nodeguid|{builder.SiteName()}|{nodeGUID}");
        }

        public static ICacheDependencyBuilder NodeRelationships(this ICacheDependencyBuilder builder, int? nodeId) => NodeRelationships(builder, nodeId.GetValueOrDefault());
        public static ICacheDependencyBuilder NodeRelationships(this ICacheDependencyBuilder builder, int nodeId)
        {
            if (nodeId <= 0)
            {
                return builder;
            }

            return builder.AddKey($"nodeid|{nodeId}|relationships");
        }

        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, Guid? documentGUID) => Page(builder, documentGUID.GetValueOrDefault());
        public static ICacheDependencyBuilder Page(this ICacheDependencyBuilder builder, Guid documentGUID)
        {
            if (documentGUID == default)
            {
                return builder;
            }

            return builder.AddKey($"documentguid|{builder.SiteName()}|{documentGUID}");
        }

        public static ICacheDependencyBuilder Attachment(this ICacheDependencyBuilder builder, Guid? attachmentGUID) => Attachment(builder, attachmentGUID.GetValueOrDefault());
        public static ICacheDependencyBuilder Attachment(this ICacheDependencyBuilder builder, Guid attachmentGUID)
        {
            if (attachmentGUID == default)
            {
                return builder;
            }

            return builder.AddKey($"attachment|{attachmentGUID}");
        }

        public static ICacheDependencyBuilder Media(this ICacheDependencyBuilder builder, Guid? mediaFileGUID) => Media(builder, mediaFileGUID.GetValueOrDefault());
        public static ICacheDependencyBuilder Media(this ICacheDependencyBuilder builder, Guid mediaFileGUID)
        {
            if (mediaFileGUID == default)
            {
                return builder;
            }

            return builder.AddKey($"mediafile|{mediaFileGUID}");
        }

        public static ICacheDependencyBuilder Collection<T>(this ICacheDependencyBuilder builder, IEnumerable<T> items, Action<T, ICacheDependencyBuilder> action)
        {
            foreach (var item in items.Where(x => x != null))
            {
                action(item, builder);
            }

            return builder;
        }


        public static ICacheDependencyBuilder Collection<T>(this ICacheDependencyBuilder builder, IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items.Where(x => x != null))
            {
                action(item);
            }

            return builder;
        }

        public static ICacheDependencyBuilder CustomTableItems(this ICacheDependencyBuilder builder, string codeName)
        {
            if (string.IsNullOrWhiteSpace(codeName))
            {
                return builder;
            }

            return builder.AddKey($"customtableitem.{codeName}|all");
        }

        public static ICacheDependencyBuilder CustomTableItem(this ICacheDependencyBuilder builder, string codeName, int? customTableItemId) => CustomTableItem(builder, codeName, customTableItemId.GetValueOrDefault());
        public static ICacheDependencyBuilder CustomTableItem(this ICacheDependencyBuilder builder, string codeName, int customTableItemId)
        {
            if (string.IsNullOrWhiteSpace(codeName) || customTableItemId <= 0)
            {
                return builder;
            }

            return builder.AddKey($"customtableitem.{codeName}|byid|{customTableItemId}");
        }

        public static ICacheDependencyBuilder ApplyDependenciesTo(this ICacheDependencyBuilder builder, Action<string> action)
        {
            foreach (string cacheKey in builder.GetKeys())
            {
                action(cacheKey);
            }
            return builder;
        }

        public static ICacheDependencyBuilder ApplyAllDependenciesTo(this ICacheDependencyBuilder builder, Action<string[]> action)
        {
            action(builder.GetKeys().ToArray());
            return builder;
        }

        public static CMSCacheDependency GetCMSCacheDependency(this ICacheDependencyBuilder builder)
        {
            return CacheHelper.GetCacheDependency(builder.GetKeys().ToArray());
        }
    }
}
