using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PeopleSearch.Data.Entities;
using PeopleSearch.Data.Services;
using PeopleSearch.Extentions;

namespace PeopleSearch.Controllers
{
    [Route("people")]
    public class PeopleController : Controller
    {
        private const string SystemError = "The system is unable to access people data at this time. Please contact support if this issue persists.";
        private const string NotFoundMessage = "Information not found.";
        private const string UpdateErrorMessage = "An error occurred while attempting to update a person.";

        private readonly IPeopleService peopleService;
        private readonly ILogger logger;

        public PeopleController(IPeopleService peopleService, ILogger<PeopleController> logger)
        {
            this.peopleService = peopleService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetById")]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetById([FromRoute] int id)
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
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to retrieve a person. ID: {id}");
                return StatusCode(500, new { Message = SystemError });
            }

        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(IEnumerable<Person>), 200)]
        [ProducesResponseType(400)]
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
        [Route("find/{search}")]
        [ProducesResponseType(typeof(IEnumerable<Person>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> FindPeople([FromRoute] string search, [FromQuery] int skip, [FromQuery] int take)
        {
            try
            {
                var people = await peopleService.FindAsync(search, skip, take);

                return this.Ok(people);
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to list people. URL: {Request.GetEncodedPathAndQuery()}");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Person), 201)]
        [ProducesResponseType(400)]
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

                var uri = this.Url.Link("GetById", new { id = person.Id });

                return this.Created(uri, person);
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"An error occurred while attempting to create a person. URL: {Request.GetEncodedPathAndQuery()}, Person:  {person.ToJson()}.");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(Person), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"{UpdateErrorMessage} URL: {Request.GetEncodedPathAndQuery()}, Person:  {person.ToJson()}.");
                return StatusCode(500, new { Message = SystemError });
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(Person), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePerson([FromBody] Person person)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest(this.ModelState);
                }

                logger.LogInformation(1100, $"Updating person.");

                await peopleService.DeleteAsync(person.Id);
                await peopleService.SaveAsync();

                var uri = this.Url.Link("GetById", new { id = person.Id });

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(1500, ex, $"{UpdateErrorMessage} URL: {Request.GetEncodedPathAndQuery()}, Person:  {person.ToJson()}.");
                return StatusCode(500, new { Message = SystemError });
            }
        }
    }
}
