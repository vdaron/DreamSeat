using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using LoveSeat.Interfaces;
using LoveSeat.Support;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MindTouch.Dream;
using MindTouch.Tasking;

namespace LoveSeat
{
	public class CouchDatabase : CouchBase, IDocumentDatabase
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

		#region Documents Management
		/// <summary>
		/// Creates a document using the json provided. 
		/// No validation or smarts attempted here by design for simplicities sake
		/// </summary>
		/// <param name="id">Id of Document</param>
		/// <param name="jsonForDocument"></param>
		/// <returns></returns>
		public Result<Document> CreateDocument(string id, string jsonForDocument, Result<Document> result)
		{
			JObject jobj = JObject.Parse(jsonForDocument);
			if (jobj.Value<object>("_rev") != null)
				jobj.Remove("_rev");

			BasePlug.At(id).Put(DreamMessage.Ok(MimeType.JSON, jobj.ToString(Formatting.None)), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						result.Return(new Document(JObject.Parse(a.ToText())));
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
		/// Create a new document
		/// </summary>
		/// <param name="doc"document></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public Result<Document> CreateDocument(Document doc, Result<Document> result)
		{
			return CreateDocument(doc.Id, doc.ToString(), result);
		}
		/// <summary>
		/// Creates a document when you intend for Couch to generate the id for you.
		/// </summary>
		/// <param name="jsonForDocument">Json for creating the document</param>
		/// <returns></returns>
		public Result<Document> CreateDocument(string jsonForDocument, Result<Document> result)
		{
			Document doc = new Document(JObject.Parse(jsonForDocument));

			BasePlug.Post(DreamMessage.Ok(MimeType.JSON, jsonForDocument), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						JObject jobj = JObject.Parse(a.ToText());
						doc["_id"] = jobj["id"];
						doc["_rev"] = jobj["rev"];
						result.Return(doc);
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
		public Result<JObject> DeleteDocument(string id, string rev, Result<JObject> result)
		{
			BasePlug.At(id).With("rev", rev).Delete(new Result<DreamMessage>()).WhenDone(
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
		/// Returns document with given id.
		/// will null if document is not found
		/// </summary>
		/// <param name="id">id of the document</param>
		/// <returns></returns>
		public Result<Document> GetDocument(string id, Result<Document> result)
		{
			BasePlug.At(id).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						result.Return(new Document(JObject.Parse(a.ToText())));
					}
					else if (a.Status == DreamStatus.NotFound)
					{
						result.Return((Document)null);
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
		public Result<T> GetDocument<T>(string id, Result<T> result)
		{
			return GetDocument(id, new ObjectSerializer<T>(), result);
		}
		public Result<T> GetDocument<T>(string id, IObjectSerializer<T> objectSerializer, Result<T> result)
		{
			BasePlug.At(id).Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					switch (a.Status)
					{
						case DreamStatus.Ok:
							result.Return(objectSerializer.Deserialize(a.ToText()));
							break;
						case DreamStatus.NotFound:
							result.Return(default(T));
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
		public Result<Document> SaveDocument(Document document, Result<Document> result)
		{
			if (document.Rev == null)
				return CreateDocument(document, result);

			BasePlug.At(document.Id).With("rev", document.Rev).Put(DreamMessage.Ok(MimeType.JSON, document.ToString()), new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Created)
					{
						document.Rev = JObject.Parse(a.ToText())["rev"].Value<string>();
						result.Return(document);
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

		#region Attachment Management
		/// <summary>
		/// Adds an attachment to a document.  If revision is not specified then the most recent will be fetched and used.  
		/// Warning: if you need document update conflicts to occur please use the method that specifies the revision
		/// </summary>
		/// <param name="id">id of the couch Document</param>
		/// <param name="attachment">byte[] of of the attachment.  Use File.ReadAllBytes()</param>
		/// <param name="contentType">Content Type must be specifed</param>	
		public Result<JObject> AddAttachment(string id, byte[] attachment, string filename, string contentType, Result<JObject> result)
		{
			GetDocument(id, new Result<Document>()).WhenDone(
				a => AddAttachment(id, a.Rev, attachment, filename, contentType, result),
				e => result.Throw(e)
			);
			return result;
		}
		/// <summary>
		/// Adds an attachment to the documnet.  Rev must be specified on this signature.  If you want to attach no matter what then use the method without the rev param
		/// </summary>
		/// <param name="id">id of the couch Document</param>
		/// <param name="rev">revision _rev of the Couch Document</param>
		/// <param name="attachment">byte[] of of the attachment.  Use File.ReadAllBytes()</param>
		/// <param name="filename">filename of the attachment</param>
		/// <param name="contentType">Content Type must be specifed</param>			
		/// <returns></returns>
		public Result<JObject> AddAttachment(string id, string rev, byte[] attachment, string filename, string contentType, Result<JObject> result)
		{
			BasePlug.At(id, filename).With("rev", rev).Put(DreamMessage.Ok(MimeType.JSON, attachment), new Result<DreamMessage>()).WhenDone(
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
		public Result<Stream> GetAttachmentStream(Document doc, string attachmentName, Result<Stream> result)
		{
			return GetAttachmentStream(doc.Id, doc.Rev, attachmentName, result);
		}
		public Result<Stream> GetAttachmentStream(string docId, string rev, string attachmentName, Result<Stream> result)
		{
			BasePlug.At(XUri.EncodeFragment(docId), XUri.EncodeFragment(attachmentName)).Get(new Result<DreamMessage>()).WhenDone(
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
		public Result<Stream> GetAttachmentStream(string docId, string attachmentName, Result<Stream> result)
		{
			GetDocument(docId, new Result<Document>()).WhenDone(
				a => GetAttachmentStream(docId, a.Rev, attachmentName, result),
				e => result.Throw(e)
			);

			return result;
		}
		public Result<JObject> DeleteAttachment(string id, string rev, string attachmentName, Result<JObject> result)
		{
			BasePlug.At(id, XUri.EncodeFragment(attachmentName)).With("rev", rev).Delete(new Result<DreamMessage>()).WhenDone(
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
		public Result<JObject> DeleteAttachment(string id, string attachmentName, Result<JObject> result)
		{
			GetDocument(id, new Result<Document>()).WhenDone(
				a => DeleteAttachment(a.Id, a.Rev, attachmentName, result),
				e => result.Throw(e)
			);
			return result;
		}
		#endregion

		/// <summary>
		/// Gets the results of a view with no view parameters. Use the overload to pass parameters
		/// </summary>
		/// <param name="viewName">The name of the view</param>
		/// <param name="designDoc">The design doc on which the view resides</param>
		/// <returns></returns>
		public Result<ViewResult<T>> View<T>(string viewName, string designDoc, Result<ViewResult<T>> result)
		{
			return View<T>(viewName, null, designDoc,result);
		}
		/// <summary>
		/// Gets the results of the view using the defaultDesignDoc and no view parameters.  Use the overloads to specify options.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <returns></returns>
		public Result<ViewResult<T>> View<T>(string viewName,Result<ViewResult<T>> result)
		{
			ThrowDesignDocException();
			return View<T>(viewName, DefaultDesignDocId,result);
		}
		/// <summary>
		/// Gets the results of the view using any and all parameters
		/// </summary>
		/// <param name="viewName">The name of the view</param>
		/// <param name="options">Options such as startkey etc.</param>
		/// <param name="designDoc">The design doc on which the view resides</param>
		/// <returns></returns>
		public Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, string designDoc, Result<ViewResult<T>> result)
		{
			return ProcessGenericResults<T>(BasePlug.At("_design",designDoc,"_view",viewName), options, new ObjectSerializer<T>(),result);
		}
		/// <summary>
		/// Allows you to specify options and uses the defaultDesignDoc Specified.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Result<ViewResult<T>> View<T>(string viewName, ViewOptions options,Result<ViewResult<T>> result)
		{
			ThrowDesignDocException();
			return View<T>(viewName, options, DefaultDesignDocId,result);
		}
		/// <summary>
		/// Allows you to override the objectSerializer and use the Default Design Doc settings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <param name="options"></param>
		/// <param name="objectSerializer"></param>
		/// <returns></returns>
		public Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, IObjectSerializer<T> objectSerializer, Result<ViewResult<T>> result)
		{
			ThrowDesignDocException();
			return View<T>(viewName, options, DefaultDesignDocId, objectSerializer,result);
		}
		/// <summary>
		/// Don't use this overload unless you intend to override the default ObjectSerialization behavior.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <param name="options"></param>
		/// <param name="designDoc"></param>
		/// <param name="objectSerializer">Only needed unless you'd like to override the default behavior of the serializer</param>
		/// <returns></returns>
		public Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, string designDoc, IObjectSerializer<T> objectSerializer, Result<ViewResult<T>> result)
		{
			return ProcessGenericResults<T>(BasePlug.At("_design", designDoc, "_view", viewName), options, objectSerializer, result);
		}
		public Result<string> Show(string showName, string docId, Result<string> result)
		{
			ThrowDesignDocException();
			return Show(showName, docId, DefaultDesignDocId, result);
		}
		public Result<string> Show(string showName, string docId, string designDoc, Result<string> result)
		{
			BasePlug.At("_design", designDoc, "_show", showName, docId).Get(new Result<DreamMessage>()).WhenDone(
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
		public Result<IListResult> List(string listName, string viewName, ViewOptions options, string designDoc, Result<IListResult> result)
		{
			BasePlug.At("_design", designDoc, "_list", viewName, options.ToString()).Get(new Result<DreamMessage>()).WhenDone(
				a => result.Return(new ListResult(a)),
				e => result.Throw(e)
			);
			return result;
		}
		public Result<IListResult> List(string listName, string viewName, ViewOptions options, Result<IListResult> result)
		{
			ThrowDesignDocException();
			return List(listName, viewName, options, DefaultDesignDocId, result);
		}

		/// <summary>
		/// Gets all the documents in the database using the _all_docs uri
		/// </summary>
		/// <returns></returns>
		public Result<ViewResult> GetAllDocuments(Result<ViewResult> result)
		{
			return ProcessResults(BasePlug.At("_all_docs"), null, result);
		}
		public Result<ViewResult> GetAllDocuments(ViewOptions options, Result<ViewResult> result)
		{
			return ProcessResults(BasePlug.At("_all_docs"), options, result);
		}

		#region Private Methods
		private Result<ViewResult<T>> ProcessGenericResults<T>(Plug uri, ViewOptions options, IObjectSerializer<T> objectSerializer, Result<ViewResult<T>> result)
		{
			uri.With(options).Get(new Result<DreamMessage>()).WhenDone(
				a => result.Return(new ViewResult<T>(a, objectSerializer)),
				e => result.Throw(e)
			);

			return result;
		}
		private Result<ViewResult> ProcessResults(Plug uri, ViewOptions options, Result<ViewResult> result)
		{
			uri.With(options).Get(new Result<DreamMessage>()).WhenDone(
				a => result.Return(new ViewResult(a)),
				e => result.Throw(e)
			);

			return result;
		}
		private void ThrowDesignDocException()
		{
			if (string.IsNullOrEmpty(DefaultDesignDocId))
				throw new Exception("You must use SetDefaultDesignDoc prior to using this signature.  Otherwise explicitly specify the design doc in the other overloads.");
		} 
		#endregion
	}
}