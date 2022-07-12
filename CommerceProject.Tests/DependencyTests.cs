using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommerceProject.Business.Helper.Logging;
using NLog;
using Microsoft.Practices.Unity;
using CommerceProject.Business.BusinessContracts;
using CommerceProject.Business.BusinessServices;
using System.Web.Mvc;
using System.Collections.Generic;
using System;
using CommerceProject.Business.Entities;
using System.Linq;
using Unity;

namespace CommerceProject.Tests
{
    [TestClass]
    public class DependencyTests
    {
        [TestMethod]
        public void DependencyTest1()
        {
            IUnityContainer myContainer = new UnityContainer();
            myContainer.RegisterType<IKisaLinkService, KisaLinkService>();

            MyClass classUnderTest = new MyClass(myContainer);
            //classUnderTest.GenerateLink();
            var result = classUnderTest.AddVisitor();
            Assert.IsTrue(result);
        }

        class MyClass
        {
            IUnityContainer unityContainer;
            public MyClass(IUnityContainer container)
            {
                this.unityContainer = container;
            }

            public string GenerateLink()
            {
                var service = unityContainer.Resolve<IKisaLinkService>();
                var data = service.GenerateShortLink();
                return data;
            }

            public bool AddVisitor()
            {
                var service = unityContainer.Resolve<IKisaLinkService>();
                var data = service.AddVisitor("http://www.ilkorta.com/5xkLwmh5");
                return data;
            }
        }
    }
}