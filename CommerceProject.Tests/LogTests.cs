using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommerceProject.Business.Helper.Logging;
using NLog;

namespace CommerceProject.Tests
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void LogTest1()
        {
            LogHelper.LogKaydet(LogLevel.Info, "test informational");
            Assert.IsTrue(true);
        }
    }
}
