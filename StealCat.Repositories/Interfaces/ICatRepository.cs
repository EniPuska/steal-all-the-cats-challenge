using StealCat.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCat.Repositories.Interfaces
{
    public  interface ICatRepository
    {
        Task<Cat> GetByCatIdAsync(string catId);
        Task AddAsync(Cat catEntity);
        Task SaveChangesAsync();
        Task<Cat> GetCatById(int id);
        Task<List<Cat>> GetCatsAsync(int skip, int take, string tag);
    }
}
