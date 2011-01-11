using LoveSeat.Interfaces;
using LoveSeat.Support;
using Moq;

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
    public class PagingHelperTest
    {
        [Test]
        public void Should_Show_Previous_If_OffSet_Not_Equal_Zero()
        {
            var result = new Mock<IViewResult>();
            result.ExpectGet(x => x.OffSet).Returns(1);
            var options = new ViewOptions();
            var model = new PageableModel();
            model.UpdatePaging(options, result.Object);
             Assert.IsTrue(model.ShowPrev);        
        }
        [Test]
        public void Should_Not_Show_Previous_If_Offset_Equal_Zero()
        {
            var result = new Mock<IViewResult>();
            result.ExpectGet(x => x.OffSet).Returns(0);
            var options = new ViewOptions();
            var model = new PageableModel();
            model.UpdatePaging(options, result.Object);
            Assert.IsFalse(model.ShowPrev);        
        }
        [Test]
        public void Should_Show_Next_Unless_Offset_Plus_Limit_Gte_Count()
        {
            var result = new Mock<IViewResult>();
            result.ExpectGet(x => x.OffSet).Returns(5);
            var options = new ViewOptions();
            options.Limit = 5;
            result.ExpectGet(x => x.TotalRows).Returns(10);
            var model = new PageableModel();
            model.UpdatePaging(options, result.Object);
            Assert.IsFalse(model.ShowNext);
        }

        [Test]
        public void Should_Skip_1_On_Next_Url_When_Offset_is_Not_Zero()
        {
            var result = new Mock<IViewResult>();
            result.ExpectGet(x => x.OffSet).Returns(1);
            var options = new ViewOptions();
            var model = new PageableModel();
            model.UpdatePaging(options, result.Object);
            Assert.IsTrue(model.PrevUrlParameters.Contains("skip=1"));
            Assert.IsTrue(model.PrevUrlParameters.Contains("descending=true"));
        }
       


    }
}