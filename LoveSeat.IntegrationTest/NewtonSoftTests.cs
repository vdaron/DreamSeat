using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if NUNIT
using NUnit.Framework;
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
    public class NewtonSoftTests
    {
		public class User
		{
			public string FirstName { get; set; }
			public string LastName { get; set; }

			public User(JObject obj)
			{ 
				FirstName = "default"; 
			}
		}

		[Test]
		public void SouldSerializeWithoutDefaultConstructor()
		{
			User u = new User(new JObject()) { LastName = "Daron" };
			string ujson = JsonConvert.SerializeObject(u);

			User u2 = JsonConvert.DeserializeObject<User>(ujson);
			Assert.AreSame(u.FirstName, u2.FirstName);

		}
        [Test]
        public void JArray_Should_Support_Complex_Types()
        {
            var arry = new JArray();
            arry.Add(1);
            arry.Add("abc");
            var result = arry.ToString(Formatting.None);
            Assert.AreEqual("[1,\"abc\"]", result);
        }
        [Test]
        public void KeyOptions_Should_Produce_Single_Value_For_A_Single_Array()
        {
            var arry = new KeyOptions();
            arry.Add(1);
            var result = arry.ToString();
            Assert.AreEqual("1", result);
            
        }
        [Test]
        public void KeyOptions_Should_Produce_A_Complex_Array_For_Multiple_Values()
        {
            var arry = new KeyOptions();
            arry.Add(1);
            arry.Add(new DateTime(2011,1,1));
            var result = arry.ToString();
            Assert.AreEqual("[1,\"2011-01-01T00:00:00\"]", result);
        }

        [Test]
        public void KeyOptions_Should_Produce_Squirley_Brackets_for_CouchValueEmpty()
        {
            var arry = new KeyOptions();
            arry.Add(CouchValue.Empty);
            arry.Add(1);
            var result = arry.ToString();
            Assert.AreEqual("[{},1]", result);
        }

        [Test]
        public void KeyOptions_Should_Produce_IsoTime()
        {
            var arry = new KeyOptions();
            arry.Add(CouchValue.Empty);
            arry.Add(new DateTime(2011,1,1));
            var result = arry.ToString();
            Assert.AreEqual("[{},\"2011-01-01T00:00:00\"]", result);
    
        }
    }
}