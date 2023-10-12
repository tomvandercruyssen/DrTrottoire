using DrTrottoirApi.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrTrottoirApi.Tests.Validators
{
    [TestClass]
    public class TestKboValidator
    {
        [TestMethod]
        [DataRow("1234", false)]
        [DataRow("23455.53532.3432423", false)]
        [DataRow("23455.535323432423", false)]
        [DataRow("23455.5.35323432423", false)]
        [DataRow("23455.53532343242.3", false)]
        [DataRow("2345.535.3299", false)]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("1234.567.890", true)]
        [DataRow("1222.537.810", true)]
        public void IsValid_ShouldReturnExpected(string kbo, bool expected)
        {
            Assert.AreEqual(expected, KboValidator.IsValid(kbo));
        }
    }
}
