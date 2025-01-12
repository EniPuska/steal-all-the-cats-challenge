namespace StealCatApi.Models
{
    public class DbCatsDto
    {
        public string Id { get; set; }
        public byte[] Image { get; set; }
        public List<Tags> Tags { get; set; }
    }

    public class Tags
    {
        public string Name { get; set; }
    }
}
