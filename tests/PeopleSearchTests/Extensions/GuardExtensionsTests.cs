using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using PeopleSearch.Extentions;

namespace PeopleSearchTests.Extensions
{
    [TestClass]
    public class GuardExtensionsTests
    {
        [TestMethod]
        public void GuardNull_Null_ArgumentNullException()
        {
            var testValue = (string)null;
            Func<Task> func = () => {
                testValue.GuardNull(nameof(testValue));
                return Task.CompletedTask;
            };
            func.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GuardNull_String_Success()
        {
            var testValue = "a value";
            Func<Task> func = () => {
                testValue.GuardNull(nameof(testValue));
                return Task.CompletedTask;
            };
            func.Should().NotThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void GuardEmpty_EmptyString_ArgumentException()
        {
            var testValue = string.Empty;
            Func<Task> func = () => {
                testValue.GuardEmpty(nameof(testValue));
                return Task.CompletedTask;
            };
            func.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void GuardEmpty_String_Success()
        {
            var testValue = "a value";
            Func<Task> func = () => {
                testValue.GuardEmpty(nameof(testValue));
                return Task.CompletedTask;
            };
            func.Should().NotThrow<ArgumentException>();
        }
    }
}
