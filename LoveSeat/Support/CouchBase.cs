using System;
using System.Net;
using MindTouch.Dream;
using MindTouch.Tasking;

namespace LoveSeat.Support
{
    public abstract class CouchBase
    {
		protected Plug Plug;

		protected CouchBase(XUri baseUri)
		{
			Plug = Plug.New(baseUri);
		}
		protected CouchBase(XUri baseUri, string userName, string password)
		{
			Plug = Plug.New(baseUri).WithCredentials(userName, password);
		}

		public Result<bool> Authenticate(string userName, string password, Result<bool> result)
		{
			Plug.At("_session").Post(DreamMessage.Ok(MimeType.FORM_URLENCODED,String.Format("name={0}&password={1}",userName,password)), new Result<DreamMessage>(TimeSpan.FromSeconds(3))).WhenDone(
				a => {
					if (a.Status == DreamStatus.Ok)
					{
						Plug.CookieJar.Update(a.Cookies, new XUri(Plug.Uri.SchemeHostPort));
						Plug = Plug.WithHeader("X-CouchDB-WWW-Authenticate", "Cookie");
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