using System;
using System.IO;
using Newtonsoft.Json.Linq;
using MindTouch.Tasking;

namespace LoveSeat.Interfaces
{
	public interface IDocumentDatabase
	{
		/// <summary>
		/// Creates a document using the json provided. 
		/// No validation or smarts attempted here by design for simplicities sake
		/// </summary>
		/// <param name="id">Id of Document</param>
		/// <param name="jsonForDocument"></param>
		/// <returns></returns>
		Result<Document> CreateDocument(string id, string jsonForDocument, Result<Document> result);

		Result<Document> CreateDocument(Document doc, Result<Document> result);

		/// <summary>
		/// Creates a document when you intend for Couch to generate the id for you.
		/// </summary>
		/// <param name="jsonForDocument">Json for creating the document</param>
		/// <returns></returns>
		Result<Document> CreateDocument(string jsonForDocument, Result<Document> result);

		Result<JObject> DeleteDocument(string id, string rev, Result<JObject> result);

		/// <summary>
		/// Returns null if document is not found
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Result<Document> GetDocument(string id, Result<Document> result);

		Result<T> GetDocument<T>(string id, Result<T> result);
		Result<T> GetDocument<T>(string id, IObjectSerializer<T> objectSerializer, Result<T> result);

		/// <summary>
		/// Adds an attachment to a document.  If revision is not specified then the most recent will be fetched and used.  Warning: if you need document update conflicts to occur please use the method that specifies the revision
		/// </summary>
		/// <param name="id">id of the couch Document</param>
		/// <param name="attachment">byte[] of of the attachment.  Use File.ReadAllBytes()</param>
		/// <param name="contentType">Content Type must be specifed</param>	
		Result<JObject> AddAttachment(string id, byte[] attachment, string filename, string contentType,Result<JObject> result);

		/// <summary>
		/// Adds an attachment to the documnet.  Rev must be specified on this signature.  If you want to attach no matter what then use the method without the rev param
		/// </summary>
		/// <param name="id">id of the couch Document</param>
		/// <param name="rev">revision _rev of the Couch Document</param>
		/// <param name="attachment">byte[] of of the attachment.  Use File.ReadAllBytes()</param>
		/// <param name="filename">filename of the attachment</param>
		/// <param name="contentType">Content Type must be specifed</param>			
		/// <returns></returns>
		Result<JObject> AddAttachment(string id, string rev, byte[] attachment, string filename, string contentType, Result<JObject> result);

		Result<Stream> GetAttachmentStream(Document doc, string attachmentName,Result<Stream> result);
		Result<Stream> GetAttachmentStream(string docId, string rev, string attachmentName, Result<Stream> result);
		Result<Stream> GetAttachmentStream(string docId, string attachmentName, Result<Stream> result);
		Result<JObject> DeleteAttachment(string id, string rev, string attachmentName, Result<JObject> result);
		Result<JObject> DeleteAttachment(string id, string attachmentName, Result<JObject> result);
		Result<Document> SaveDocument(Document document, Result<Document> result);

		/// <summary>
		/// Gets the results of a view with no view parameters.  Use the overload to pass parameters
		/// </summary>
		/// <param name="viewName">The name of the view</param>
		/// <param name="designDoc">The design doc on which the view resides</param>
		/// <returns></returns>
		Result<ViewResult<T>> View<T>(string viewName, string designDoc, Result<ViewResult<T>> result);

		/// <summary>
		/// Gets the results of the view using any and all parameters
		/// </summary>
		/// <param name="viewName">The name of the view</param>
		/// <param name="options">Options such as startkey etc.</param>
		/// <param name="designDoc">The design doc on which the view resides</param>
		/// <returns></returns>
		Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, string designDoc, Result<ViewResult<T>> result);

		/// <summary>
		/// Don't use this overload unless you intend to override the default ObjectSerialization behavior.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <param name="options"></param>
		/// <param name="designDoc"></param>
		/// <param name="objectSerializer">Only needed unless you'd like to override the default behavior of the serializer</param>
		/// <returns></returns>
		Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, string designDoc, IObjectSerializer<T> objectSerializer, Result<ViewResult<T>> result);

		/// <summary>
		/// Gets all the documents in the database using the _all_docs uri
		/// </summary>
		/// <returns></returns>
		Result<ViewResult> GetAllDocuments(Result<ViewResult> result);

		Result<ViewResult> GetAllDocuments(ViewOptions options, Result<ViewResult> result);

		/// <summary>
		/// Gets the results of the view using the defaultDesignDoc and no view parameters.  Use the overloads to specify options.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <returns></returns>
		Result<ViewResult<T>> View<T>(string viewName,Result<ViewResult<T>> result);

		/// <summary>
		/// Allows you to specify options and uses the defaultDesignDoc Specified.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, Result<ViewResult<T>> result);

		/// <summary>
		/// Allows you to override the objectSerializer and use the Default Design Doc settings.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="viewName"></param>
		/// <param name="options"></param>
		/// <param name="objectSerializer"></param>
		/// <returns></returns>
		Result<ViewResult<T>> View<T>(string viewName, ViewOptions options, IObjectSerializer<T> objectSerializer, Result<ViewResult<T>> result);

		Result<T> GetDocument<T>(Guid id, IObjectSerializer<T> objectSerializer, Result<T> result);
		Result<T> GetDocument<T>(Guid id, Result<T> result);
		Result<string> Show(string showName, string docId,Result<string> result);
		Result<IListResult> List(string listName, string viewName, ViewOptions options, string designDoc, Result<IListResult> result);
		Result<IListResult> List(string listName, string viewName, ViewOptions options, Result<IListResult> result);
	}
}