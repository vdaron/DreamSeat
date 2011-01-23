using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using LoveSeat;
using MindTouch.Tasking;
using Newtonsoft.Json.Linq;
using System.Text;
using MindTouch.Dream;
using System.Collections.Generic;
using LoveSeat.Interfaces;

#if NUNIT
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestFixtureSetUpAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute;
using TestFixtureTearDownAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute;
#endif

namespace LoveSeat.IntegrationTest
{
	[TestFixture]
	public class CouchClientTest
	{
		private static CouchClient client;
		private const string baseDatabase = "love-seat-test-base";
		private const string replicateDatabase = "love-seat-test-repli";

		private static readonly string host = ConfigurationManager.AppSettings["Host"].ToString();
		private static readonly int port = int.Parse(ConfigurationManager.AppSettings["Port"].ToString());
		private static readonly string username = ConfigurationManager.AppSettings["UserName"].ToString();
		private static readonly string password = ConfigurationManager.AppSettings["Password"].ToString();

		[TestFixtureSetUp]
#if NUNIT
		public static void Setup()
#else
		public static void Setup(TestContext o)
#endif
		{
			client = new CouchClient();
			client.Logon(username, password, new Result<bool>()).Wait();

			if (client.HasDatabase(baseDatabase))
			{
				client.DeleteDatabase(baseDatabase);
			}
			client.CreateDatabase(baseDatabase);

			if (client.HasDatabase(replicateDatabase))
			{
				client.DeleteDatabase(replicateDatabase);
			}
			client.CreateDatabase(replicateDatabase);

			CouchDatabase db = client.GetDatabase(baseDatabase);
			CouchViewDocument view = new CouchViewDocument("testviewitem");
			view.Views.Add("testview", new CouchView("function(doc) {emit(doc._rev, doc)}"));
			db.CreateDocument(view);

		}
		[TestFixtureTearDown]
		public static void TearDown()
		{
			//delete the test database
			if (client.HasDatabase(baseDatabase))
			{
				client.DeleteDatabase(baseDatabase);
			}
			if (client.HasDatabase(replicateDatabase))
			{
				client.DeleteDatabase(replicateDatabase);
			}
			if (client.HasUser("Leela"))
			{
				client.DeleteAdminUser("Leela");
			}
		}

		[Test]
		public void TestCookieAuthentication()
		{
			CouchClient client = new CouchClient();
			client.Logon(username, password, new Result<bool>()).Wait();
			Assert.IsTrue(client.IsLogged(new Result<bool>()).Wait());
			client.Logoff(new Result<bool>()).Wait();
			Assert.IsFalse(client.IsLogged(new Result<bool>()).Wait());
		}

		[Test]
		public void TestBasicAuthentication()
		{
			CouchClient client = new CouchClient(username,password);

			CouchDatabase db = client.GetDatabase(baseDatabase);
			db.CreateDocument(new CouchDocument());
			
		}

