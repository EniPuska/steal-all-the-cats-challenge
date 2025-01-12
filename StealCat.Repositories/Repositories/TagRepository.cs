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
    public class TagRepository : ITagRepository
    {
        private readonly CatDbContext _context;

        public TagRepository(CatDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task AddTagAsync(Tag tagEntity)
        {
            await _context.Tags.AddAsync(tagEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task AddRangeAsync(List<Tag> tags)
        {
            await _context.Tags.AddRangeAsync(tags);
        }
    }
}
