using EffectiveMobile;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using Xunit;

namespace TestProject.Integration;

public class AdvertisementIntegrationTests : IClassFixture<WebApplicationFactory<EffectiveMobile.Program>>
{
    private readonly WebApplicationFactory<EffectiveMobile.Program> _factory;

    public AdvertisementIntegrationTests(WebApplicationFactory<EffectiveMobile.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });
    }

    /// <summary>
    /// Интеграционный тест, проверяющий что endpoint поиска рекламных площадок
    /// возвращает статус OK при корректном запросе
    /// </summary>
    [Fact]
    public async Task SearchAdvertisements_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/Advertisement/search?location=/ru/msk");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// Интеграционный тест, проверяющий что endpoint загрузки рекламных площадок
    /// возвращает статус OK при корректном запросе с файлом
    /// </summary>
    [Fact]
    public async Task LoadAdvertisements_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Яндекс.Директ:/ru"));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", "test.txt");

        // Act
        var response = await client.PostAsync("/api/Advertisement/load", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}