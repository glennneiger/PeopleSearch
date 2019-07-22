using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PeopleSearch.Data.Entities;

namespace PeopleSearch.Data.Services
{
    public interface IPeopleService: IDisposable
    {
        void Add(Person person);
        Task UpdateAsync(Person person);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
        Task<int> CountAsync(string searchString);
        Task<IEnumerable<Person>> FindAsync(string searchString, int skip = 0, int take = 10);
        Task<IEnumerable<Person>> ListAsync(int skip = 0, int take = 10);
        Task<Person> GetAsync(int id);
        Task<bool> SaveAsync();
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<NameStat>> NameStatsAsync();
    }
}