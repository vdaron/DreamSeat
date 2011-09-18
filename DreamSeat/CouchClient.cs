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
		/// <param name="aHost">The hostname of the CouchDB instance</param>
		/// <param name="aPort">The port of the CouchDB instance</param>
		/// <param name="aUserName">The username of the CouchDB instance</param>
		/// <param name="aPassword">The password of the CouchDB instance</param>
		public CouchClient(
			string aHost = Constants.LOCALHOST,
			int aPort = Constants.DEFAULT_PORT,
			string aUserName = null,
			string aPassword = null)
			: base(new XUri(String.Format("http://{0}:{1}",aHost,aPort)), aUserName, aPassword)
		{
		}

		#region Asynchronous Methods
		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="aReplicationOptions">Replication Options</param>
		/// <param name="aResult"></param>
		/// <returns></returns>
		[Obsolete("If using CouchDB >= 1.1 use CouchReplicationDocument")]
		public Result<JObject> TriggerReplication(ReplicationOptions aReplicationOptions, Result<JObject> aResult)
		{
			if (aReplicationOptions == null)
				throw new ArgumentNullException("aReplicationOptions");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			Plug p = BasePlug.At(Constants.REPLICATE);

			string json = aReplicationOptions.ToString();
			p.Post(DreamMessage.Ok(MimeType.JSON, json), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if((a.Status == DreamStatus.Accepted)||
					   (a.Status == DreamStatus.Ok))
					{
						aResult.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						aResult.Throw(new CouchException(a));
					}
				},
				aResult.Throw
			);

			return aResult;
		}
		/// <summary>
		/// Restarts the CouchDB instance. You must be authenticated as a user with administration privileges for this to work.
		/// </summary>
		/// <param name="aResult"></param>
		/// <returns></returns>
		public Result RestartServer(Result aResult)
		{
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At(Constants.RESTART).Post(DreamMessage.Ok(MimeType.JSON,String.Empty), new Result<DreamMessage>()).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
						aResult.Return();
					else
						aResult.Throw(new CouchException(a));
				},
				aResult.Throw
			);
			return aResult;
		}
		/// <summary>
		/// Returns a bool indicating whether or not the database exists.
		/// </summary>
		/// <param name="aDatabaseName"></param>
		/// <param name="aResult"></param>
		/// <returns></returns>
		public Result<bool> HasDatabase(string aDatabaseName, Result<bool> aResult)
		{
			if (String.IsNullOrEmpty(aDatabaseName))
				throw new ArgumentException("DatabaseName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At(XUri.EncodeFragment(aDatabaseName)).Head(new Result<DreamMessage>()).WhenDone(
				a => aResult.Return(a.Status == DreamStatus.Ok),
				aResult.Throw
			);

			return aResult;
		}
		/// <summary>
		/// Creates a database
		/// </summary>
		/// <param name="aDatabaseName">Name of new database</param>
		/// <param name="aResult"></param>
		/// <returns></returns>
		public Result<JObject> CreateDatabase(string aDatabaseName, Result<JObject> aResult)
		{
			if (String.IsNullOrEmpty(aDatabaseName))
				throw new ArgumentException("DatabaseName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At(XUri.EncodeFragment(aDatabaseName)).Put(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						aResult.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						aResult.Throw(new CouchException(a));
					}
				},
				aResult.Throw
			);
			return aResult;
		}
		/// <summary>
		/// Deletes the specified database
		/// </summary>
		/// <param name="aDatabaseName">Database to delete</param>
		/// <param name="aResult"></param>
		/// <returns></returns>
		public Result<JObject> DeleteDatabase(string aDatabaseName, Result<JObject> aResult)
		{
			if (String.IsNullOrEmpty(aDatabaseName))
				throw new ArgumentException("DatabaseName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At(XUri.EncodeFragment(aDatabaseName)).Delete(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						aResult.Return(JObject.Parse(a.ToText()));
					}
					else
					{
						aResult.Throw(new CouchException(a));
					}
				},
				aResult.Throw
			);

			return aResult;
		}
		/// <summary>
		///  Gets a Database
		/// </summary>
		/// <param name="aDatabaseName">Name of the database</param>
		/// <param name="createIfNotExists">Flag specifying if the database must be created if not found</param>
		/// <param name="aResult"></param>
		/// <returns></returns>
		public Result<CouchDatabase> GetDatabase(string aDatabaseName, bool createIfNotExists, Result<CouchDatabase> aResult)
		{
			if (String.IsNullOrEmpty(aDatabaseName))
				throw new ArgumentException("DatabaseName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");


			HasDatabase(aDatabaseName, new Result<bool>()).WhenDone(
				exists =>
				{
					if (exists)
					{
						aResult.Return(new CouchDatabase(BasePlug.At(XUri.EncodeFragment(aDatabaseName))));
					}
					else
					{
						if (createIfNotExists)
						{
							CreateDatabase(aDatabaseName, new Result<JObject>()).WhenDone(
								a => aResult.Return(new CouchDatabase(BasePlug.At(XUri.EncodeFragment(aDatabaseName)))),
								aResult.Throw
							);
						}
						else
						{
							aResult.Return((CouchDatabase)null);
						}
					}
				},
				aResult.Throw
			);

			return aResult;
		}
		/// <summary>
		/// Gets a Database, if database didn't exists, it will be created
		/// </summary>
		/// <param name="aDatabaseName">Name of the database</param>
		/// <param name="aResult"></param>
		/// <returns></returns>
		public Result<CouchDatabase> GetDatabase(string aDatabaseName, Result<CouchDatabase> aResult)
		{
			return GetDatabase(aDatabaseName, true, aResult);
		}
		#endregion

		#region Synchronous Methods
		/// <summary>
		/// Triggers one way replication from the source to target.  If bidirection is needed call this method twice with the source and target args reversed.
		/// </summary>
		/// <param name="aReplicationOptions">Replication options</param>
		/// <returns></returns>
		public JObject TriggerReplication(ReplicationOptions aReplicationOptions)
		{
			return TriggerReplication(aReplicationOptions, new Result<JObject>()).Wait();
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
		/// <param name="aDatabaseName"></param>
		/// <returns></returns>
		public bool HasDatabase(string aDatabaseName)
		{
			return HasDatabase(aDatabaseName, new Result<bool>()).Wait();
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
		public Result<Dictionary<string, Dictionary<string, string>>> GetConfig(Result<Dictionary<string, Dictionary<string, string>>> aResult)
		{
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At(Constants.CONFIG).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						aResult.Return(JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(a.ToText()));
					else
						aResult.Throw(new CouchException(a));
				},
				aResult.Throw
			);
			return aResult;
		}
		public Result<Dictionary<string, string>> GetConfigSection(string aSection, Result<Dictionary<string, string>> aResult)
		{
			if (aSection == null)
				throw new ArgumentNullException("aSection");
			if (aResult == null)
				throw new ArgumentNullException("aResult");
			if (String.IsNullOrEmpty(aSection))
				throw new ArgumentException("Section cannot be empty");

			BasePlug.At(Constants.CONFIG,XUri.EncodeFragment(aSection)).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					switch(a.Status)
					{
						case DreamStatus.Ok:
							aResult.Return(JsonConvert.DeserializeObject<Dictionary<string, string>>(a.ToText()));
							break;
						case DreamStatus.NotFound:
							aResult.Return(new Dictionary<string, string>());
							break;
						default:
							aResult.Throw(new CouchException(a));
							break;
					}
				},
				aResult.Throw
			);
			return aResult;
		}
		public Result<string> GetConfigValue(string aSection, string aKeyName, Result<string> aResult)
		{
			if (String.IsNullOrEmpty(aSection))
				throw new ArgumentException("aSection cannot be null nor empty");
			if (String.IsNullOrEmpty(aKeyName))
				throw new ArgumentException("aKeyName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");


			BasePlug.At(Constants.CONFIG,XUri.EncodeFragment(aSection),XUri.EncodeFragment(aKeyName)).Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
					{
						string value = a.ToText();
						switch(a.Status)
						{
							case DreamStatus.Ok:
								aResult.Return(value.Substring(1, value.Length - 3));// remove " and "\n
								break;
							case DreamStatus.NotFound:
								aResult.Return((string)null);
								break;
							default:
								aResult.Throw(new CouchException(a));
								break;
						}
					},
				aResult.Throw
			);
			return aResult;
		}
		public Result SetConfigValue(string aSection, string aKeyName, string aValue, Result aResult)
		{
			if (String.IsNullOrEmpty(aSection))
				throw new ArgumentException("aSection cannot be null nor empty");
			if (String.IsNullOrEmpty(aKeyName))
				throw new ArgumentException("aKeyName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			if (aValue == null)
				return DeleteConfigValue(aSection, aKeyName, aResult);

			BasePlug.At(Constants.CONFIG, XUri.EncodeFragment(aSection), XUri.EncodeFragment(aKeyName)).Put(DreamMessage.Ok(MimeType.TEXT, "\"" + aValue + "\""), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						aResult.Return();
					else
						aResult.Throw(new CouchException(a));
				},
				aResult.Throw
			);
			return aResult;
		}
		public Result DeleteConfigValue(string aSection, string aKeyName, Result aResult)
		{
			if (String.IsNullOrEmpty(aSection))
				throw new ArgumentException("aSection cannot be null nor empty");
			if (String.IsNullOrEmpty(aKeyName))
				throw new ArgumentException("aKeyName cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At(Constants.CONFIG, XUri.EncodeFragment(aSection), XUri.EncodeFragment(aKeyName)).Delete(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						aResult.Return();// remove " and "\n
					else
						aResult.Throw(new CouchException(a));
				},
				aResult.Throw
			);
			return aResult;
		} 
		#endregion

		#region Synchronous Methods
		public Dictionary<string, Dictionary<string, string>> GetConfig()
		{
			return GetConfig(new Result<Dictionary<string, Dictionary<string, string>>>()).Wait();
		}
		public Dictionary<string, string> GetConfig(string aSection)
		{
			return GetConfigSection(aSection, new Result<Dictionary<string, string>>()).Wait();
		}
		public string GetConfigValue(string aSection, string aKeyName)
		{
			return GetConfigValue(aSection, aKeyName, new Result<string>()).Wait();
		}
		public void SetConfigValue(string aSection, string aKeyName, string aValue)
		{
			SetConfigValue(aSection, aKeyName, aValue, new Result()).Wait();
		}
		public void DeleteConfigValue(string aSection, string aKeyName)
		{
			DeleteConfigValue(aSection, aKeyName, new Result()).Wait();
		} 
		#endregion
		#endregion

		#region User Management
		public void CreateAdminUser(string aUserName, string aPassword)
		{
			if (String.IsNullOrEmpty(aUserName))
				throw new ArgumentException("aUserName cannot be null nor empty");
			if (String.IsNullOrEmpty(aPassword))
				throw new ArgumentException("aPassword cannot be null nor empty");

			SetConfigValue("admins", aUserName, aPassword, new Result()).Wait();
			BasePlug.WithCredentials(aUserName, aPassword);// Logon(username, password, new Result<bool>()).Wait();
			CouchUser user = new CouchUser {Name = aUserName};

			ObjectSerializer<CouchUser> serializer = new ObjectSerializer<CouchUser>();
			BasePlug.At("_users", HttpUtility.UrlEncode("org.couchdb.user:" + aUserName)).Put(DreamMessage.Ok(MimeType.JSON, serializer.Serialize(user)), new Result<DreamMessage>()).Wait();
		}
		/// <summary>
		/// Deletes user  (if you have permission)
		/// </summary>
		/// <param name="aUser">User name</param>
		public void DeleteAdminUser(string aUser)
		{
			if (String.IsNullOrEmpty(aUser))
				throw new ArgumentException("aUser cannot be null nor empty");

			DeleteConfigValue("admins", aUser, new Result()).Wait();

			var userDb = GetDatabase("_users");
			var userId = "org.couchdb.user:" + aUser;
			var userDoc = userDb.GetDocument(userId, new Result<JDocument>()).Wait();
			if (userDoc != null)
			{
				userDb.DeleteDocument(userDoc.Id, userDoc.Rev, new Result<string>()).Wait();
			}
		}
		/// <summary>
		/// Returns true/false depending on whether or not the user is contained in the _users database
		/// </summary>
		/// <param name="aUserId"></param>
		/// <returns></returns>
		public bool HasUser(string aUserId)
		{
			if (String.IsNullOrEmpty(aUserId))
				throw new ArgumentException("aUserId cannot be empty");

			return GetUser(aUserId) != null;
		}

		/// <summary>
		/// Get's the user.
		/// </summary>
		/// <param name="aUserId"></param>
		/// <returns></returns>
		public JDocument GetUser(string aUserId)
		{
			if (String.IsNullOrEmpty(aUserId))
				throw new ArgumentException("aUser cannot be null nor empty");

			var db = new CouchDatabase(BasePlug.At("_users"));
			aUserId = "org.couchdb.user:" + HttpUtility.UrlEncode(aUserId);
			return db.GetDocument(aUserId, new Result<JDocument>()).Wait();
		} 
		#endregion
	}
}