		[Test]
		public void Should_Trigger_Replication()
		{
			string dbname = "test-replicate-db-created";
			
			var obj = client.TriggerReplication(new ReplicationOptions(baseDatabase,"http://"+username+":"+password+"@"+ host + ":5984/" + replicateDatabase){ Continuous = true });
			Assert.IsTrue(obj != null);

			var obj2 = client.TriggerReplication(new ReplicationOptions(baseDatabase,dbname){CreateTarget = true});

			Assert.IsTrue(obj2 != null);

			CouchDatabase db = client.GetDatabase(dbname, false);
			Assert.IsNotNull(db);

			client.DeleteDatabase(dbname);
			db = client.GetDatabase(dbname, false);
			Assert.IsNull(db);
		}
		[Test]
		public void Should_Create_Document_From_String()
		{
			string obj = @"{""test"": ""prop""}";
			var db = client.GetDatabase(baseDatabase);
			string id = Guid.NewGuid().ToString("N");
			var result = db.CreateDocument(id, obj, new Result<string>()).Wait();
			Assert.IsNotNull(db.GetDocument<CouchDocument>(id));
		}
		[Test]
		public void Should_Create_Document_From_String_WIthId_GeneratedByCouchDb()
		{
			JDocument obj = new JDocument(@"{""test"": ""prop""}");
			var db = client.GetDatabase(baseDatabase);
			var result = db.CreateDocument(obj);

			Assert.IsNotNull(result.Id);
			Assert.IsNotNull("prop",result["test"].Value<string>());
		}
		[Test]
		public void Should_Save_Existing_Document()
		{
			JDocument obj = new JDocument( @"{""test"": ""prop""}" );
			obj.Id = Guid.NewGuid().ToString("N");

			var db = client.GetDatabase(baseDatabase);
			var result = db.CreateDocument(obj);
			var doc = db.GetDocument<JDocument>(obj.Id);
			doc["test"] = "newprop";
			var newresult = db.SaveDocument(doc);
			Assert.AreEqual(newresult.Value<string>("test"), "newprop");
		}
		[Test]
		public void Should_Delete_Document()
		{
			var db = client.GetDatabase(baseDatabase);
			string id = Guid.NewGuid().ToString("N");
			db.CreateDocument(id, "{}", new Result<string>()).Wait();
			var doc = db.GetDocument<CouchDocument>(id);
			db.DeleteDocument(doc);
			Assert.IsNull(db.GetDocument<CouchDocument>(id));
		}
		[Test]
		public void Should_Determine_If_Doc_Has_Attachment()
		{
			var db = client.GetDatabase(baseDatabase);
			string Id = Guid.NewGuid().ToString("N");

			db.CreateDocument(Id, "{}", new Result<string>()).Wait();
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a text document")))
			{
				db.AddAttachment(Id, ms, "martin.txt");
			}
			var doc2 = db.GetDocument<CouchDocument>(Id);
			Assert.IsTrue(doc2.HasAttachment);
		}
		[Test]
		public void Should_Return_Attachment_Names()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument(@"{""_id"":""upload""}", new Result<string>()).Wait();
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a text document")))
			{
				db.AddAttachment("upload", ms, "martin.txt");
			}
			var bdoc = db.GetDocument<CouchDocument>("upload");
			Assert.IsTrue(bdoc.GetAttachmentNames().Contains("martin.txt"));
		}

		[Test]
		public void Should_Create_And_Read_ConfigValue()
		{
			client.SetConfigValue("coucou", "key", "value");
			Assert.AreEqual("value",client.GetConfigValue("coucou","key"));
			client.DeleteConfigValue("coucou", "key");
			Assert.IsNull(client.GetConfigValue("coucou", "key"));
		}
		[Test]
		public void Should_Read_ConfigSection()
		{
			client.SetConfigValue("coucou", "key", "value");
			Dictionary<string, string> section = client.GetConfigSection("coucou", new Result<Dictionary<string, string>>()).Wait();
			Assert.AreEqual(1, section.Count);
			Assert.IsTrue(section.ContainsKey("key"));
			Assert.AreEqual("value", section["key"]);
		}
		[Test]
		public void Should_Read_Configs()
		{
			Dictionary<string, Dictionary<string, string>> config = client.GetConfig(new Result<Dictionary<string, Dictionary<string, string>>>()).Wait();
			Assert.IsTrue(config.Count > 0);
		}

		[Test]
		[Ignore]
		public void Should_Delete_Admin_User()
		{
			client.DeleteAdminUser("Leela");
		}
		[Test]
		[Ignore]
		public void Should_Create_Admin_User()
		{
			client.CreateAdminUser("Leela", "Turanga");
		}

