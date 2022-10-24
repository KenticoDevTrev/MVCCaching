using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCCaching
{
    public static class ICacheDependencyKeysBuidlerExtensions
    {
        public static ICacheDependencyKeysBuilder PageTypeDefinition(this ICacheDependencyKeysBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return builder;
            }

            return builder.CustomKey($"{DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE}|byname|{className}");
        }

        public static ICacheDependencyKeysBuilder ObjectType(this ICacheDependencyKeysBuilder builder, string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return builder;
            }

            return builder.CustomKey($"{typeName}|all");
        }

        public static ICacheDependencyKeysBuilder PageOrder(this ICacheDependencyKeysBuilder builder)
        {
            return builder.CustomKey("nodeorder");
        }

        public static ICacheDependencyKeysBuilder SitePageType(this ICacheDependencyKeysBuilder builder, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return builder;
            }

            return builder.CustomKey($"nodes|{builder.SiteName()}|{className}|all");
        }

        public static ICacheDependencyKeysBuilder Pages(this ICacheDependencyKeysBuilder builder, IEnumerable<TreeNode> pages)
        {
            return builder.CustomKeys(pages.Select(p => $"documentid|{p.DocumentID}"));
        }

        public static ICacheDependencyKeysBuilder Object(this ICacheDependencyKeysBuilder builder, string objectType, int? id) => Object(builder, objectType, id.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Object(this ICacheDependencyKeysBuilder builder, string objectType, int id)
        {
            if (string.IsNullOrWhiteSpace(objectType) || id <= 0)
            {
                return builder;
            }

            builder.CustomKey($"{objectType}|byid|{id}");

            return builder;
        }

        public static ICacheDependencyKeysBuilder Object(this ICacheDependencyKeysBuilder builder, string objectType, Guid? guid) => Object(builder, objectType, guid.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Object(this ICacheDependencyKeysBuilder builder, string objectType, Guid guid)
        {
            if (guid == default)
            {
                return builder;
            }

            return builder.CustomKey($"{objectType}|byguid|{guid}");
        }

        public static ICacheDependencyKeysBuilder Object(this ICacheDependencyKeysBuilder builder, string objectType, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return builder;
            }

            return builder.CustomKey($"{objectType}|byname|{name}");
        }

        public static ICacheDependencyKeysBuilder Objects(this ICacheDependencyKeysBuilder builder, IEnumerable<BaseInfo> objects)
        {
            return builder.CustomKeys(objects.Select(o => $"{o.TypeInfo.ObjectType}|byid|{o.Generalized.ObjectID}"));
        }

        public static ICacheDependencyKeysBuilder PagePath(this ICacheDependencyKeysBuilder builder, string path, PathTypeEnum type = PathTypeEnum.Explicit)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return builder;
            }

            switch (type)
            {
                case PathTypeEnum.Single:
                    builder.CustomKey($"node|{builder.SiteName()}|{path}");
                    break;
                case PathTypeEnum.Children:
                    builder.CustomKey($"node|{builder.SiteName()}|{path}|childnodes");
                    break;
                case PathTypeEnum.Section:
                    builder.CustomKey($"node|{builder.SiteName()}|{path}");
                    builder.CustomKey($"node|{builder.SiteName()}|{path}|childnodes");
                    break;
                case PathTypeEnum.Explicit:
                default:
                    if (path.EndsWith("/%"))
                    {
                        builder.CustomKey($"node|{builder.SiteName()}|{path}|childnodes");
                    }
                    else
                    {
                        builder.CustomKey($"node|{builder.SiteName()}|{path}");
                    }
                    break;
            }

            return builder;
        }

        public static ICacheDependencyKeysBuilder Page(this ICacheDependencyKeysBuilder builder, int? documentId) => Page(builder, documentId.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Page(this ICacheDependencyKeysBuilder builder, int documentId)
        {
            if (documentId <= 0)
            {
                return builder;
            }

            return builder.CustomKey($"documentid|{documentId}");
        }

        public static ICacheDependencyKeysBuilder Pages(this ICacheDependencyKeysBuilder builder, IEnumerable<int?> documentIds) => Pages(builder, documentIds.Where(x => x.HasValue).Select(x => x.Value));
        public static ICacheDependencyKeysBuilder Pages(this ICacheDependencyKeysBuilder builder, IEnumerable<int> documentIds)
        {
            return builder.CustomKeys(documentIds.Select(id => $"documentid|{id}"));
        }

        public static ICacheDependencyKeysBuilder Node(this ICacheDependencyKeysBuilder builder, int? nodeId) => Node(builder, nodeId.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Node(this ICacheDependencyKeysBuilder builder, int nodeId)
        {
            if (nodeId <= 0)
            {
                return builder;
            }

            return builder.CustomKey($"nodeid|{nodeId}");
        }

        public static ICacheDependencyKeysBuilder Node(this ICacheDependencyKeysBuilder builder, Guid? nodeGUID) => Node(builder, nodeGUID.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Node(this ICacheDependencyKeysBuilder builder, Guid nodeGUID)
        {
            if (nodeGUID == default)
            {
                return builder;
            }

            return builder.CustomKey($"nodeguid|{builder.SiteName()}|{nodeGUID}");
        }

        public static ICacheDependencyKeysBuilder NodeRelationships(this ICacheDependencyKeysBuilder builder, int? nodeId) => NodeRelationships(builder, nodeId.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder NodeRelationships(this ICacheDependencyKeysBuilder builder, int nodeId)
        {
            if (nodeId <= 0)
            {
                return builder;
            }

            return builder.CustomKey($"nodeid|{nodeId}|relationships");
        }

        public static ICacheDependencyKeysBuilder Page(this ICacheDependencyKeysBuilder builder, Guid? documentGUID) => Page(builder, documentGUID.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Page(this ICacheDependencyKeysBuilder builder, Guid documentGUID)
        {
            if (documentGUID == default)
            {
                return builder;
            }

            return builder.CustomKey($"documentguid|{builder.SiteName()}|{documentGUID}");
        }

        public static ICacheDependencyKeysBuilder Attachment(this ICacheDependencyKeysBuilder builder, Guid? attachmentGUID) => Attachment(builder, attachmentGUID.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Attachment(this ICacheDependencyKeysBuilder builder, Guid attachmentGUID)
        {
            if (attachmentGUID == default)
            {
                return builder;
            }

            return builder.CustomKey($"attachment|{attachmentGUID}");
        }

        public static ICacheDependencyKeysBuilder Media(this ICacheDependencyKeysBuilder builder, Guid? mediaFileGUID) => Media(builder, mediaFileGUID.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder Media(this ICacheDependencyKeysBuilder builder, Guid mediaFileGUID)
        {
            if (mediaFileGUID == default)
            {
                return builder;
            }

            return builder.CustomKey($"mediafile|{mediaFileGUID}");
        }

        public static ICacheDependencyKeysBuilder Collection<T>(this ICacheDependencyKeysBuilder builder, IEnumerable<T> items, Action<T, ICacheDependencyKeysBuilder> action)
        {
            foreach (var item in items.Where(x => x != null))
            {
                action(item, builder);
            }

            return builder;
        }


        public static ICacheDependencyKeysBuilder Collection<T>(this ICacheDependencyKeysBuilder builder, IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items.Where(x => x != null))
            {
                action(item);
            }

            return builder;
        }

        public static ICacheDependencyKeysBuilder CustomTableItems(this ICacheDependencyKeysBuilder builder, string codeName)
        {
            if (string.IsNullOrWhiteSpace(codeName))
            {
                return builder;
            }

            return builder.CustomKey($"customtableitem.{codeName}|all");
        }

        public static ICacheDependencyKeysBuilder CustomTableItem(this ICacheDependencyKeysBuilder builder, string codeName, int? customTableItemId) => CustomTableItem(builder, codeName, customTableItemId.GetValueOrDefault());
        public static ICacheDependencyKeysBuilder CustomTableItem(this ICacheDependencyKeysBuilder builder, string codeName, int customTableItemId)
        {
            if (string.IsNullOrWhiteSpace(codeName) || customTableItemId <= 0)
            {
                return builder;
            }

            return builder.CustomKey($"customtableitem.{codeName}|byid|{customTableItemId}");
        }

        public static ICacheDependencyKeysBuilder ApplyDependenciesTo(this ICacheDependencyKeysBuilder builder, Action<string> action)
        {
            foreach (string cacheKey in builder.GetKeys())
            {
                action(cacheKey);
            }
            return builder;
        }

        public static ICacheDependencyKeysBuilder ApplyAllDependenciesTo(this ICacheDependencyKeysBuilder builder, Action<string[]> action)
        {
            action(builder.GetKeys().ToArray());
            return builder;
        }

        public static CMSCacheDependency GetCMSCacheDependency(this ICacheDependencyKeysBuilder builder)
        {
            return CacheHelper.GetCacheDependency(builder.GetKeys().ToArray());
        }
    }
}
