using System;
using Microsoft.AspNetCore.Mvc;
using PeopleSearch.Models;

namespace SiriusIQ.Web.MigrationPortal.Controllers
{
    [Route("health")]
    public class HealthController : Controller
    {
        private const string TestErrorMessage = "this is a test exception.";
        private readonly EnvironmentConfig environmentConfig;

        public HealthController(EnvironmentConfig environmentConfig) => this.environmentConfig = environmentConfig;

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            var result = new
            {
                Name = "People Search Web.API",
                Type = this.environmentConfig.Name,
                TimeStamp = DateTime.UtcNow,
                Version = this.GetType().Assembly.GetName().Version.ToString()
            };

            return this.Ok(result);
        }

        [HttpGet]
        [Route("throw/{errorcode:int}")]
        public IActionResult Throw(int errorCode)
        {
            if (errorCode > 0)
            {
                return StatusCode(errorCode, TestErrorMessage);
            }
            throw new Exception(TestErrorMessage);
        }
    }
}
