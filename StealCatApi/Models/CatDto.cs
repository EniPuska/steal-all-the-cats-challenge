namespace StealCatApi.Models
{
    //We use this model when we fetch Data from the Caas API
    public class CatDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<CatBreed> Breeds { get; set; }
    }

    public class CatBreed
    {
        public string Name { get; set; }
        public string Temperament { get; set; }
    }
}
