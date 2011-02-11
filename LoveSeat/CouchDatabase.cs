using System;
using System.IO;
using LoveSeat.Interfaces;
using LoveSeat.Support;
using MindTouch.Dream;
using MindTouch.Tasking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoveSeat
{
	public class CouchDatabase : CouchBase
	{
		public string DefaultDesignDocId { get; set; }

		public CouchDatabase(XUri databaseUri)
			: base(databaseUri)
		{
		}

		public CouchDatabase(XUri databaseUri,string username,string password)
			: base(databaseUri,username,password)
		{
		}

		/// <summary>
		/// Retrieve DatabaseInformation
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchDatabaseInfo> GetInfo(Result<CouchDatabaseInfo> result)
		{
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.Get(DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a => {
					if(a.Status == DreamStatus.Ok)
						result.Return(JsonConvert.DeserializeObject<CouchDatabaseInfo>(a.ToText()));
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		public CouchDatabaseInfo GetInfo()
		{
			return GetInfo(new Result<CouchDatabaseInfo>()).Wait();
		}

		/// <summary>
		/// Request compaction of the specified database. Compaction compresses the disk database file
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result Compact(Result result)
		{
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.COMPACT).Post(DreamMessage.Ok(MimeType.JSON,String.Empty), new Result<DreamMessage>()).WhenDone(
				a => {
					if (a.Status == DreamStatus.Accepted)
					{
						result.Return();
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
		public void Compact()
		{
			Compact(new Result()).Wait();
		}

		/// <summary>
		/// Compacts the view indexes associated with the specified design document. You can use this in place of the full database compaction if
		/// you know a specific set of view indexes have been affected by a recent database change
		/// </summary>
		/// <param name="documentViewId">Design Document id to compact</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result CompactDocumentView(string documentViewId,Result result)
		{
			if (String.IsNullOrEmpty(documentViewId))
				throw new ArgumentNullException(documentViewId);
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.COMPACT).At(XUri.EncodeFragment(documentViewId)).Post(DreamMessage.Ok(MimeType.JSON, String.Empty), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Accepted)
					{
						result.Return();
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
		public void CompactDocumentView(string documentViewId)
		{
			CompactDocumentView(documentViewId, new Result()).Wait();
		}

		#region Change Management
		/// <summary>
		/// Request database changes
		/// </summary>
		/// <param name="options">Change options</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchChanges> GetChanges(ChangeOptions options, Result<CouchChanges> result)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			if (result == null)
				throw new ArgumentNullException("result");

			options.Feed = ChangeFeed.Normal;

			BasePlug.At(Constants._CHANGES).With(options).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						ObjectSerializer<CouchChanges> serializer = new ObjectSerializer<CouchChanges>();
						result.Return(serializer.Deserialize(a.ToText()));
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
		/// Request database changes including documents
		/// </summary>
		/// <typeparam name="T">Type of document used while returning changes</typeparam>
		/// <param name="options">Change options</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchChanges<T>> GetChanges<T>(ChangeOptions options, Result<CouchChanges<T>> result) where T : ICouchDocument
		{
			if (options == null)
				throw new ArgumentNullException("options");
			if (result == null)
				throw new ArgumentNullException("result");

			options.Feed = ChangeFeed.Normal;
			options.IncludeDocs = true;

			BasePlug.At(Constants._CHANGES).With(options).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						ObjectSerializer<CouchChanges<T>> serializer = new ObjectSerializer<CouchChanges<T>>();
						result.Return(serializer.Deserialize(a.ToText()));
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
		/// Request continuous changes from database
		/// </summary>
		/// <param name="options">Change options</param>
		/// <param name="callback">Callback used for each change notification</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchContinuousChanges> GetCoutinuousChanges(
			ChangeOptions options,
			CouchChangeDelegate callback,
			Result<CouchContinuousChanges> result)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (result == null)
				throw new ArgumentNullException("result");

			options.Feed = ChangeFeed.Continuous;
			BasePlug.At(Constants._CHANGES).With(options).InvokeEx(Verb.GET, DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.IsSuccessful)
					{
						result.Return(new CouchContinuousChanges(a, callback));
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
		/// Request continuous changes from database including Documents
		/// </summary>
		/// <typeparam name="T">>Type of document used while returning changes</typeparam>
		/// <param name="options">Change options</param>
		/// <param name="callback">Callback used for each change notification</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<CouchContinuousChanges<T>> GetCoutinuousChanges<T>(
			ChangeOptions options,
			CouchChangeDelegate<T> callback,
			Result<CouchContinuousChanges<T>> result) where T : ICouchDocument
		{
			if (options == null)
				throw new ArgumentNullException("options");
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (result == null)
				throw new ArgumentNullException("result");

			options.Feed = ChangeFeed.Continuous;
			options.IncludeDocs = true;

			BasePlug.At(Constants._CHANGES).With(options).InvokeEx(Verb.GET, DreamMessage.Ok(), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.IsSuccessful)
					{
						result.Return(new CouchContinuousChanges<T>(a, callback));
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
		/// Request database changes
		/// </summary>
		/// <param name="options">Change options</param>
		/// <returns></returns>
		public CouchChanges GetChanges(ChangeOptions options)
		{
			return GetChanges(options, new Result<CouchChanges>()).Wait();
		}
		/// <summary>
		/// Request database changes including documents
		/// </summary>
		/// <typeparam name="T">Type of document used while returning changes</typeparam>
		/// <param name="options">Change options</param>
		/// <returns></returns>
		public CouchChanges<T> GetChanges<T>(ChangeOptions options) where T : ICouchDocument
		{
			return GetChanges(options, new Result<CouchChanges<T>>()).Wait();
		}
		/// <summary>
		/// Request continuous changes from database
		/// </summary>
		/// <param name="options">Change options</param>
		/// <param name="callback">Callback used for each change notification</param>
		/// <returns></returns>
		public CouchContinuousChanges GetCoutinuousChanges(ChangeOptions options, CouchChangeDelegate callback)
		{
			return GetCoutinuousChanges(options, callback, new Result<CouchContinuousChanges>()).Wait();
		}
		/// <summary>
		/// Request continuous changes from database including Documents
		/// </summary>
		/// <typeparam name="T">>Type of document used while returning changes</typeparam>
		/// <param name="options">Change options</param>
		/// <param name="callback">Callback used for each change notification</param>
		/// <returns></returns>
		public CouchContinuousChanges<T> GetCoutinuousChanges<T>(ChangeOptions options, CouchChangeDelegate<T> callback) where T : ICouchDocument
		{
			return GetCoutinuousChanges(options, callback, new Result<CouchContinuousChanges<T>>()).Wait();
		}

		#endregion

		#region Documents Management
		#region Primitives methods

		/// <summary>
		/// Creates a document when you intend for Couch to generate the id for you.
		/// </summary>
		/// <param name="json">Json for creating the document</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<string> CreateDocument(string json, Result<string> result)
		{
			return CreateDocument(null, json, result);
		}

		/// <summary>
		/// Creates a document using the json provided. 
		/// No validation or smarts attempted here by design for simplicities sake
		/// </summary>
		/// <param name="id">Id of Document, if null or empty, id will be generated by the server</param>
		/// <param name="json"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<string> CreateDocument(string id, string json, Result<string> result)
		{
			if (String.IsNullOrEmpty(json))
				throw new ArgumentNullException("json");
			if (result == null)
				throw new ArgumentNullException("result");

			JObject jobj = JObject.Parse(json);
			if (jobj.Value<object>(Constants._REV) != null)
				jobj.Remove(Constants._REV);

			Plug p = BasePlug;
			string verb = Verb.POST;
			if (!String.IsNullOrEmpty(id))
			{
				p = p.AtPath(XUri.EncodeFragment(id));
				verb = Verb.PUT;
			}

			p.Invoke(verb,DreamMessage.Ok(MimeType.JSON, jobj.ToString(Formatting.None)), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						result.Return(a.ToText());
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
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rev"></param>
		/// <param name="json"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<string> SaveDocument(string id, string rev, string json, Result<string> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (String.IsNullOrEmpty(rev))
				throw new ArgumentNullException("rev");
			if (String.IsNullOrEmpty(json))
				throw new ArgumentNullException("json");
			if (result == null)
				throw new ArgumentNullException("result");

			JObject jobj = JObject.Parse(json);
			BasePlug.AtPath(XUri.EncodeFragment(id)).With(Constants.REV, rev).Put(DreamMessage.Ok(MimeType.JSON, jobj.ToString(Formatting.None)), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						result.Return(a.ToText());
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
		/// Delete the specified document
		/// </summary>
		/// <param name="id">id of the document</param>
		/// <param name="rev">revision</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<string> DeleteDocument(string id, string rev, Result<string> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (String.IsNullOrEmpty(rev))
				throw new ArgumentNullException("rev");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.AtPath(XUri.EncodeFragment(id)).With(Constants.REV, rev).Delete(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return(a.ToText());
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Retrive a document based on doc id
		/// </summary>
		/// <param name="id">id of the document</param>
		/// <param name="result">Jobject or </param>
		/// <returns></returns>
		public Result<string> GetDocument(string id, Result<string> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.AtPath(XUri.EncodeFragment(id)).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					switch(a.Status)
					{
						case DreamStatus.Ok:
							result.Return(a.ToText());
							break;
						case DreamStatus.NotFound:
							result.Return((string)null);
							break;
						default:
							result.Throw(new CouchException(a));
							break;
					}
				},
				e => result.Throw(e)
			);
			return result;
		}
		#endregion

		/// <summary>
		/// Create a document based on object based on ICouchDocument interface. If the ICouchDocument does not have an Id, CouchDB will generate the id for you
		/// </summary>
		/// <typeparam name="TDocument">ICouchDocument Type to return</typeparam>
		/// <param name="doc">ICouchDocument to create</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<TDocument> CreateDocument<TDocument>(TDocument doc, Result<TDocument> result) where TDocument : class, ICouchDocument
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (result == null)
				throw new ArgumentNullException("result");

			ObjectSerializer<TDocument> serializer = new ObjectSerializer<TDocument>();

			CreateDocument(doc.Id, serializer.Serialize(doc), new Result<string>()).WhenDone(
				a =>
				{
					JObject value = JObject.Parse(a);
					doc.Id = value[Constants.ID].Value<string>();
					doc.Rev = value[Constants.REV].Value<string>();
					result.Return(doc);
				},
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Create a document based on object based on ICouchDocument interface
		/// </summary>
		/// <typeparam name="TDocument"></typeparam>
		/// <param name="doc"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<TDocument> UpdateDocument<TDocument>(TDocument doc, Result<TDocument> result) where TDocument : class, ICouchDocument
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (result == null)
				throw new ArgumentNullException("result");
			if (String.IsNullOrEmpty(doc.Id))
				throw new ArgumentException("Document must have an id");
			if (String.IsNullOrEmpty(doc.Rev))
				throw new ArgumentException("Document must have a revision");

			ObjectSerializer<TDocument> objectSerializer = new ObjectSerializer<TDocument>();

			SaveDocument(doc.Id, doc.Rev, objectSerializer.Serialize(doc), new Result<string>()).WhenDone(
				a =>
				{
					JObject value = JObject.Parse(a);
					doc.Id = value[Constants.ID].Value<string>();
					doc.Rev = value[Constants.REV].Value<string>();
					result.Return(doc);
				},
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Retrieve Document using with specified id and deserialize result
		/// </summary>
		/// <typeparam name="TDocument">Object created during deserialization, must inherit ICouchDocument</typeparam>
		/// <param name="id">id of the document</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<TDocument> GetDocument<TDocument>(string id, Result<TDocument> result) where TDocument : ICouchDocument
		{
			BasePlug.AtPath(XUri.EncodeFragment(id)).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					switch (a.Status)
					{
						case DreamStatus.Ok:
							try
							{
								ObjectSerializer<TDocument> objectSerializer = new ObjectSerializer<TDocument>();
								TDocument res = objectSerializer.Deserialize(a.ToText());
								// If object inherit BaseDocument, id and rev are set during Deserialiation
								if (!(res is CouchDocument))
								{
									// Load id and rev (TODO: try to optimise this)
									JObject idrev = JObject.Parse(a.ToText());
									res.Id = idrev[Constants._ID].Value<string>();
									res.Rev = idrev[Constants._REV].Value<string>();
								}
								result.Return(res);
							}
							catch (Exception ex)
							{
								result.Throw(ex);
							}
							break;
						case DreamStatus.NotFound:
							result.Return(default(TDocument));
							break;
						default:
							result.Throw(new CouchException(a));
							break;
					}
				},
				e => result.Throw(e)
			);

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> DeleteDocument(ICouchDocument doc, Result<JObject> result)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (result == null)
				throw new ArgumentNullException("result");

			DeleteDocument(doc.Id, doc.Rev, new Result<string>()).WhenDone(
				a => result.Return(JObject.Parse(a)),
				e => result.Throw(e)
			);
			return result;
		}

		public TDocument CreateDocument<TDocument>(TDocument doc) where TDocument : class, ICouchDocument
		{
			return CreateDocument(doc, new Result<TDocument>()).Wait();
		}
		public TDocument UpdateDocument<TDocument>(TDocument doc) where TDocument : class, ICouchDocument
		{
			return UpdateDocument(doc, new Result<TDocument>()).Wait();
		}
		public TDocument GetDocument<TDocument>(string id) where TDocument : class, ICouchDocument
		{
			return GetDocument(id, new Result<TDocument>()).Wait();
		}
		public void DeleteDocument(ICouchDocument doc)
		{
			DeleteDocument(doc, new Result<JObject>()).Wait();
		}
		#endregion

		#region Attachment Management
		#region Primitives methods

		/// <summary>
		/// Adds an attachment to the documnet.  Rev must be specified on this signature.  If you want to attach no matter what then use the method without the rev param
		/// </summary>
		/// <param name="id">id of the couch Document</param>
		/// <param name="rev">revision _rev of the Couch Document</param>
		/// <param name="attachment">Stream of the attachment. Will be closed once request is sent</param>
		/// <param name="attachmentLength">Length of the Stream.</param>
		/// <param name="fileName">filename of the attachment</param>
		/// <param name="contentType">Content Type of the document</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> AddAttachment(string id, string rev, Stream attachment, long attachmentLength, string fileName, MimeType contentType, Result<JObject> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (String.IsNullOrEmpty(rev))
				throw new ArgumentNullException("rev");
			if (attachment == null)
				throw new ArgumentNullException("attachment");
			if (attachmentLength < 0)
				throw new ArgumentOutOfRangeException("attachmentLength");
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			if (contentType == null)
				throw new ArgumentNullException("contentType");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.AtPath(XUri.EncodeFragment(id)).At(XUri.EncodeFragment(fileName)).With(Constants.REV, rev).Put(DreamMessage.Ok(contentType, attachmentLength, attachment), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
						result.Return(JObject.Parse(a.ToText()));
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Delete an attachment
		/// </summary>
		/// <param name="id">Id of the document</param>
		/// <param name="rev">Revision of the document</param>
		/// <param name="attachmentName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> DeleteAttachment(string id, string rev, string attachmentName, Result<JObject> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (String.IsNullOrEmpty(rev))
				throw new ArgumentNullException("rev");
			if (String.IsNullOrEmpty(attachmentName))
				throw new ArgumentNullException("attachmentName");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.AtPath(XUri.EncodeFragment(id)).At(XUri.EncodeFragment(attachmentName)).With(Constants.REV, rev).Delete(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
						result.Return(JObject.Parse(a.ToText()));
					else
						result.Throw(new CouchException(a));
				},
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Retrieve an attachment
		/// </summary>
		/// <param name="docId">Id of the document</param>
		/// <param name="rev">Revision of the document</param>
		/// <param name="attachmentName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<Stream> GetAttachment(string docId, string rev, string attachmentName, Result<Stream> result)
		{
			if (String.IsNullOrEmpty(docId))
				throw new ArgumentNullException("docId");
			if (String.IsNullOrEmpty(rev))
				throw new ArgumentNullException("rev");
			if (String.IsNullOrEmpty(attachmentName))
				throw new ArgumentNullException("attachmentName");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.AtPath(XUri.EncodeFragment(docId)).At(XUri.EncodeFragment(attachmentName)).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						result.Return(a.ToStream());
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
		#endregion
		/// <summary>
		/// Add Attachment to the document
		/// </summary>
		/// <param name="id">id of the document</param>
		/// <param name="rev">rev of the document</param>
		/// <param name="attachment">attachment stream (will be closed)</param>
		/// <param name="fileName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> AddAttachment(string id, string rev, Stream attachment, string fileName, Result<JObject> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (String.IsNullOrEmpty(rev))
				throw new ArgumentNullException("rev");
			if (attachment == null)
				throw new ArgumentNullException("attachment");
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			if (result == null)
				throw new ArgumentNullException("result");

			return AddAttachment(id, rev, attachment, attachment.Length, fileName, MimeType.FromFileExtension(fileName), result);
		}

		/// <summary>
		/// Adds an attachment to a document.  If revision is not specified then the most recent will be fetched and used.  
		/// Warning: if you need document update conflicts to occur please use the method that specifies the revision
		/// </summary>
		/// <param name="id">id of the couch Document</param>
		/// <param name="attachment">Stream of the attachment. Will be closed once request is sent</param>
		/// <param name="filename">Filename must be specifed</param>
		/// <param name="result"></param>	
		public Result<JObject> AddAttachment(string id, Stream attachment, string filename, Result<JObject> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (attachment == null)
				throw new ArgumentNullException("attachment");
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentNullException("fileName");
			if (result == null)
				throw new ArgumentNullException("result");

			GetDocument(id,new Result<CouchDocument>()).WhenDone(
				a => AddAttachment(id, a.Rev, attachment, filename, result),
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Add Attachment to the document to the specified document
		/// </summary>
		/// <param name="doc">Document</param>
		/// <param name="filePath">File path</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> AddAttachment(ICouchDocument doc, string filePath, Result<JObject> result)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (String.IsNullOrEmpty(filePath))
				throw new ArgumentNullException("filePath");
			if (result == null)
				throw new ArgumentNullException("result");
			if (!File.Exists(filePath))
				throw new FileNotFoundException("File not found", filePath);

			return AddAttachment(doc.Id, doc.Rev, File.Open(filePath, FileMode.Open), Path.GetFileName(filePath), result);
		}
		/// <summary>
		/// GetAttachment Stream of document
		/// </summary>
		/// <param name="doc">document</param>
		/// <param name="attachmentName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<Stream> GetAttachment(ICouchDocument doc, string attachmentName, Result<Stream> result)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (String.IsNullOrEmpty(attachmentName))
				throw new ArgumentNullException("attachmentName");
			if (result == null)
				throw new ArgumentNullException("result");

			return GetAttachment(doc.Id, doc.Rev, attachmentName, result);
		}
		/// <summary>
		/// GetAttachment Stream of document with specified id
		/// </summary>
		/// <param name="docId">Document id</param>
		/// <param name="attachmentName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<Stream> GetAttachment(string docId, string attachmentName, Result<Stream> result)
		{
			if (String.IsNullOrEmpty(docId))
				throw new ArgumentNullException("docId");
			if (String.IsNullOrEmpty(attachmentName))
				throw new ArgumentNullException("attachmentName");
			if (result == null)
				throw new ArgumentNullException("result");

			GetDocument(docId, new Result<CouchDocument>()).WhenDone(
				a => GetAttachment(docId, a.Rev, attachmentName, result),
				e => result.Throw(e)
			);

			return result;
		}
		/// <summary>
		/// Delete Attachment
		/// </summary>
		/// <param name="id">id of the document</param>
		/// <param name="attachmentName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> DeleteAttachment(string id, string attachmentName, Result<JObject> result)
		{
			if (String.IsNullOrEmpty(id))
				throw new ArgumentNullException("id");
			if (String.IsNullOrEmpty(attachmentName))
				throw new ArgumentNullException("attachmentName");
			if (result == null)
				throw new ArgumentNullException("result");

			GetDocument(id, new Result<CouchDocument>()).WhenDone(
				a => DeleteAttachment(a.Id, a.Rev, attachmentName, result),
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Delete Attachment
		/// </summary>
		/// <param name="doc">document</param>
		/// <param name="attachmentName">Attachment file name</param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<JObject> DeleteAttachment(ICouchDocument doc, string attachmentName, Result<JObject> result)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			if (String.IsNullOrEmpty(attachmentName))
				throw new ArgumentNullException("attachmentName");
			if (result == null)
				throw new ArgumentNullException("result");

			DeleteAttachment(doc.Id, doc.Rev, attachmentName, result).WhenDone(
				a => result.Return(a),
				e => result.Throw(e)
			);
			return result;
		}

		public JObject AddAttachment(string id, string rev, Stream attachment, string fileName)
		{
			return AddAttachment(id, rev, attachment, fileName,new Result<JObject>()).Wait();
		}
		public JObject AddAttachment(string id, Stream attachment, string fileName)
		{
			return AddAttachment(id, attachment, fileName, new Result<JObject>()).Wait();
		}
		public JObject AddAttachment(ICouchDocument doc, string filePath)
		{
			return AddAttachment(doc, filePath, new Result<JObject>()).Wait();
		}
		public Stream GetAttachment(ICouchDocument doc, string attachmentName)
		{
			return GetAttachment(doc, attachmentName, new Result<Stream>()).Wait();
		}
		public Stream GetAttachment(string docId, string attachmentName)
		{
			return GetAttachment(docId, attachmentName, new Result<Stream>()).Wait();
		}
		public JObject DeleteAttachment(string id, string attachmentName)
		{
			return DeleteAttachment(id, attachmentName, new Result<JObject>()).Wait();
		}
		public JObject DeleteAttachment(ICouchDocument doc, string attachmentName)
		{
			return DeleteAttachment(doc, attachmentName, new Result<JObject>()).Wait();
		}
		#endregion

		#region All Documents Special View
		#region Asynchronous Methods
		public Result<ViewResult<TKey, TValue>> GetAllDocuments<TKey, TValue>(Result<ViewResult<TKey, TValue>> result)
		{
			return GetAllDocuments(new ViewOptions(), result);
		}
		public Result<ViewResult<TKey, TValue>> GetAllDocuments<TKey, TValue>(ViewOptions options, Result<ViewResult<TKey, TValue>> result)
		{
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.ALL_DOCS).With(options).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok || a.Status == DreamStatus.NotModified)
					{
						result.Return(GetViewResult<TKey, TValue>(a));
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
		public Result<ViewResult<TKey, TValue, TDocument>> GetAllDocuments<TKey, TValue, TDocument>(Result<ViewResult<TKey, TValue, TDocument>> result) where TDocument : ICouchDocument
		{
			return GetAllDocuments(new ViewOptions(), result);
		}
		public Result<ViewResult<TKey, TValue, TDocument>> GetAllDocuments<TKey, TValue, TDocument>(ViewOptions options, Result<ViewResult<TKey, TValue, TDocument>> result) where TDocument : ICouchDocument
		{
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.ALL_DOCS).With(Constants.INCLUDE_DOCS, true).With(options).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok || a.Status == DreamStatus.NotModified)
					{
						result.Return(GetViewResult<TKey, TValue, TDocument>(a));
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
		#endregion
		#region Synchronous Methods
		public ViewResult<TKey, TValue> GetAllDocuments<TKey, TValue>()
		{
			return GetAllDocuments<TKey, TValue>(new ViewOptions());
		}
		public ViewResult<TKey, TValue> GetAllDocuments<TKey, TValue>(ViewOptions options)
		{
			return GetAllDocuments(options, new Result<ViewResult<TKey, TValue>>()).Wait();
		}
		public ViewResult<TKey, TValue, TDocument> GetAllDocuments<TKey, TValue, TDocument>() where TDocument : ICouchDocument
		{
			return GetAllDocuments<TKey, TValue, TDocument>(new ViewOptions());
		}
		public ViewResult<TKey, TValue, TDocument> GetAllDocuments<TKey, TValue, TDocument>(ViewOptions options) where TDocument : ICouchDocument
		{
			return GetAllDocuments(options, new Result<ViewResult<TKey, TValue, TDocument>>()).Wait();
		}
		#endregion
		#endregion

		#region Views
		#region Asynchronous methods
		private Result<DreamMessage> GetView(string viewId, string viewName, ViewOptions options, Result<DreamMessage> result)
		{
			if (String.IsNullOrEmpty(viewId))
				throw new ArgumentNullException("viewId");
			if (String.IsNullOrEmpty(viewName))
				throw new ArgumentNullException("viewName");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.DESIGN, XUri.EncodeFragment(viewId), Constants.VIEW, XUri.EncodeFragment(viewName)).With(options).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok || a.Status == DreamStatus.NotModified)
					{
						result.Return(a);
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

		public Result<ViewResult<TKey, TValue>> GetView<TKey, TValue>(string viewId, string viewName, Result<ViewResult<TKey, TValue>> result)
		{
			return GetView(viewId, viewName, new ViewOptions(), result);
		}
		public Result<ViewResult<TKey, TValue>> GetView<TKey, TValue>(string viewId, string viewName, ViewOptions options, Result<ViewResult<TKey, TValue>> result)
		{
			if (String.IsNullOrEmpty(viewId))
				throw new ArgumentNullException("viewId");
			if (String.IsNullOrEmpty(viewName))
				throw new ArgumentNullException("viewName");
			if (result == null)
				throw new ArgumentNullException("result");

			GetView(viewId,viewName,options,new Result<DreamMessage>()).WhenDone(
				a => result.Return(GetViewResult<TKey, TValue>(a)),
				e => result.Throw(e)
			);
			return result;
		}
		public Result<ViewResult<TKey, TValue, TDocument>> GetView<TKey, TValue, TDocument>(string viewId, string viewName, Result<ViewResult<TKey, TValue, TDocument>> result) where TDocument : ICouchDocument
		{
			return GetView(viewId, viewName, new ViewOptions(), result);
		}
		public Result<ViewResult<TKey, TValue, TDocument>> GetView<TKey, TValue, TDocument>(string viewId, string viewName, ViewOptions options, Result<ViewResult<TKey, TValue, TDocument>> result) where TDocument : ICouchDocument
		{
			if (String.IsNullOrEmpty(viewId))
				throw new ArgumentNullException("viewId");
			if (String.IsNullOrEmpty(viewName))
				throw new ArgumentNullException("viewName");
			if (result == null)
				throw new ArgumentNullException("result");

			// Ensure Documents are requested
			options.IncludeDocs = true;

			GetView(viewId,viewName,options,new Result<DreamMessage>()).WhenDone(
				a => result.Return(GetViewResult<TKey, TValue, TDocument>(a)),
				e => result.Throw(e)
			);
			return result;
		}

		public Result<ViewResult<TKey, TValue>> GetTempView<TKey, TValue>(CouchView view, Result<ViewResult<TKey, TValue>> result)
		{
			return GetTempView(view, null, result);
		}
		public Result<ViewResult<TKey, TValue>> GetTempView<TKey, TValue>(CouchView view, ViewOptions options, Result<ViewResult<TKey, TValue>> result)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if(result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.TEMP_VIEW).With(options).Post(DreamMessage.Ok(MimeType.JSON, JsonConvert.SerializeObject(view)), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok || a.Status == DreamStatus.NotModified)
					{
						result.Return(GetViewResult<TKey, TValue>(a));
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
		public Result<ViewResult<TKey, TValue, TDocument>> GetTempView<TKey, TValue, TDocument>(CouchView view, Result<ViewResult<TKey, TValue, TDocument>> result) where TDocument : ICouchDocument
		{
			return GetTempView(view, null, result);
		}
		public Result<ViewResult<TKey, TValue, TDocument>> GetTempView<TKey, TValue, TDocument>(CouchView view, ViewOptions options, Result<ViewResult<TKey, TValue, TDocument>> result) where TDocument : ICouchDocument
		{
			if (view == null)
				throw new ArgumentNullException("view");
			if (result == null)
				throw new ArgumentNullException("result");

			BasePlug.At(Constants.TEMP_VIEW).With(options).Post(DreamMessage.Ok(MimeType.JSON, JsonConvert.SerializeObject(view)), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok || a.Status == DreamStatus.NotModified)
					{
						result.Return(GetViewResult<TKey, TValue, TDocument>(a));
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

		public Result<JObject> GetView(string viewId, string viewName, Result<JObject> result)
		{
			return GetView(viewId,viewName,new ViewOptions(),result);
		}
		public Result<JObject> GetView(string viewId, string viewName, ViewOptions options, Result<JObject> result)
		{
			if (String.IsNullOrEmpty(viewId))
				throw new ArgumentNullException("viewId");
			if (String.IsNullOrEmpty(viewName))
				throw new ArgumentNullException("viewName");
			if (result == null)
				throw new ArgumentNullException("result");

			GetView(viewId, viewName, options, new Result<DreamMessage>()).WhenDone(
				a => result.Return(JObject.Parse(a.ToText())),
				e => result.Throw(e)
			);
			return result;
		}
		#endregion
		#region Synchronous methods
		public ViewResult<TKey, TValue> GetView<TKey, TValue>(string viewId, string viewName)
		{
			return GetView(viewId, viewName, new Result<ViewResult<TKey, TValue>>()).Wait();
		}
		public ViewResult<TKey, TValue> GetView<TKey, TValue>(string viewId, string viewName, ViewOptions options)
		{
			return GetView(viewId, viewName, options, new Result<ViewResult<TKey, TValue>>()).Wait();
		}
		public ViewResult<TKey, TValue, TDocument> GetView<TKey, TValue, TDocument>(string viewId, string viewName) where TDocument : ICouchDocument
		{
			return GetView(viewId, viewName, new Result<ViewResult<TKey, TValue, TDocument>>()).Wait();
		}
		public ViewResult<TKey, TValue, TDocument> GetView<TKey, TValue, TDocument>(string viewId, string viewName, ViewOptions options) where TDocument : ICouchDocument
		{
			return GetView(viewId, viewName, options, new Result<ViewResult<TKey, TValue, TDocument>>()).Wait();
		}

		public ViewResult<TKey, TValue> GetTempView<TKey, TValue>(CouchView view)
		{
			return GetTempView(view, null, new Result<ViewResult<TKey, TValue>>()).Wait();
		}
		public ViewResult<TKey, TValue> GetTempView<TKey, TValue>(CouchView view, ViewOptions options)
		{
			return GetTempView(view, options, new Result<ViewResult<TKey, TValue>>()).Wait();
		}
		public ViewResult<TKey, TValue, TDocument> GetTempView<TKey, TValue, TDocument>(CouchView view) where TDocument : ICouchDocument
		{
			return GetTempView(view, null, new Result<ViewResult<TKey, TValue, TDocument>>()).Wait();
		}
		public ViewResult<TKey, TValue, TDocument> GetTempView<TKey, TValue, TDocument>(CouchView view, ViewOptions options) where TDocument : ICouchDocument
		{
			return GetTempView(view, options, new Result<ViewResult<TKey, TValue, TDocument>>()).Wait();
		}

		public JObject GetView(string viewId, string viewName)
		{
			return GetView(viewId, viewName, new Result<JObject>()).Wait();
		}
		public JObject GetView(string viewId, string viewName, ViewOptions options)
		{
			return GetView(viewId, viewName, options, new Result<JObject>()).Wait();
		}
		#endregion
		#endregion

		private static ViewResult<TKey, TValue> GetViewResult<TKey, TValue>(DreamMessage a)
		{
			ViewResult<TKey, TValue> val;
			switch (a.Status)
			{
				case DreamStatus.Ok:
					ObjectSerializer<ViewResult<TKey, TValue>> objectSerializer = new ObjectSerializer<ViewResult<TKey, TValue>>();
					val = objectSerializer.Deserialize(a.ToText());
					val.Status = DreamStatus.Ok;
					val.ETag = a.Headers.ETag;
					break;
				default:
					val = new ViewResult<TKey, TValue> { Status = a.Status };
					break;
			}
			return val;
		}
		private static ViewResult<TKey, TValue, TDocument> GetViewResult<TKey, TValue, TDocument>(DreamMessage a) where TDocument : ICouchDocument
		{
			ViewResult<TKey, TValue, TDocument> val;
			switch (a.Status)
			{
				case DreamStatus.Ok:
					ObjectSerializer<ViewResult<TKey, TValue, TDocument>> objectSerializer = new ObjectSerializer<ViewResult<TKey, TValue, TDocument>>();
					val = objectSerializer.Deserialize(a.ToText());
					val.Status = DreamStatus.Ok;
					val.ETag = a.Headers.ETag;
					break;
				default:
					val = new ViewResult<TKey, TValue, TDocument> { Status = a.Status };
					break;
			}
			return val;
		}
	}
}