using System;
using MindTouch.Dream;
using MindTouch.Tasking;
using Newtonsoft.Json.Linq;

namespace DreamSeat.Support
{
	public abstract class CouchBase
	{
		protected Plug BasePlug;

		protected CouchBase(Plug aPlug)
		{
			BasePlug = aPlug;
		}

		protected CouchBase(XUri aBaseUri, string aUserName = null, string aPassword = null)
		{
			if (aBaseUri == null)
				throw new ArgumentNullException("aBaseUri");

			BasePlug = Plug.New(aBaseUri).WithCredentials(aUserName, aPassword);
		}

		/// <summary>
		/// Perform Cookie base authentication with given username and password
		/// Resulting cookie will be automatically used for all subsequent requests
		/// </summary>
		/// <param name="aUserName">User Name</param>
		/// <param name="aPassword">Password</param>
		/// <param name="aResult"></param>
		/// <returns>true if authentication succeed</returns>
		public Result<bool> Logon(string aUserName, string aPassword, Result<bool> aResult)
		{
			if (String.IsNullOrEmpty(aUserName))
				throw new ArgumentException("aUserName cannot be null nor empty");
			if (String.IsNullOrEmpty(aPassword))
				throw new ArgumentException("aPassword cannot be null nor empty");
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			string content = String.Format("name={0}&password={1}", aUserName, aPassword);

			BasePlug.At("_session").Post(DreamMessage.Ok(MimeType.FORM_URLENCODED,content), new Result<DreamMessage>()).WhenDone(
				a => {
					switch(a.Status)
					{
						case DreamStatus.Ok:
							BasePlug.CookieJar.Update(a.Cookies, new XUri(BasePlug.Uri.SchemeHostPort));
							BasePlug = BasePlug.WithHeader("X-CouchDB-WWW-Authenticate", "Cookie");
							aResult.Return(true);
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
		public Result<bool> Logoff(Result<bool> aResult)
		{
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At("_session").Delete(new Result<DreamMessage>()).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
						aResult.Return(true);
					else
						aResult.Throw(new CouchException(a));
				},
				aResult.Throw
			);
			return aResult;
		}
		public Result<bool> IsLogged(Result<bool> aResult)
		{
			if (aResult == null)
				throw new ArgumentNullException("aResult");

			BasePlug.At("_session").Get(new Result<DreamMessage>()).WhenDone(
				a =>
				{
					if (a.Status == DreamStatus.Ok)
					{
						JObject user = JObject.Parse(a.ToText());
						aResult.Return(user["info"]["authenticated"] != null);
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
	}
}