using Microsoft.EntityFrameworkCore;
using StealCat.Data;
using StealCat.Data.Entities;
using StealCat.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCat.Repositories.Repositories
{
    public class CatRepository : ICatRepository
    {
        private readonly CatDbContext _context;

        public CatRepository(CatDbContext context)
        {
            _context = context;
        }

        public async Task<Cat> GetByCatIdAsync(string catId)
        {
            return await _context.Cats.FirstOrDefaultAsync(c => c.CatId == catId);
        }

        public async Task AddAsync(Cat catEntity)
        {
            await _context.Cats.AddAsync(catEntity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Cat> GetCatById(int id)
        {
            return await _context.Cats
                .Include(cat => cat.CatTags)
                .ThenInclude(catTag => catTag.Tag)
                .FirstOrDefaultAsync(cat => cat.Id == id);  
        }

        public async Task<List<Cat>> GetCatsAsync(int skip, int take, string tag)
        {
            var query = _context.Cats.AsQueryable();

            // If a tag is provided, filter by that tag
            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(c => c.CatTags.Any(ct => ct.Tag.Name == tag));
            }

            // Apply the include for related tags
            query = query.Include(c => c.CatTags)
                         .ThenInclude(ct => ct.Tag);

            // Apply pagination
            query = query.Skip(skip)
                         .Take(take);

            return await query.ToListAsync();
        }
    }
}
