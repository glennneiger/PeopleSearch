using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PeopleSearch.Data.Entities;
using PeopleSearch.Data.Services;
using PeopleSearch.Extentions;
using PeopleSearch.Models;

namespace PeopleSearch.Controllers
{
    [Route("people/data")]
    public class PeopleDataController : Controller
    {
        private const string SystemError = "The system is unable to access people data at this time. Please contact support if this issue persists.";
        private const string NotFoundMessage = "Information not found.";
        private const string UpdateErrorMessage = "An error occurred while attempting to update a person.";

        private readonly IPeopleService peopleService;
        private readonly ILogger logger;

        public PeopleDataController(IPeopleService peopleService, ILogger<PeopleDataController> logger)
        {
            this.peopleService = peopleService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task ListPeople([FromQuery] int skip, [FromQuery] int take)
        {
            try
            {
                var people = await peopleService.ListAsync(skip, take);

                Response.Headers.Add("Content-Type", "text/event-stream");

                foreach (var result in people)
                {
                    await Response.WriteAsync(result.ToJson());

                    Response.Body.Flush();
                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to list people. URL: {Request.GetEncodedPathAndQuery()}");
                Response.StatusCode = 500;
                Response.ContentType = "application/json";
                await Response.WriteAsync((new { Message = SystemError }).ToJson());
            }
        }

        [HttpGet]
        [Route("find")]
        [ProducesResponseType(typeof(IEnumerable<Person>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FindPeople([FromQuery] string search, [FromQuery] int skip, [FromQuery] int take, [FromQuery] bool goSlow)
        {
            try
            {
                var people = await peopleService.FindAsync(search, skip, take);
                if (goSlow)
                {
                    Thread.Sleep(people.Count() * 300);
                }
                return this.Ok(people);
            }
            catch (ArgumentException)
            {
                return this.BadRequest($"Invalid data.");
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to list people. URL: {Request.GetEncodedPathAndQuery()}");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpGet]
        [Route("stats")]
        [ProducesResponseType(typeof(Stats), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PeopleStats()
        {
            try
            {
                var countTask = peopleService.CountAsync();
                var nameStatsTask = peopleService.NameStatsAsync();
                await Task.WhenAll(countTask, nameStatsTask);

                var stats = new Stats()
                {
                    TotalPeople = await countTask,
                    NameStats = await nameStatsTask
                };

                return this.Ok(stats);
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to list people. URL: {Request.GetEncodedPathAndQuery()}");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpGet]
        [Route("count")]
        [ProducesResponseType(typeof(Stats), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PeopleCount([FromQuery] string search)
        {
            try
            {
                var count = 0;

                if (string.IsNullOrWhiteSpace(search))
                {
                    count = await peopleService.CountAsync();
                }
                else
                {
                    count = await peopleService.CountAsync(search);
                }

                return this.Ok(count);
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to list people. URL: {Request.GetEncodedPathAndQuery()}");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetPerson")]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] 
        public async Task<IActionResult> GetPerson([FromRoute] int id)
        {
            try
            {
                var person = await peopleService.GetAsync(id);

                if (person == null)
                {
                    logger.LogWarning(1404, $"The person with id '{id}' was not found.");
                    return NotFound(new { message = NotFoundMessage });
                }

                return this.Ok(person);
            }
            catch (ArgumentException)
            {
                return this.BadRequest($"Invalid data.");
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to retrieve a person. ID: {id}");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Person), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostPerson([FromBody] Person person)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                logger.LogInformation(1100, $"Creating person.");

                peopleService.Add(person);
                await peopleService.SaveAsync();

                var uri = this.Url.Link("GetPerson", new { id = person.Id });

                return this.Created(uri, person);
            }
            catch (ArgumentException)
            {
                return this.BadRequest($"Invalid data.");
            }
            catch (DbUpdateException)
            {
                return this.BadRequest($"User with name {person.LastName}, {person.FirstName} already exists.");
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to create a person. URL: {Request.GetEncodedPathAndQuery()}, Person:  {person.ToJson()}.");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutPerson([FromBody] Person person)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                logger.LogInformation(1100, $"Updating person.");

                await peopleService.UpdateAsync(person);
                await peopleService.SaveAsync();

                var uri = this.Url.Link("GetById", new { id = person.Id });

                return NoContent();
            }
            catch (ArgumentException)
            {
                return this.BadRequest($"Invalid data.");
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"{UpdateErrorMessage} URL: {Request.GetEncodedPathAndQuery()}, Person:  {person.ToJson()}.");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePerson([FromRoute] int id)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                logger.LogInformation(1100, $"Deleting person.");

                await peopleService.DeleteAsync(id);
                await peopleService.SaveAsync();

                return NoContent();
            }
            catch (ArgumentException)
            {
                return this.BadRequest($"Invalid data.");
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"{UpdateErrorMessage} URL: {Request.GetEncodedPathAndQuery()}, Person ID:  {id}.");
                return StatusCode(500, new { Message = SystemError });
            }
        }
    }
}
