using MVCCaching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace XperienceCommunity.MVCCaching.Implementations
{
    public class CacheDependenciesStoreAndScope : ICacheDependenciesStore, ICacheDependenciesScope
    {
        public CacheDependenciesStoreAndScope()
        {

        }

        private readonly ConcurrentStack<HashSet<string>> keyScopes = new ConcurrentStack<HashSet<string>>();

        public void Begin() => keyScopes.Push(new HashSet<string>(StringComparer.OrdinalIgnoreCase));

        public void Store(string[] keys)
        {
            if (!keyScopes.TryPeek(out var currentScope))
            {
                return;
            }

            foreach (string key in keys)
            {
                currentScope.Add(key.ToLower());
            }
        }

        public string[] End()
        {
            if (!keyScopes.TryPop(out var currentScope))
            {
                return new string[] { };
            }

            if (!keyScopes.TryPeek(out var parentScope))
            {
                return [.. currentScope];
            }

            foreach (string key in currentScope)
            {
                parentScope.Add(key);
            }
            return [.. currentScope];
        }


    }
}
