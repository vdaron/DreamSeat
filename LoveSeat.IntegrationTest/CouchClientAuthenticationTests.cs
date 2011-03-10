//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MindTouch.Tasking;

//#if NUNIT
//using NUnit.Framework;
//#else
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
//using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
//using TestFixtureSetUpAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute;
//using TestFixtureTearDownAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute;
//#endif

//namespace LoveSeat.IntegrationTest
//{
//    [TestFixture]
//    class CouchClientAuthenticationTests
//    {
//        private static CouchClient client;
//        private static string username;
//        private static string password;

//        [TestFixtureSetUp]
//        [Ignore]
//#if NUNIT
//        public static void Setup()
//#else
//        public static void Setup(TestContext o)
//#endif
//        {
//            username = "adminuser_" + DateTime.Now.ToString("yyyyMMddhhmmss");
//            password = username;

//            client = new CouchClient();
//            client.CreateAdminUser(username, password);
//        }
//        [TestFixtureTearDown]
//        [Ignore]
//        public static void TearDown()
//        {
//            if (client.HasUser(username))
//            {
//                client.DeleteAdminUser(username);
//            }
//            client.RestartServer();
//        }

//        #region Authentication Tests

//        [Test]
//        [Ignore]
//        public void TestCookieAuthentication()
//        {
//            client.Logon(username, password, new Result<bool>()).Wait();
//            Assert.IsTrue(client.IsLogged(new Result<bool>()).Wait());
//            client.Logoff(new Result<bool>()).Wait();
//            Assert.IsFalse(client.IsLogged(new Result<bool>()).Wait());
//        }
//        [Test]
//        [Ignore]
//        public void TestBasicAuthentication()
//        {
//            CouchClient client = new CouchClient(username, password);
//            client.GetConfig();
//        }
//        [Test]
//        [Ignore]
//        public void Should_Delete_Admin_User()
//        {
//            client.DeleteAdminUser(username);
//        }
//        [Test]
//        [Ignore]
//        public void Should_Create_Admin_User()
//        {
//            client.CreateAdminUser(username, password);
//        }
//        [Test]
//        [Ignore]
//        public void Should_Create_User()
//        {
//            CouchDatabase db = client.GetDatabase("_users");
//            CouchUser user = db.GetDocument<CouchUser>("org.couchdb.user:Professor", new Result<CouchUser>()).Wait();
//        }
//        #endregion
//    }
//}
