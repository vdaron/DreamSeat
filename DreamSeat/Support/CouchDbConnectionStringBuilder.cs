using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DreamSeat.Support
{
	public class CouchDbConnectionStringBuilder : DbConnectionStringBuilder
	{
		public CouchDbConnectionStringBuilder(string connectionString)
		{
			base.ConnectionString = connectionString;
		}

		public string Host
		{
			get { return GetValue("Host","localhost"); }
		}
		public string UserName
		{
			get { return GetValue("UserName",String.Empty); }
		}
		public string Database
		{
			get { return GetValue("Database", String.Empty); }
		}
		public string Password
		{
			get { return GetValue("Password", String.Empty); }
		}
		public int Port
		{
			get { return GetValue("Port", Constants.DEFAULT_PORT); }
		}
		public bool SslEnabled
		{
			get { return GetValue("SslEnabled", false); }
		}

		private string GetValue(string keyword, string defaultValue)
		{
			if(ContainsKey(keyword))
			{
				return (string) base[keyword];
			}
			
			return defaultValue;
		}

		private int GetValue(string keyword, int defaultValue)
		{
			string strVal = GetValue(keyword, null);
			if(strVal != null)
			{
				int val;
				if(int.TryParse(strVal, out val))
					return val;
			}
			return defaultValue;
		}

		private bool GetValue(string keyword, bool defaultValue)
		{
			string strVal = GetValue(keyword, null);
			if (strVal != null)
			{
				bool val;
				if (bool.TryParse(strVal, out val))
					return val;
			}
			return defaultValue;
		}

	}
}
