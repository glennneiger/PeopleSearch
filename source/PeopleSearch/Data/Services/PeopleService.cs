using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PeopleSearch.Data.Contexts;
using PeopleSearch.Data.Entities;
using PeopleSearch.Extentions;

namespace PeopleSearch.Data.Services
{
    public class PeopleService : IPeopleService
    {
        private PeopleContext context;

        public PeopleService(PeopleContext context)
        {
            context.GuardNull(nameof(context));

            this.context = context;
        }

        public async Task<IEnumerable<Person>> FindAsync(string searchString, int skip = 0, int take = 10)
        {
            searchString.GuardEmpty(nameof(searchString));

            return await context.People
                .Where(p => EF.Functions.Like(p.LastName, $"%{searchString}%") || EF.Functions.Like(p.FirstName, $"%{searchString}%"))
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip(skip).Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> ListAsync(int skip = 0, int take = 10)
        {
            return await context.People
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Person> GetAsync(int id)
        {
            return await context.People
                .Where(p => p.Id == id)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .FirstOrDefaultAsync();
        }

        public void Add(Person person)
        {
            person.GuardNull(nameof(person));

            context.Add(person);
        }

        public async Task UpdateAsync(Person person)
        {
            person.GuardNull(nameof(person));

            if (person.Id < 1)
            {
                throw new ArgumentException($"{nameof(person.Id)} must be greater than 0.");
            }

            var exists = await ExistsAsync(person.Id);
            if (!exists)
            {
                throw new ArgumentException("Person not found.");
            }

            context.Entry(person).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException($"{nameof(id)} must be greater than 0.");
            }

            var person = await GetAsync(id);
            if (person == null)
            {
                throw new ArgumentException("Person not found.");
            }

            context.Remove(person);
        }

        public async Task<int> CountAsync()
        {
            return await context.People.CountAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return (await context.SaveChangesAsync()) > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.People.AnyAsync(e => e.Id == id);
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }
    }
}
