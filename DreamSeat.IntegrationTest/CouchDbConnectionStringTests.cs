using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamSeat.Support;
using NUnit.Framework;

namespace DreamSeat.IntegrationTest
{
	[TestFixture]
	class CouchDbConnectionStringTests
	{
		[Test]
		public void CouchDbConnectionStringBuilder()
		{
			CouchDbConnectionStringBuilder c = new CouchDbConnectionStringBuilder("Host=test;port=10;username=un;Password=coucou;SslEnabled=true");
			Assert.AreEqual("test", c.Host);
			Assert.AreEqual("un", c.UserName);
			Assert.AreEqual(10, c.Port);
			Assert.AreEqual("coucou", c.Password);
			Assert.AreEqual(true,c.SslEnabled);
		}
		[Test]
		public void CouchDbConnectionStringBuilderDefaultValues()
		{
			CouchDbConnectionStringBuilder c = new CouchDbConnectionStringBuilder(String.Empty);
			Assert.AreEqual("localhost", c.Host);
			Assert.AreEqual(String.Empty, c.UserName);
			Assert.AreEqual(5984, c.Port);
			Assert.AreEqual(String.Empty, c.Password);
			Assert.AreEqual(false, c.SslEnabled);
		}
	}
}
