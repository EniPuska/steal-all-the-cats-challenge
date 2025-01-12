using StealCat.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCat.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> GetTagByNameAsync(string name);
        Task AddTagAsync(Tag tagEntity);
        Task<List<Tag>> GetAllTagsAsync();
        Task AddRangeAsync(List<Tag> tags);
    }
}
