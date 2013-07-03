﻿using System;
using System.IO;
using System.Linq;
using MindTouch.Tasking;
using System.Text;
using MindTouch.Dream;
using System.Collections.Generic;
using DreamSeat.Interfaces;
using DreamSeat.Support;
using Newtonsoft.Json.Linq;
#if NUNIT
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestFixtureSetUpAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute;
using TestFixtureTearDownAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute;
#endif

namespace DreamSeat.IntegrationTest
{
	[TestFixture]
	public class CouchClientTest
	{
		public class TestSubClass : CouchDocument
		{
			public string TESTVAL { get; set; }
		}

		private static CouchClient client;
		private const string baseDatabase = "dream-seat-test-base";
		private const string replicateDatabase = "dream-seat-test-repli";
		private const string couchdbHostName = "localhost";

		[TestFixtureSetUp]
#if NUNIT
		public static void Setup()
#else
		public static void Setup(TestContext o)
#endif
		{
			client = new CouchClient(aHost: couchdbHostName);

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
			CouchDesignDocument view = new CouchDesignDocument("testviewitem");
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
		public void Should_Trigger_Replication()
		{
			string dbname = "test-replicate-db-created";

			var obj = client.TriggerReplication(new ReplicationOptions(baseDatabase, String.Format("http://{0}:5984/{1}", couchdbHostName, replicateDatabase)) { Continuous = true });
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
		public void GetDatabases()
		{
			IEnumerable<string> database = client.GetAllDatabases();
			Assert.IsNotNull(database);
			Assert.IsTrue(database.Contains(baseDatabase));
			Assert.IsTrue(database.Contains(replicateDatabase));
		}
		[Test]
		public void GetDatabaseInfo()
		{
			CouchDatabase baseDb = client.GetDatabase(baseDatabase);
			CouchDatabaseInfo couchDatabaseInfo = baseDb.GetInfo();

			Assert.AreEqual(baseDatabase, couchDatabaseInfo.Name);
			Assert.AreEqual(false, couchDatabaseInfo.CompactRunning);
			Assert.AreNotEqual(0, couchDatabaseInfo.DiskFormatVersion);
			Assert.AreNotEqual(0, couchDatabaseInfo.DiskSize);
			Assert.AreNotEqual(0, couchDatabaseInfo.DocCount);
			Assert.AreNotEqual(0, couchDatabaseInfo.DocDeletedCount);
			Assert.AreNotEqual(0, couchDatabaseInfo.InstanceStartTimeMs);
			Assert.AreNotEqual(DateTime.MinValue, couchDatabaseInfo.InstanceStartTime);
		}
		[Test]
		public void Should_Trigger_Replication_using_replicator_db()
		{
			CouchDatabase replDb = client.GetDatabase("_replicator");

			ICouchDocument existingDoc = replDb.GetDocument<JDocument>("C");
			if (existingDoc != null)
				replDb.DeleteDocument(existingDoc);

			CouchReplicationDocument doc = replDb.CreateDocument(
				new CouchReplicationDocument
					{
						Id = "C", 
						Source = baseDatabase, 
						Target = String.Format("http://{0}:5984/{1}",couchdbHostName,replicateDatabase),
						Continuous = true,
						UserContext = new UserContext { Name = "bob", Roles = new[] { "role1", "role2" } }
					});

			//Sleep two second to ensure the replicationid is set by couchdb
			System.Threading.Thread.Sleep(2000);

			CouchReplicationDocument doc2 = replDb.GetDocument<CouchReplicationDocument>("C");
			Assert.IsNotEmpty(doc2.ReplicationId);
			Assert.IsNotEmpty(doc2.ReplicationState);
			Assert.IsTrue(doc2.Continuous);
			Assert.IsTrue(doc2.ReplicationStateTime.HasValue);
			Assert.IsNotNull(doc2.UserContext);
			Assert.AreEqual("bob",doc2.UserContext.Name);
			Assert.AreEqual(2, doc2.UserContext.Roles.Length);
			Assert.AreEqual("role1", doc2.UserContext.Roles[0]);
			Assert.AreEqual("role2", doc2.UserContext.Roles[1]);

			replDb.DeleteDocument(doc2);
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
			Assert.AreEqual("prop", result.Value<string>("test"));
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
			var newresult = db.UpdateDocument(doc);
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
		public void ShouldCreateAttachmentInSubCouchDocumentClass()
		{
			var db = client.GetDatabase(baseDatabase);

			TestSubClass tsc = new TestSubClass {TESTVAL = "Hello"};

			tsc = db.CreateDocument(tsc, new Result<TestSubClass>()).Wait();

			var attachment = Encoding.UTF8.GetBytes("test");
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("This is a text document")))
			{
				db.AddAttachment(tsc.Id, ms, "test_upload.txt");
			}

			tsc = db.GetDocument(tsc.Id, new Result<TestSubClass>()).Wait();

			Assert.IsTrue(tsc.HasAttachment);
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
			ViewResult<string, JObject> result = db.GetAllDocuments(new Result<ViewResult<string, JObject>>()).Wait();
			Assert.IsTrue(!string.IsNullOrEmpty(result.ETag));
		}
		[Test]
		public void Should_Get_304_If_ETag_Matches()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument(@"{""_id"":""test_eTag_exception""}", new Result<string>()).Wait();
			ViewResult<string, JObject> result = db.GetAllDocuments(new Result<ViewResult<string, JObject>>()).Wait();
			ViewResult<string, JObject> cachedResult = db.GetAllDocuments(new ViewOptions { Etag = result.ETag }, new Result<ViewResult<string, JObject>>()).Wait();
			Assert.AreEqual(DreamStatus.NotModified, cachedResult.Status);
		}
		[Test]
		public void Should_Get_Results_Asychronously()
		{
			string obj = @"{""test"": ""prop""}";
			CouchDatabase db = client.GetDatabase(baseDatabase);
			db.CreateDocument("TEST", obj, new Result<string>()).Wait();

			string val1 = null;
			string val2 = null;

			Result<string> res1 = new Result<string>();
			Result<string> res2 = new Result<string>();
			db.GetDocument("TEST", res1);
			db.GetDocument("TEST", res2);

			res2.Wait();
			res1.Wait();

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
			db.CreateDocument(new JDocument());
			db.CreateDocument(new JDocument());
			db.CreateDocument(new JDocument());

			ViewResult<string, JObject> result = db.GetView("testviewitem", "testview", new Result<ViewResult<string, JObject>>()).Wait();
			Assert.IsNotNull(result);
			Assert.IsTrue(result.TotalRows > 0);
		}
		[Test]
		public void Should_Return_View_Results_With_Documents()
		{
			CouchDatabase db = client.GetDatabase(baseDatabase);
			db.CreateDocument(new JDocument());
			db.CreateDocument(new JDocument());
			db.CreateDocument(new JDocument());

			ViewResult<string, JObject, JDocument> result = db.GetView<string, JObject, JDocument>("testviewitem", "testview");
			Assert.IsNotNull(result);
			foreach (ViewResultRow<string, JObject, JDocument> row in result.Rows)
			{
				Assert.IsNotNull(row.Doc);
				Assert.IsNotNull(row.Key);
				Assert.IsNotNull(row.Id);
				Assert.IsNotNull(row.Value);
			}
		}
		[Test]
		public void Should_Return_View_Results_As_JObject()
		{
			CouchDatabase db = client.GetDatabase(baseDatabase);
			db.CreateDocument(new JDocument());
			db.CreateDocument(new JDocument());
			db.CreateDocument(new JDocument());

			JObject result = db.GetView("testviewitem", "testview", new Result<JObject>()).Wait();
			Assert.IsNotNull(result);
			Assert.IsNotNull(result["rows"]);
		}
		[Test]
		public void CreateViewDocument()
		{
			var db = client.GetDatabase(baseDatabase);
			CouchDesignDocument doc = new CouchDesignDocument("firstviewdoc");
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
			CouchDesignDocument doc = new CouchDesignDocument("test_compactview");
			doc.Views.Add("test", new CouchView("function(doc) { emit(null, doc) }"));
			db.CreateDocument(doc);

			db.CompactDocumentView("test_compactview");
		}
		[Test]
		public void Should_Run_TestView()
		{
			var db = client.GetDatabase(baseDatabase);
			db.CreateDocument("id1", "{}", new Result<string>()).Wait();
			CouchDesignDocument doc = new CouchDesignDocument("test_compactview");
			ViewResult<string, JObject> result = db.GetTempView<string, JObject>(new CouchView("function(doc) { emit(null, doc) }"));

			Assert.IsNotNull(result);
			Assert.AreEqual(DreamStatus.Ok, result.Status);
		}
		[Test]
		[Ignore]
		public void Should_Restart_Server()
		{
			client.RestartServer();
		}
		[Test]
		public void GetChangesWithDocument()
		{
			if (client.HasDatabase("test_changes"))
				client.DeleteDatabase("test_changes");

			var db = client.GetDatabase("test_changes");
			db.CreateDocument(null, "{}", new Result<string>()).Wait();

			CouchChanges<JDocument> changes = db.GetChanges(new ChangeOptions(), new Result<CouchChanges<JDocument>>()).Wait();
			Assert.AreEqual(1,changes.Results.Length);
			Assert.IsNotNull(changes.Results[0].Doc);
			Assert.IsNotNull(changes.Results[0].Changes);
			Assert.IsNotNull(changes.Results[0].Id);
			Assert.IsNotNull(changes.Results[0].Sequence);
		}
		[Test]
		public void GetChanges()
		{
			if (client.HasDatabase("test_changes"))
				client.DeleteDatabase("test_changes");

			var db = client.GetDatabase("test_changes");
			db.CreateDocument(null, "{}", new Result<string>()).Wait();

			CouchChanges changes = db.GetChanges(new ChangeOptions(), new Result<CouchChanges>()).Wait();
			Assert.AreEqual(1, changes.Results.Length);
			Assert.IsNotNull(changes.Results[0].Changes);
			Assert.IsNotNull(changes.Results[0].Id);
			Assert.IsNotNull(changes.Results[0].Sequence);
		}
        [Test]
        public void GetChangesByType()
        {
            if (client.HasDatabase("test_changes"))
                client.DeleteDatabase("test_changes");

            var db = client.GetDatabase("test_changes");
            CouchDesignDocument doc = new CouchDesignDocument("showdoc");
            doc.Filters.Add("bytype", "function(doc, req) { if(doc.type == req.query.type) { return true; } else { return false; }}");
            db.CreateDocument(doc);

            db.CreateDocument(null, "{type:1}", new Result<string>()).Wait();
            db.CreateDocument(null, "{type:2}", new Result<string>()).Wait();

            CouchChanges changes = db.GetChanges(new ChangeOptions()
            {
                Filter = @"showdoc/bytype",
                AdditionalParams = new Dictionary<string, string>() { { "type", "1" } }
            }, new Result<CouchChanges>()).Wait();
            Assert.AreEqual(1, changes.Results.Length);
            Assert.IsNotNull(changes.Results[0].Changes);
            Assert.IsNotNull(changes.Results[0].Id);
            Assert.IsNotNull(changes.Results[0].Sequence);
        }
		[Test]
		public void GetContinuousChanges()
		{
			System.Threading.AutoResetEvent evt = new System.Threading.AutoResetEvent(false);
			if (client.HasDatabase("test_changes"))
				client.DeleteDatabase("test_changes");

			string id = null;
			var db = client.GetDatabase("test_changes");
			using (CouchContinuousChanges ccc = db.GetCoutinuousChanges(new ChangeOptions() {Since=0}, (x, y) =>
				{
					try
					{
						id = y.Id;
						Assert.IsNotNull(y.Id);
						Assert.IsTrue(y.Sequence > 0);
					}
					finally
					{
						evt.Set();
					}
				},
				new Result<CouchContinuousChanges>()).Wait())
			{
				JDocument result = db.CreateDocument(new JDocument(), new Result<JDocument>()).Wait();
				evt.WaitOne();

				Assert.AreEqual(result.Id, id);
			}
		}
		[Test]
		public void GetContinuousChangesWithDocument()
		{
			System.Threading.AutoResetEvent evt = new System.Threading.AutoResetEvent(false);
			if (client.HasDatabase("test_changes"))
				client.DeleteDatabase("test_changes");

			string id = null;
			var db = client.GetDatabase("test_changes");
			using (CouchContinuousChanges<JDocument> ccc = db.GetCoutinuousChanges<JDocument>(new ChangeOptions() { Since = 0 }, (x, y) =>
				{
					try
					{
						Assert.IsNotNull(y.Doc);
						id = y.Doc.Id;
						Assert.IsNotNull(y.Id);
						Assert.IsTrue(y.Sequence > 0);
					}
					finally
					{
						evt.Set();
					}
				},
			new Result<CouchContinuousChanges<JDocument>>()).Wait())
			{
				JDocument result = db.CreateDocument(new JDocument(), new Result<JDocument>()).Wait();
				evt.WaitOne();

				Assert.AreEqual(result.Id, id);
			}
		}
		[Test]
		public void CreateShow()
		{
			var db = client.GetDatabase(baseDatabase);

			JDocument d = new JDocument(@"{""title"": ""some title""}");
			d.Id = "sampleid";
			db.CreateDocument(d);

			CouchDesignDocument doc = new CouchDesignDocument("showdoc");
			doc.Shows.Add("simple", "function(doc, req) {return '<h1>' + doc.title + '</h1>';}");
			db.CreateDocument(doc);
		}
		[Test]
		public void CreateValidateDocUpdate()
		{
			const string validationFunction = "function(newDoc, oldDoc, userCtx) {}";

			var db = client.GetDatabase(baseDatabase);

			CouchDesignDocument doc = new CouchDesignDocument("validationFunction");
			doc.ValidateDocUpdate = validationFunction;
			db.CreateDocument(doc);

			JDocument jd = db.GetDocument<JDocument>(doc.Id);
			Assert.AreEqual(validationFunction, jd.Value<string>("validate_doc_update"));

			db.DeleteDocument(jd);
		}
		[Test]
		public void ViewStaleOptions()
		{
			ViewOptions viewOptions = new ViewOptions();
			viewOptions.Stale = Stale.Normal;

			Plug p = Plug.New("http://localhost").With(viewOptions);
			Assert.AreEqual("http://localhost?stale=ok",p.ToString());

			viewOptions.Stale = Stale.UpdateAfter;
			Plug p2 = Plug.New("http://localhost").With(viewOptions);
			Assert.AreEqual("http://localhost?stale=update_after", p2.ToString());
		}
	}
}
