using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrTrottoirApi.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DrTrottoirApi.Tests.Validators
{
    [TestClass]
    public class TestRoleValidator
    {
        [TestMethod]
        [DataRow(new[] { "Student", "Unknown" }, false)]
        [DataRow(new[] { "" }, false)]
        [DataRow(new[] { "dsfdsdf", "Unknown" }, false)]
        [DataRow(new[] { "sdfsdf", "Student" }, false)]

        [DataRow(new[] { "Student", "SuperStudent" }, true)]
        [DataRow(new[] { "Student", "Admin" }, true)]
        [DataRow(new[] { "Admin" }, true)]
        [DataRow(new[] { "Student" }, true)]
        public void CorrectRoles_ShouldReturnExpected(string[] roles, bool expected)
        {
            Assert.AreEqual(expected, RoleValidator.CorrectRoles(roles.ToList()));
        }
    }
}
