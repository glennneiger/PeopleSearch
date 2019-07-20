using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeopleSearch.Data.Contexts;
using PeopleSearch.Data.Entities;
using PeopleSearch.Data.Services;

namespace PeopleSearchTests.Data.Services
{
    [TestClass]
    public class PeopleServiceTests : TestBase
    {
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

        [TestMethod]
        public async Task ListAsync_Success()
        {
            var connection = await GetOpenSqliteConnectionAsync();

            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                await SeedDataAsync(options);

                using (var context = new PeopleContext(options))
                {
                    var service = new PeopleService(context);

                    var people = await service.ListAsync();

                    people.Should().NotBeNullOrEmpty();
                    people.Should().HaveCount(5);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [TestMethod]
        public async Task GetAsync_Id_CorrectPerson()
        {
            var id = 2;
            var connection = await GetOpenSqliteConnectionAsync();
            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                await SeedDataAsync(options);

                using (var context = new PeopleContext(options))
                {
                    var service = new PeopleService(context);

                    var person = await service.GetAsync(id);

                    person.Should().NotBeNull();
                    person.Id.Should().Be(id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [TestMethod]
        public async Task Delete_NegativeId_Failure()
        {
            var connection = await GetOpenSqliteConnectionAsync();
            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                await SeedDataAsync(options);

                using (var context = new PeopleContext(options))
                {
                    var service = new PeopleService(context);

                    Func<Task> func = () => service.DeleteAsync(-1);

                    func.Should().Throw<ArgumentException>();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [TestMethod]
        public async Task Add_IdZero_IdAssigned()
        {
            var connection = await GetOpenSqliteConnectionAsync();

            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                using (var context = new PeopleContext(options))
                {
                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    var service = new PeopleService(context);

                    service.Add(new Person
                    {
                        Id = 0,
                        FirstName = "Andrew",
                        LastName = "Skoraro",
                        PhoneNumber = "(759) 548-2082"
                    });

                    await service.SaveAsync();
                    var people = await service.ListAsync();

                    people.Should().NotBeNullOrEmpty();
                    people.Should().HaveCount(1);
                    people.First().Id.Should().BeGreaterThan(0);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [TestMethod]
        public async Task Add_NoFirstName_Failure()
        {
            var connection = await GetOpenSqliteConnectionAsync();

            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                using (var context = new PeopleContext(options))
                {
                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    var service = new PeopleService(context);

                    service.Add(new Person
                    {
                        Id = 0,
                        LastName = "Smith",
                        PhoneNumber = "(555) 555-5555"
                    });

                    var people = await service.ListAsync();

                    Func<Task> func = () => service.SaveAsync();
                    
                    func.Should().Throw<DbUpdateException>();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [TestMethod]
        public async Task Update_LastName_Success()
        {
            var id = 3;
            var connection = await GetOpenSqliteConnectionAsync();

            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                using (var context = new PeopleContext(options))
                {
                    await SeedDataAsync(options);

                    var service = new PeopleService(context);

                    var person = await service.GetAsync(id);
                    person.FirstName = "Matt";
                    person.LastName = "Segedi";

                    await service.UpdateAsync(person);
                    await service.SaveAsync();
                }

                using (var context = new PeopleContext(options))
                {
                    var service = new PeopleService(context);
                    var people = await service.ListAsync();

                    people.Should().NotBeNullOrEmpty();
                    people.Should().HaveCount(5);
                    var person = people.SingleOrDefault(p => p.Id == id);
                    person.Should().NotBeNull();
                    person.LastName.Should().Be("Segedi");
                }
            }
            finally
            {
                connection.Close();
            }
        }
        
        [TestMethod]
        public async Task Delete_Success()
        {
            var id = 4;
            var connection = await GetOpenSqliteConnectionAsync();
            try
            {
                var options = new DbContextOptionsBuilder<PeopleContext>()
                           .UseSqlite(connection)
                           .Options;

                await SeedDataAsync(options);

                using (var context = new PeopleContext(options))
                {
                    var service = new PeopleService(context);
                    await service.DeleteAsync(id);
                    await service.SaveAsync();
                }

                using (var context = new PeopleContext(options))
                {
                    var service = new PeopleService(context);
                    var people = await service.ListAsync();

                    people.Should().NotBeNullOrEmpty();
                    people.Should().HaveCount(4);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private async Task SeedDataAsync(DbContextOptions<PeopleContext> options)
        {
            using (var context = new PeopleContext(options))
            {
                context.Database.EnsureCreated();

                await context.People.AddRangeAsync
                (
                    new Person { Id = 1, FirstName = "Andrew", LastName = "Skoraro", PhoneNumber = "(759) 548-2082" },
                    new Person { Id = 2, FirstName = "Cynthia", LastName = "Matthews", PhoneNumber = "(328) 410-9209" },
                    new Person { Id = 3, FirstName = "Emily", LastName = "Curry", PhoneNumber = "(480) 463-3223" },
                    new Person { Id = 4, FirstName = "Kevin", LastName = "Luna", PhoneNumber = "(711) 205-1976" },
                    new Person { Id = 5, FirstName = "Jeremy", LastName = "Pearson", PhoneNumber = "(432) 371-8303" }
                );

                await context.SaveChangesAsync();
            }
        }

        private async Task<SqliteConnection> GetOpenSqliteConnectionAsync()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            return connection;
        }
    }
}