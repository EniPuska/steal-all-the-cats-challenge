using Newtonsoft.Json;
using StealCat.Data;
using StealCat.Data.Entities;
using StealCat.Repositories.Interfaces;
using StealCatApi.Models;

namespace StealCatApi.Services
{
    public class CatsService
    {
        private readonly ICatRepository _catRepository;
        private readonly ITagRepository _tagRepository;
        private readonly HttpClient _httpClient;
        private readonly CatDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public CatsService(IHttpClientFactory httpClientFactory, CatDbContext context, IConfiguration configuration, ICatRepository catRepository, ITagRepository tagRepository)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _configuration = configuration;
            _apiKey = configuration["CaaSApi:ApiKey"];
            _baseUrl = configuration["CaaSApi:BaseUrl"];
            _catRepository = catRepository;
            _tagRepository = tagRepository;
        }

        public async Task<List<CatDto>> FetchCats()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/images/search?limit=25");
            request.Headers.Add("x-api-key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var catDtos = JsonConvert.DeserializeObject<List<CatDto>>(content);

            // Download all images in parallel
            var downloadTasks = catDtos.Select(async catDto =>
            {
                var imageBytes = await DownloadImageAsync(catDto.Url);
                return new { CatDto = catDto, Image = imageBytes };
            }).ToList();

            var downloadedCats = await Task.WhenAll(downloadTasks);

            foreach (var downloadedCat in downloadedCats)
            {
                var catDto = downloadedCat.CatDto;
                var imageBytes = downloadedCat.Image;

                var existingCat = await _catRepository.GetByCatIdAsync(catDto.Id);

                if (existingCat == null)
                {
                    var catEntity = new Cat
                    {
                        CatId = catDto.Id,
                        Image = imageBytes,
                        Width = catDto.Width,
                        Height = catDto.Height,
                        Created = DateTime.UtcNow
                    };

                    if (catDto.Breeds.Any())
                    {
                        foreach (var breed in catDto.Breeds)
                        {
                            var tags = breed.Temperament.Split(',').Select(t => t.Trim());

                            foreach (var tag in tags)
                            {
                                var tagEntity = await _tagRepository.GetTagByNameAsync(tag) ?? new Tag { Name = tag, Created = DateTime.UtcNow };

                                catEntity.CatTags.Add(new CatTag
                                {
                                    Cat = catEntity,
                                    Tag = tagEntity
                                });

                                if (tagEntity.Id == 0)
                                {
                                    await _tagRepository.AddTagAsync(tagEntity);
                                }
                            }
                        }
                    }

                    await _catRepository.AddAsync(catEntity);
                }
            }

            await _context.SaveChangesAsync();
            return catDtos;
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            ValidationService.ValidateImageUrl(imageUrl);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<DbCatsDto> GetCatByIdAsync(int id)
        {
            await ValidationService.ValidateCatExists(_catRepository, id);

            Cat cat = await _catRepository.GetCatById(id);

            if (cat == null)
            {
                return null;
            }

            var catDto = new DbCatsDto
            {
                Id = cat.CatId,
                Image = cat.Image,
                Width = cat.Width,
                Height = cat.Height,
                Tags = cat.CatTags.Select(ct => new Tags
                {
                    Name = ct.Tag.Name
                }).ToList()
            };

            return catDto;
        }

        public async Task<List<DbCatsDto>> GetCatsWithPagingAsync(int page, int pageSize, string tag)
        {
            ValidationService.ValidatePaging(page, pageSize);

            var skip = (page - 1) * pageSize;

            List<Cat> cats = new List<Cat>();

            if (string.IsNullOrEmpty(tag))
                cats = await _catRepository.GetCatsAsync(skip, pageSize, null);
            else
                cats = await _catRepository.GetCatsAsync(skip, pageSize, tag);

            if (cats == null || !cats.Any())
            {
                return null;
            }

            var catDtos = cats.Select(cat => new DbCatsDto
            {
                Id = cat.CatId,
                Image = cat.Image,
                Width = cat.Width,
                Height = cat.Height,
                Tags = cat.CatTags.Select(tag => new Tags 
                { 
                    Name = tag.Tag.Name 
                }).ToList()
            }).ToList();

            return catDtos;
        }
    }
}
