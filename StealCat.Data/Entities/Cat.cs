using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCat.Data.Entities
{
    public class Cat
    {
        public int Id { get; set; } 
        public string CatId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] Image { get; set; } 
        public DateTime Created { get; set; }
        public ICollection<CatTag> CatTags { get; set; } = new List<CatTag>();
    }
}
