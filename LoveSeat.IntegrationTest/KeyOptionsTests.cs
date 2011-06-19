using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NUNIT
using NUnit.Framework;
using LoveSeat.Support;
using Newtonsoft.Json.Linq;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestFixtureSetUpAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute;
using TestFixtureTearDownAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute;
#endif

namespace LoveSeat.IntegrationTest
{
	[TestFixture]
	class KeyOptionsTests
	{
		[Test]
		public void EmptyKeyOptionsTest()
		{
			KeyOptions keyOptions = new KeyOptions();
			Assert.IsFalse(keyOptions.HasValues);
			Assert.IsTrue(keyOptions.Count == 0);
			Assert.AreEqual(String.Empty,keyOptions.ToString());
		}

		[Test]
		public void SingleStringValue()
		{
			KeyOptions keyOptions = new KeyOptions("somevalue");
			Assert.IsTrue(keyOptions.HasValues);
			Assert.AreEqual(1,keyOptions.Count);
			Assert.AreEqual("\"somevalue\"", keyOptions.ToString());
		}
		[Test]
		public void MultipleValues()
		{
			KeyOptions keyOptions = new KeyOptions("somevalue",1);
			Assert.IsTrue(keyOptions.HasValues);
			Assert.AreEqual(2, keyOptions.Count);
			Assert.AreEqual("[\"somevalue\",1]", keyOptions.ToString());
		}

		[Test]
		public void SubArrayTest()
		{
			KeyOptions keyOptions = new KeyOptions("somevalue",new JArray(1,2,3));
			Assert.AreEqual("[\"somevalue\",[1,2,3]]",keyOptions.ToString());
		}
		[Test]
		public void EmptyObjectTest()
		{
			KeyOptions keyOptions = new KeyOptions("somevalue",new JObject());
			Assert.AreEqual("[\"somevalue\",{}]", keyOptions.ToString());
		}
	}
}
