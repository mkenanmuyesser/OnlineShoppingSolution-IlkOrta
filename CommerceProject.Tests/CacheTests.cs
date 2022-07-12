using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommerceProject.Business.Helper.Caching;

namespace CommerceProject.Tests
{
    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public void CacheTest1()
        {
            CacheHelper.CacheWrite("test", 123);
            object cacheOku = CacheHelper.CacheRead("test");

            Assert.IsNotNull(cacheOku);
        }
    }
}
