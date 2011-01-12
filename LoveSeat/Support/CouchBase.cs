using System;
using System.Net;
using MindTouch.Dream;
using MindTouch.Tasking;

namespace LoveSeat.Support
{
    public abstract class CouchBase
    {
		protected Plug BasePlug;

		protected CouchBase(XUri baseUri)
		{
			BasePlug = Plug.New(baseUri);
		}
		protected CouchBase(XUri baseUri, string userName, string password)
		{
			BasePlug = Plug.New(baseUri).WithCredentials(userName, password);
		}

		public Result<bool> Authenticate(string userName, string password, Result<bool> result)
		{
			BasePlug.At("_session").Post(DreamMessage.Ok(MimeType.FORM_URLENCODED,String.Format("name={0}&password={1}",userName,password)), new Result<DreamMessage>(TimeSpan.FromSeconds(3))).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
					{
						BasePlug.CookieJar.Update(a.Cookies, new XUri(BasePlug.Uri.SchemeHostPort));
						BasePlug = BasePlug.WithHeader("X-CouchDB-WWW-Authenticate", "Cookie");
						result.Return(true);
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

		//protected Cookie GetSession()
		//{
		//    if (string.IsNullOrEmpty(username)) return null;
		//    var request = new CouchRequest(baseUri + "_session");
		//    var response = request.Post()
		//        .ContentType("application/x-www-form-urlencoded")
		//        .Data("name=" + username + "&password=" + password)
		//        .GetResponse();

		//    var header = response.Headers.Get("Set-Cookie");
		//    if (header != null)
		//    {
		//        var parts = header.Split(';')[0].Split('=');
		//        var authCookie = new Cookie(parts[0], parts[1]);
		//        authCookie.Domain = response.Server;
		//        return authCookie;
		//    }
		//    return null;
		//}
		//protected CouchRequest GetRequest(string uri)
		//{
		//    return GetRequest(uri, null);
		//}
		//protected CouchRequest GetRequest(string uri, string etag)
		//{
		//    var request = new CouchRequest(uri, GetSession(), etag);
		//    return request;
		//}
    }
}