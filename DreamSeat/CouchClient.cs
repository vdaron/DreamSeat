using System;
using System.Collections.Generic;
using System.Web;
using DreamSeat.Support;
using MindTouch.Dream;
using MindTouch.Tasking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DreamSeat
{
	/// <summary>
	/// Used as the starting point for any communication with CouchDB
	/// </summary>
	public class CouchClient : CouchBase
	{
		/// <summary>
		/// Constructs the CouchClient and gets an authentication cookie (10 min)
		/// </summary>
		/// <param name="host">The hostname of the CouchDB instance</param>
		/// <param name="port">The port of the CouchDB instance</param>
		/// <param name="username">The username of the CouchDB instance</param>
		/// <param name="password">The password of the CouchDB instance</param>
		public CouchClient(
			string host = Constants.LOCALHOST,
			int port = Constants.DEFAULT_PORT,
			string username = null,
			string password = null)
			: base(new XUri(String.Format("http://{0}:{1}",host,port)), username, password)
		{
		}

		#region Asynchronous Methods

		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="options">Replication Options</param>
		/// <param name="result"></param>
		/// <returns></returns>
		[Obsolete("If using CouchDB >= 1.1 use CouchReplicationDocument")]
		public Result<JObject> TriggerReplication(ReplicationOptions options, Result<JObject> result)
		{
			Plug p = BasePlug.At(Constants.REPLICATE);

			string json = options.ToString();
			p.Post(DreamMessage.Ok(MimeType.JSON, json), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if((a.Status == DreamStatus.Accepted)||
					   (a.Status == DreamStatus.Ok))
					{
						result.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						result.Throw(new CouchException(a));
					}
				},
				result.Throw
			);

			return result;
		}

		/// <summary>
		/// Restarts the CouchDB instance. You must be authenticated as a user with administration privileges for this to work.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result RestartServer(Result result)
		{
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.RESTART).Post(DreamMessage.Ok(MimeType.JSON,String.Empty), new Result<DreamMessage>()).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
						result.Return();
					else
						result.Throw(new CouchException(a));
				},
				result.Throw
			);
			return result;
		}

		/// <summary>
		/// Returns a bool indicating whether or not the database exists.
		/// </summary>
		/// <param name="databaseName"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<bool> HasDatabase(string databaseName, Result<bool> result)
		{
			BasePlug.At(XUri.EncodeFragment(databaseName)).Head(new Result<DreamMessage>()).WhenDone(
				a => result.Return(a.Status == DreamStatus.Ok),
				result.Throw
			);

			return result;
		}

		/// <summary>
		/// Creates a database
		/// </summary>
		/// <param name="databaseName">Name of new database</param>
		/// <param name="result"></param>
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
				result.Throw
			);
			return result;
		}

		/// <summary>
		/// Deletes the specified database
		/// </summary>
		/// <param name="databaseName">Database to delete</param>
		/// <param name="result"></param>
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
				result.Throw
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
								result.Throw
							);
						}
						else
						{
							result.Return((CouchDatabase)null);
						}
					}
				},
				result.Throw
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
		/// <param name="options">Replication options</param>
		/// <returns></returns>
		public JObject TriggerReplication(ReplicationOptions options)
		{
			return TriggerReplication(options, new Result<JObject>()).Wait();
		}
		/// <summary>
		/// Restarts the CouchDB instance. You must be authenticated as a user with administration privileges for this to work.
		/// </summary>
		public void RestartServer()
		{
			RestartServer(new Result()).Wait();
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
			BasePlug.At(Constants.CONFIG).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return(JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(a.ToText()));
					else
						result.Throw(new CouchException(a));
				},
				result.Throw
			);
			return result;
		}
		public Result<Dictionary<string, string>> GetConfigSection(string section, Result<Dictionary<string, string>> result)
		{
			BasePlug.At(Constants.CONFIG,XUri.EncodeFragment(section)).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					switch(a.Status)
					{
						case DreamStatus.Ok:
							result.Return(JsonConvert.DeserializeObject<Dictionary<string, string>>(a.ToText()));
							break;
						case DreamStatus.NotFound:
							result.Return(new Dictionary<string, string>());
							break;
						default:
							result.Throw(new CouchException(a));
							break;
					}
				},
				result.Throw
			);
			return result;
		}
		public Result<string> GetConfigValue(string section, string keyName, Result<string> result)
		{
			BasePlug.At(Constants.CONFIG,XUri.EncodeFragment(section),XUri.EncodeFragment(keyName)).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
					{
						string value = a.ToText();
						switch(a.Status)
						{
							case DreamStatus.Ok:
								result.Return(value.Substring(1, value.Length - 3));// remove " and "\n
								break;
							case DreamStatus.NotFound:
								result.Return((string)null);
								break;
							default:
								result.Throw(new CouchException(a));
								break;
						}
					},
				result.Throw
			);
			return result;
		}
		public Result SetConfigValue(string section, string keyName, string value, Result result)
		{
			BasePlug.At(Constants.CONFIG, XUri.EncodeFragment(section), XUri.EncodeFragment(keyName)).Put(DreamMessage.Ok(MimeType.TEXT, "\"" + value + "\""), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return();
					else
						result.Throw(new CouchException(a));
				},
				result.Throw
			);
			return result;
		}
		public Result DeleteConfigValue(string section, string keyName, Result result)
		{
			BasePlug.At(Constants.CONFIG, XUri.EncodeFragment(section), XUri.EncodeFragment(keyName)).Delete(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return();// remove " and "\n
					else
						result.Throw(new CouchException(a));
				},
				result.Throw
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

		#region User Management
		public void CreateAdminUser(string username, string password)
		{
			SetConfigValue("admins", username, password, new Result()).Wait();
			BasePlug.WithCredentials(username, password);// Logon(username, password, new Result<bool>()).Wait();
			CouchUser user = new CouchUser {Name = username};

			ObjectSerializer<CouchUser> serializer = new ObjectSerializer<CouchUser>();
			BasePlug.At("_users", HttpUtility.UrlEncode("org.couchdb.user:" + username)).Put(DreamMessage.Ok(MimeType.JSON, serializer.Serialize(user)), new Result<DreamMessage>()).Wait();
		}
		/// <summary>
		/// Deletes user  (if you have permission)
		/// </summary>
		/// <param name="user">User name</param>
		public void DeleteAdminUser(string user)
		{
			DeleteConfigValue("admins", user, new Result()).Wait();

			var userDb = GetDatabase("_users");
			var userId = "org.couchdb.user:" + user;
			var userDoc = userDb.GetDocument(userId, new Result<JDocument>()).Wait();
			if (userDoc != null)
			{
				userDb.DeleteDocument(userDoc.Id, userDoc.Rev, new Result<string>()).Wait();
			}
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
		public JDocument GetUser(string userId)
		{
			var db = new CouchDatabase(BasePlug.At("_users"));
			userId = "org.couchdb.user:" + HttpUtility.UrlEncode(userId);
			return db.GetDocument(userId, new Result<JDocument>()).Wait();
		} 
		#endregion
	}
}
