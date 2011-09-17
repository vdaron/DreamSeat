using System;
using MindTouch.Dream;
using MindTouch.Tasking;
using Newtonsoft.Json.Linq;

namespace DreamSeat.Support
{
	public abstract class CouchBase
	{
		protected Plug BasePlug;

		protected CouchBase(XUri baseUri)
		{
			BasePlug = Plug.New(baseUri);
		}
		protected CouchBase(XUri baseUri, string userName, string password)
			:this(baseUri)
		{
			BasePlug = BasePlug.WithCredentials(userName, password);
		}

		/// <summary>
		/// Perform Cookie base authentication with given username and password
		/// Resulting cookie will be automatically used for all subsequent requests
		/// </summary>
		/// <param name="userName">User Name</param>
		/// <param name="password">Password</param>
		/// <param name="result"></param>
		/// <returns>true if authentication succeed</returns>
		public Result<bool> Logon(string userName, string password, Result<bool> result)
		{
			string content = String.Format("name={0}&password={1}", userName, password);

			BasePlug.At("_session").Post(DreamMessage.Ok(MimeType.FORM_URLENCODED,content), new Result<DreamMessage>()).WhenDone(
				a => {
					switch(a.Status)
					{
						case DreamStatus.Ok:
							BasePlug.CookieJar.Update(a.Cookies, new XUri(BasePlug.Uri.SchemeHostPort));
							BasePlug = BasePlug.WithHeader("X-CouchDB-WWW-Authenticate", "Cookie");
							result.Return(true);
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
		public Result<bool> Logoff(Result<bool> result)
		{
			BasePlug.At("_session").Delete(new Result<DreamMessage>()).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
						result.Return(true);
					else
						result.Throw(new CouchException(a));
				},
				result.Throw
			);
			return result;
		}
		public Result<bool> IsLogged(Result<bool> result)
		{
			BasePlug.At("_session").Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						JObject user = JObject.Parse(a.ToText());
						result.Return(user["info"]["authenticated"] != null);
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
	}
}