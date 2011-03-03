using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ContactManager
{
	static class Settings
	{
		public static readonly string Host = ConfigurationManager.AppSettings["Host"];
		public static readonly int Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
		public static readonly string Username = ConfigurationManager.AppSettings["UserName"];
		public static readonly string Password = ConfigurationManager.AppSettings["Password"];
		public static readonly string DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
	}
}
