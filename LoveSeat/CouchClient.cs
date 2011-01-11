using System;
using System.Net;
using System.Web;
using LoveSeat.Support;
using Newtonsoft.Json.Linq;
using MindTouch.Tasking;
using MindTouch.Dream;

namespace LoveSeat
{
	/// <summary>
	/// Used as the starting point for any communication with CouchDB
	/// </summary>
	public class CouchClient : CouchBase
	{
		/// <summary>
		/// This is only intended for use if your CouchDb is in Admin Party
		/// </summary>
		public CouchClient()
			: this("localhost", 5984, null, null)
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		public CouchClient(string username, string password)
			: this("localhost", 5984, username, password)
		{
		}

		/// <summary>
		/// Constructs the CouchClient and gets an authentication cookie (10 min)
		/// </summary>
		/// <param name="host">The hostname of the CouchDB instance</param>
		/// <param name="port">The port of the CouchDB instance</param>
		/// <param name="username">The username of the CouchDB instance</param>
		/// <param name="password">The password of the CouchDB instance</param>
		public CouchClient(string host, int port, string username, string password)
			: base(new XUri(String.Format("http://{0}:{1}",host,port)), username, password)
		{
		}

		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="source">Uri or database name of database to replicate from</param>
		/// <param name="target">Uri or database name of database to replicate to</param>
		/// <param name="continuous">Whether or not CouchDB should continue to replicate going forward on it's own</param>
		/// <returns></returns>
		public Result<JObject> TriggerReplication(string source, string target, bool continuous, Result<JObject> result)
		{
			Plug p = Plug.At("_replicate");
			ReplicationOptions options = new ReplicationOptions(source, target, continuous);
			p.Post(DreamMessage.Ok(MimeType.JSON, options.ToString()), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						result.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						result.Throw(new CouchException(a));
					}
				},
				e => result.Throw(e)
			);

			return result;
		}
		public JObject TriggerReplication(string source, string target, bool continuous)
		{
			return TriggerReplication(source, target, continuous, new Result<JObject>()).Wait();
		}
		public Result<JObject> TriggerReplication(string source, string target, Result<JObject> result)
		{
			return TriggerReplication(source, target, false, result);
		}
		public JObject TriggerReplication(string source, string target)
		{
			return TriggerReplication(source, target, new Result<JObject>()).Wait();
		}

		/// <summary>
		/// Returns a bool indicating whether or not the database exists.
		/// </summary>
		/// <param name="databaseName"></param>
		/// <returns></returns>
		public Result<bool> HasDatabase(string databaseName, Result<bool> result)
		{
			Plug.At(XUri.EncodeFragment(databaseName)).Head(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return(true);
					else
						result.Return(false);
				},
				e => result.Throw(e)
			);

			return result;
		}
		public bool HasDatabase(string databaseName)
		{
			return HasDatabase(databaseName, new Result<bool>()).Wait();
		}
		/// <summary>
		/// Creates a database
		/// </summary>
		/// <param name="databaseName">Name of new database</param>
		/// <returns></returns>
		public Result<JObject> CreateDatabase(string databaseName, Result<JObject> result)
		{
			Plug.At(XUri.EncodeFragment(databaseName)).Put(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						result.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						result.Throw(new CouchException(a));
					}
				},
				e => result.Throw(e)
			);
			return result;
		}
		public JObject CreateDatabase(string databaseName)
		{
			return CreateDatabase(databaseName, new Result<JObject>()).Wait();
		}
		/// <summary>
		/// Deletes the specified database
		/// </summary>
		/// <param name="databaseName">Database to delete</param>
		/// <returns></returns>
		public Result<JObject> DeleteDatabase(string databaseName, Result<JObject> result)
		{
			Plug.At(XUri.EncodeFragment(databaseName)).Delete(new Result<DreamMessage>()).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
					{
						result.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						result.Throw(new CouchException(a));
					}
				},
				e => result.Throw(e)
			);

			return result;
		}
		public JObject DeleteDatabase(string databaseName)
		{
			return DeleteDatabase(databaseName, new Result<JObject>()).Wait();
		}

		/// <summary>
		/// Gets a Database object
		/// </summary>
		/// <param name="databaseName">Name of database to fetch</param>
		/// <returns></returns>
		public CouchDatabase GetDatabase(string databaseName)
		{
			return new CouchDatabase(Plug, databaseName);
		}

		#region User Management (disabled for now)
		public JObject CreateAdminUser(string usernameToCreate, string passwordToCreate)
		{
			//            //Creates the user in the local.ini
			//            var iniResult = GetRequest(baseUri + "_config/admins/" + HttpUtility.UrlEncode(usernameToCreate))
			//                .Put().Json().Data("\"" + passwordToCreate + "\"").GetResponse();

			//            var user = @"{ ""name"": ""%name%"",
			//  ""_id"": ""org.couchdb.user:%name%"", ""type"": ""user"", ""roles"": [],
			//}".Replace("%name%", usernameToCreate).Replace("\r\n", "");
			//            var docResult = GetRequest(baseUri + "_users/org.couchdb.user:" + HttpUtility.UrlEncode(usernameToCreate))
			//                .Put().Json().Data(user).GetResponse().GetJObject();
			//            return docResult;
			return JObject.Parse("{}");
		}
		/// <summary>
		/// Deletes user  (if you have permission)
		/// </summary>
		/// <param name="userToDelete"></param>
		public void DeleteAdminUser(string userToDelete)
		{
			//var iniResult = GetRequest(baseUri + "_config/admins/" + HttpUtility.UrlEncode(userToDelete))
			//    .Delete().Json().GetResponse();

			//var userDb = this.GetDatabase("_users");
			//var userId = "org.couchdb.user:" + HttpUtility.UrlEncode(userToDelete);
			//var userDoc = userDb.GetDocument(userId, new Result<Document>()).Wait();
			//if (userDoc != null)
			//{
			//    userDb.DeleteDocument(userDoc.Id, userDoc.Rev, new Result<JObject>()).Wait();
			//}
		}
		/// <summary>
		/// Returns true/false depending on whether or not the user is contained in the _users database
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public bool HasUser(string userId)
		{
			return GetUser(userId) != null;
		}
		/// <summary>
		/// Get's the user.  
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public Document GetUser(string userId)
		{
			var db = new CouchDatabase(Plug, "_users");
			userId = "org.couchdb.user:" + HttpUtility.UrlEncode(userId);
			return db.GetDocument(userId, new Result<Document>()).Wait();
		} 
		#endregion
	}
}
