using Moq;
using Xunit;
using StealCatApi.Controllers;
using StealCatApi.Services;
using StealCat.Repositories.Interfaces;
using StealCat.Data.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StealCat.Data;
using StealCatApi.Models;
using Microsoft.EntityFrameworkCore; // Add this namespace for UseInMemoryDatabase

public class CatsServiceTests
{
    private readonly CatsService _catsService;
    private readonly Mock<ICatRepository> _mockCatRepo;
    private readonly Mock<ITagRepository> _mockTagRepo;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public CatsServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

        _mockCatRepo = new Mock<ICatRepository>();
        _mockTagRepo = new Mock<ITagRepository>();

        var options = new DbContextOptionsBuilder<CatDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") 
            .Options;

        var dbContextMock = new CatDbContext(options);

        // Create the CatsService instance with mocked dependencies
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["CaaSApi:ApiKey"]).Returns("live_NOZGGQTnPw8cah5JIK1ZsuzyRHkXehs4bZPBgTcWu2b0yqhVa47uA9FwuFzsumzqe");
        configuration.Setup(c => c["CaaSApi:BaseUrl"]).Returns("https://api.thecatapi.com/v1");

        _catsService = new CatsService(
            _mockHttpClientFactory.Object,
            dbContextMock, // Use real DbContext here
            configuration.Object,
            _mockCatRepo.Object,
            _mockTagRepo.Object
        );
    }

    [Fact]
    public async Task GetCatsWithPagingAsync_ReturnsCats_WhenCatsExist()
    {
        // Arrange
        var catDtos = new List<CatDto>
        {
            new CatDto { Id = "1", Url = "http://cat.com/1.jpg" }
        };

        _mockCatRepo.Setup(repo => repo.GetCatsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Cat> { new Cat { CatId = "1", Image = new byte[] { } } });

        var result = await _catsService.GetCatsWithPagingAsync(1, 10, "");

        Assert.NotNull(result);
        Assert.Single(result);  
    }
}
