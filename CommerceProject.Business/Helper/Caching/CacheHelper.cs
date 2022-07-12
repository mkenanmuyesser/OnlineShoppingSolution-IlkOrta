using CacheManager.Core;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using CommerceProject.Business.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommerceProject.Business.Helper.Caching
{
    public sealed class CacheHelper
    {
        private static ICacheManager<object> _cacheManager = null;
        private static IIcerikAyarService IcerikAyarService = new IcerikAyarService();

        static CacheHelper()
        {
            var icerikAyar = IcerikAyarService.GetFirst();

            _cacheManager = CacheFactory.Build(settings => settings.
            WithUpdateMode(CacheUpdateMode.Up).
            WithSystemRuntimeCacheHandle().
            WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(icerikAyar.ClearCacheTime)));
        }

        public static void CacheWrite(string cacheName, object value)
        {
            _cacheManager.Add(cacheName, value);
        }

        public static object CacheRead(string cacheName)
        {
            return _cacheManager.Get(cacheName);
        }

        public static void ClearCache(CacheDataObj cacheDataObject)
        {
            _cacheManager.Remove(cacheDataObject.ToString());
        }

        public static void ClearAll(CacheDataObj cacheDataObject)
        {
            _cacheManager.Clear();
        }
    }
}