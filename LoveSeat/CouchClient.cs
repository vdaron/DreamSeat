using System;
using System.Net;
using System.Web;
using LoveSeat.Support;
using Newtonsoft.Json.Linq;
using MindTouch.Tasking;
using MindTouch.Dream;
using System.Collections.Generic;
using Newtonsoft.Json;

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

		#region Asynchronous Methods
		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="source">Uri or database name of database to replicate from</param>
		/// <param name="target">Uri or database name of database to replicate to</param>
		/// <param name="continuous">Whether or not CouchDB should continue to replicate going forward on it's own</param>
		/// <returns></returns>
		public Result<JObject> TriggerReplication(string source, string target, bool continuous, Result<JObject> result)
		{
			Plug p = BasePlug.At("_replicate");
			ReplicationOptions options = new ReplicationOptions(source, target, continuous);
			p.Post(DreamMessage.Ok(MimeType.JSON, options.ToString()), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Accepted)
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
		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="source">Uri or database name of database to replicate from</param>
		/// <param name="target">Uri or database name of database to replicate to</param>
		/// <returns></returns>
		public Result<JObject> TriggerReplication(string source, string target, Result<JObject> result)
		{
			return TriggerReplication(source, target, false, result);
		}
		/// <summary>
		/// Returns a bool indicating whether or not the database exists.
		/// </summary>
		/// <param name="databaseName"></param>
		/// <returns></returns>
		public Result<bool> HasDatabase(string databaseName, Result<bool> result)
		{
			BasePlug.At(XUri.EncodeFragment(databaseName)).Head(new Result<DreamMessage>()).WhenDone(
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
		/// <summary>
		/// Creates a database
		/// </summary>
		/// <param name="databaseName">Name of new database</param>
		/// <returns></returns>
		public Result<JObject> CreateDatabase(string databaseName, Result<JObject> result)
		{
			BasePlug.At(XUri.EncodeFragment(databaseName)).Put(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
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
		/// <summary>
		/// Deletes the specified database
		/// </summary>
		/// <param name="databaseName">Database to delete</param>
		/// <returns></returns>
		public Result<JObject> DeleteDatabase(string databaseName, Result<JObject> result)
		{
			BasePlug.At(XUri.EncodeFragment(databaseName)).Delete(new Result<DreamMessage>()).WhenDone(
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
		/// <summary>
		///  Gets a Database
		/// </summary>
		/// <param name="databaseName">Name of the database</param>
		/// <param name="createIfNotExists">Flag specifying if the database must be created if not found</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchDatabase> GetDatabase(string databaseName, bool createIfNotExists, Result<CouchDatabase> result)
		{
			HasDatabase(databaseName, new Result<bool>()).WhenDone(
				exists =>
				{
					if (exists)
					{
						result.Return(new CouchDatabase(BasePlug.At(XUri.EncodeFragment(databaseName))));
					}
					else
					{
						if (createIfNotExists)
						{
							CreateDatabase(databaseName, new Result<JObject>()).WhenDone(
								a => result.Return(new CouchDatabase(BasePlug.At(XUri.EncodeFragment(databaseName)))),
								e => result.Throw(e)
							);
						}
						else
						{
							result.Return((CouchDatabase)null);
						}
					}
				},
				e => result.Throw(e)
			);

			return result;
		}
		/// <summary>
		/// Gets a Database, if database didn't exists, it will be created
		/// </summary>
		/// <param name="databaseName">Name of the database</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchDatabase> GetDatabase(string databaseName, Result<CouchDatabase> result)
		{
			return GetDatabase(databaseName, true, result);
		}
		#endregion

		#region Synchronous Methods
		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="source">Uri or database name of database to replicate from</param>
		/// <param name="target">Uri or database name of database to replicate to</param>
		/// <param name="continuous">Whether or not CouchDB should continue to replicate going forward on it's own</param>
		/// <returns></returns>
		public JObject TriggerReplication(string source, string target, bool continuous)
		{
			return TriggerReplication(source, target, continuous, new Result<JObject>()).Wait();
		}
		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="source">Uri or database name of database to replicate from</param>
		/// <param name="target">Uri or database name of database to replicate to</param>
		/// <returns></returns>
		public JObject TriggerReplication(string source, string target)
		{
			return TriggerReplication(source, target, new Result<JObject>()).Wait();
		}
		/// <summary>
		/// Returns a bool indicating whether or not the database exists.
		/// </summary>
		/// <param name="databaseName"></param>
		/// <returns></returns>
		public bool HasDatabase(string databaseName)
		{
			return HasDatabase(databaseName, new Result<bool>()).Wait();
		}
		/// <summary>
		/// Creates a database
		/// </summary>
		/// <param name="databaseName">Name of new database</param>
		/// <returns></returns>
		public JObject CreateDatabase(string databaseName)
		{
			return CreateDatabase(databaseName, new Result<JObject>()).Wait();
		}
		/// <summary>
		/// Deletes the specified database
		/// </summary>
		/// <param name="databaseName">Database to delete</param>
		/// <returns></returns>
		public JObject DeleteDatabase(string databaseName)
		{
			return DeleteDatabase(databaseName, new Result<JObject>()).Wait();
		}
		/// <summary>
		/// Gets a Database object, if database didn't exists, it will be created
		/// </summary>
		/// <param name="databaseName">Name of database to fetch</param>
		/// <returns></returns>
		public CouchDatabase GetDatabase(string databaseName)
		{
			return GetDatabase(databaseName, new Result<CouchDatabase>()).Wait();
		}
		/// <summary>
		/// Gets a Database object
		/// </summary>createIfNotExists
		/// <param name="databaseName">Name of database to fetch</param>
		/// <param name="createIfNotExists">Flag specifying if the database must be created if not found</param>
		/// <returns></returns>
		public CouchDatabase GetDatabase(string databaseName, bool createIfNotExists)
		{
			return GetDatabase(databaseName, createIfNotExists, new Result<CouchDatabase>()).Wait();
		}
		#endregion

		#region Configuration Management

		#region Asynchronous Methods
		public Result<Dictionary<string, Dictionary<string, string>>> GetConfig(Result<Dictionary<string, Dictionary<string, string>>> result)
		{
			BasePlug.At("_config").Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return(JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(a.ToText()));
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		public Result<Dictionary<string, string>> GetConfigSection(string section, Result<Dictionary<string, string>> result)
		{
			BasePlug.At("_config",XUri.EncodeFragment(section)).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return(JsonConvert.DeserializeObject<Dictionary<string, string>>(a.ToText()));
					else if (a.Status == DreamStatus.NotFound)
						result.Return(new Dictionary<string, string>());
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		public Result<string> GetConfigValue(string section, string keyName, Result<string> result)
		{
			BasePlug.At("_config",XUri.EncodeFragment(section),XUri.EncodeFragment(keyName)).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					string value = a.ToText();
					if (a.Status == DreamStatus.Ok)
						result.Return(value.Substring(1, value.Length - 3));// remove " and "\n
					else if (a.Status == DreamStatus.NotFound)
						result.Return((string)null);
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		public Result SetConfigValue(string section, string keyName, string value, Result result)
		{
			BasePlug.At("_config", XUri.EncodeFragment(section), XUri.EncodeFragment(keyName)).Put(DreamMessage.Ok(MimeType.TEXT, "\"" + value + "\""), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return();
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		public Result DeleteConfigValue(string section, string keyName, Result result)
		{
			BasePlug.At("_config", XUri.EncodeFragment(section), XUri.EncodeFragment(keyName)).Delete(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return();// remove " and "\n
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		} 
		#endregion

		#region Synchronous Methods
		public Dictionary<string, Dictionary<string, string>> GetConfig()
		{
			return GetConfig(new Result<Dictionary<string, Dictionary<string, string>>>()).Wait();
		}
		public Dictionary<string, string> GetConfig(string section)
		{
			return GetConfigSection(section, new Result<Dictionary<string, string>>()).Wait();
		}
		public string GetConfigValue(string section, string keyName)
		{
			return GetConfigValue(section, keyName, new Result<string>()).Wait();
		}
		public void SetConfigValue(string section, string keyName, string value)
		{
			SetConfigValue(section, keyName, value, new Result()).Wait();
		}
		public void DeleteConfigValue(string section, string keyName)
		{
			DeleteConfigValue(section, keyName, new Result()).Wait();
		} 
		#endregion

		#endregion

		#region User Management (disabled for now)
		public JObject CreateAdminUser(string username, string password)
		{
			SetConfigValue("admins", username, password, new Result()).Wait();

//            var user = @"{ ""name"": ""%name%"",
//			  ""_id"": ""org.couchdb.user:%name%"", ""type"": ""user"", ""roles"": [],
//			}".Replace("%name%", usernameToCreate).Replace("\r\n", "");
//            var docResult = GetRequest(baseUri + "_users/org.couchdb.user:" + HttpUtility.UrlEncode(usernameToCreate))
//                .Put().Json().Data(user).GetResponse().GetJObject();
//            return docResult;
			return JObject.Parse("{}");
		}
		/// <summary>
		/// Deletes user  (if you have permission)
		/// </summary>
		/// <param name="user">User name</param>
		public void DeleteAdminUser(string user)
		{
			DeleteConfigValue("admins", user, new Result()).Wait();

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
		public JsonDocument GetUser(string userId)
		{
			var db = new CouchDatabase(BasePlug.At("_users"));
			userId = "org.couchdb.user:" + HttpUtility.UrlEncode(userId);
			return db.GetDocument(userId, new Result<JsonDocument>()).Wait();
		} 
		#endregion
	}
}
