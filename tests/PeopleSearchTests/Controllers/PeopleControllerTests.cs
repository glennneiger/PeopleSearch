using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PeopleSearch.Controllers;
using PeopleSearch.Data.Entities;
using PeopleSearch.Data.Services;

namespace PeopleSearchTests.Controllers
{
    [TestClass]
    public class PeopleControllerTests : TestBase
    {
        //private Mock<ITelemetryService> mockTelemetryService;
        private PeopleDataController controller;
        private Mock<ILogger<PeopleDataController>> mockLogger;
        private Mock<IPeopleService> mockPeopleService;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestBase.Initialize(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestBase.Cleanup();
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            mockLogger = new Mock<ILogger<PeopleDataController>>();

            mockPeopleService = new Mock<IPeopleService>();

            controller = new PeopleDataController(mockPeopleService.Object, mockLogger.Object);

            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Headers).Returns(new Mock<IHeaderDictionary>().Object);

            var context = new Mock<HttpContext>();
            context.SetupGet(c => c.Request).Returns(request.Object);

            controller.ControllerContext = new ControllerContext() { HttpContext = context.Object };
        }
               
        [TestMethod]
        public async Task FindPeople_Success()
        {
            mockPeopleService.Setup(p => p.FindAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
           .ReturnsAsync(new List<Person>{
                new Person { Id = 1, FirstName = "Kelly", LastName = "Smith", PhoneNumber = "(759) 548-2082" },
                new Person { Id = 2, FirstName = "Cynthia", LastName = "Smith", PhoneNumber = "(328) 410-9209" },
                new Person { Id = 3, FirstName = "Emily", LastName = "Smith", PhoneNumber = "(480) 463-3223" },
                new Person { Id = 4, FirstName = "Kevin", LastName = "Smith", PhoneNumber = "(711) 205-1976" },
                new Person { Id = 5, FirstName = "Jeremy", LastName = "Smith", PhoneNumber = "(432) 371-8303" },
                new Person { Id = 6, FirstName = "Callie", LastName = "Smith", PhoneNumber = "(623) 637-7015" },
                new Person { Id = 7, FirstName = "Johanna", LastName = "Smith", PhoneNumber = "(277) 626-7039" },
                new Person { Id = 8, FirstName = "Maggie", LastName = "Smith", PhoneNumber = "(960) 261-9175" },
                new Person { Id = 9, FirstName = "Adrian", LastName = "Smith", PhoneNumber = "(913) 684-8349" },
                new Person { Id = 10, FirstName = "Erik", LastName = "Smith", PhoneNumber = "(627) 580-8133" }
           });

            var result = await this.controller.FindPeople("Smith", 0, 10, false);

            mockPeopleService.Verify(m => m.FindAsync("Smith", 0, 10));

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)result).Value.Should().NotBeNull();
            ((OkObjectResult)result).Value.Should().BeOfType<List<Person>>();

            var value = ((OkObjectResult)result).Value as IEnumerable<Person>;
            value.Should().HaveCount(10);
        }
    }
}