		[Test]
		public void Should_Get_Attachment()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument(@"{""_id"":""test_upload""}", new Result<string>()).Wait();
			var doc = db.GetDocument<CouchDocument>("test_upload");
			var attachment = Encoding.UTF8.GetBytes("test");
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a text document")))
			{
				db.AddAttachment("test_upload", ms, "test_upload.txt");
			}
			using(Stream stream = db.GetAttachment(doc, "test_upload.txt"))
			using(StreamReader sr = new StreamReader(stream))
			{
				string result = sr.ReadToEnd();
				Assert.IsTrue(result == "This is a text document");
			}
		}
		[Test]
		public void Should_Delete_Attachment()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument(@"{""_id"":""test_delete""}", new Result<string>()).Wait();
			db.GetDocument<CouchDocument>("test_delete");

			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a text document")))
			{
				db.AddAttachment("test_delete", ms, "test_upload.txt");
			}
			db.DeleteAttachment("test_delete", "test_upload.txt");
			var retrieved = db.GetDocument<CouchDocument>("test_delete");
			Assert.IsFalse(retrieved.HasAttachment);
		}
		[Test]
		public void Should_Return_Etag_In_ViewResults()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument(@"{""_id"":""test_eTag""}", new Result<string>()).Wait();
			ViewResult<JObject> result = db.GetAllDocuments(new Result<ViewResult<JObject>>()).Wait();
			Assert.IsTrue(!string.IsNullOrEmpty(result.ETag));
		}
		[Test]
		public void Should_Get_304_If_ETag_Matches()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument(@"{""_id"":""test_eTag_exception""}", new Result<string>()).Wait();
			ViewResult<JObject> result = db.GetAllDocuments(new Result<ViewResult<JObject>>()).Wait();
			ViewResult<JObject> cachedResult = db.GetAllDocuments(new ViewOptions { Etag = result.ETag }, new Result<ViewResult<JObject>>()).Wait();
			Assert.AreEqual(DreamStatus.NotModified, cachedResult.Status);
		}
		[Test]
		[Ignore]
		public void Should_Get_Results_Quickly()
		{
			var db = client.GetDatabase("accounting");
			var startTime = DateTime.Now;
			var options = new ViewOptions { Limit = 20 };
			//var result = db.View<Company>("companies_by_name", options, "accounting",new Result<ViewResult<Company>>()).Wait();
			//foreach (var item in result.Items)
			//{
			//    Console.WriteLine(item.Name);
			//}
			//var endTime = DateTime.Now;
			//double delay = (endTime - startTime).TotalMilliseconds;
			//Assert.IsTrue(delay < 80);
		}
		[Test]
		public void Should_Get_Database_Info()
		{
			var db = client.GetDatabase(baseDatabase);

			CouchDatabaseInfo info = db.GetInfo();
			Assert.IsNotNull(info);
			Assert.AreEqual(baseDatabase, info.Name);
		}

		[Test]
		public void Should_Return_View_Results()
		{
			CouchDatabase db = client.GetDatabase(baseDatabase);

			db.CreateDocument(new JDocument() { Id = "test_doc" });
			db.CreateDocument(new JDocument() { Id = "test_doc2" });
			db.CreateDocument(new JDocument() { Id = "test_doc3" });

			ViewResult<JObject> result = db.GetView<JObject>("testviewitem", "testview", new Result<ViewResult<JObject>>()).Wait();
			Assert.IsNotNull(result);
			Assert.IsTrue(result.TotalRows > 0);
		}

		[Test]
		[Ignore]
		public void Should_Create_User()
		{
			CouchDatabase db = client.GetDatabase("_users");
			CouchUser user = db.GetDocument<CouchUser>("org.couchdb.user:Professor", new Result<CouchUser>()).Wait();
		}

		[Test]
		public void CreateViewDocument()
		{
			var db = client.GetDatabase(baseDatabase);
			CouchViewDocument doc = new CouchViewDocument("firstviewdoc");
			doc.Views.Add("all", new CouchView("function(doc) { emit(null, doc) }"));

			db.CreateDocument(doc);

			Assert.IsNotNull(doc.Rev);
		}
		[Test]
		public void Should_Start_Compact()
		{
			var db = client.GetDatabase(baseDatabase);
			db.Compact();
		}
		[Test]
		public void Should_Start_CompactView()
		{
			var db = client.GetDatabase(baseDatabase);
			CouchViewDocument doc = new CouchViewDocument("test_compactview");
			doc.Views.Add("test", new CouchView("function(doc) { emit(null, doc) }"));
			db.CreateDocument(doc);

			db.CompactDocumentView("test_compactview");
		}
		[Test]
		public void Should_Run_TestView()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument("id1", "{}", new Result<string>()).Wait();
			CouchViewDocument doc = new CouchViewDocument("test_compactview");
			ViewResult<JObject> result = db.GetTempView<JObject>(new CouchView("function(doc) { emit(null, doc) }"));

			Assert.IsNotNull(result);
			Assert.AreEqual(DreamStatus.Ok, result.Status);
		}
		[Test]
		public void Should_Restart_Server()
		{
			client.RestartServer();
		}
	}
	public class Company
	{
		public string Name { get; set; }
	}
}
