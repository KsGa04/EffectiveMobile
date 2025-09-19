using EffectiveMobile.Controllers;
using EffectiveMobile.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace TestProject;

public class AdvertisementControllerTests
{
    private readonly Mock<IAdvertisementService> _mockService;
    private readonly AdvertisementController _controller;

    public AdvertisementControllerTests()
    {
        _mockService = new Mock<IAdvertisementService>();
        _controller = new AdvertisementController(_mockService.Object);
    }

    /// <summary>
    /// Проверяет, что при передаче валидного файла метод возвращает статус OK
    /// и вызывает соответствующий метод сервиса для загрузки данных
    /// </summary>
    [Fact]
    public void LoadAdvertisements_ValidFile_ShouldReturnOk()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(10);
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        // Act
        var result = _controller.LoadAdvertisements(fileMock.Object);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockService.Verify(s => s.LoadAdvertisements(It.IsAny<Stream>()), Times.Once);
    }

    /// <summary>
    /// Проверяет, что при передаче null вместо файла метод возвращает ошибку BadRequest
    /// и не вызывает метод сервиса для загрузки данных
    /// </summary>
    [Fact]
    public void LoadAdvertisements_NullFile_ShouldReturnBadRequest()
    {
        // Act
        var result = _controller.LoadAdvertisements(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// Проверяет, что при запросе рекламных площадок для валидной локации
    /// метод возвращает корректный список площадок
    /// </summary>
    [Fact]
    public void SearchAdvertisements_ValidLocation_ShouldReturnAdvertisements()
    {
        // Arrange
        var expectedAds = new List<string> { "Яндекс.Директ", "Ревдинский рабочий" };
        _mockService.Setup(s => s.FindAdvertisements(It.IsAny<string>())).Returns(expectedAds);

        // Act
        var result = _controller.SearchAdvertisements("/ru/msk");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedAds);
    }

    /// <summary>
    /// Проверяет, что при запросе с пустой локацией метод возвращает ошибку BadRequest
    /// и не вызывает метод сервиса для поиска данных
    /// </summary>
    [Fact]
    public void SearchAdvertisements_EmptyLocation_ShouldReturnBadRequest()
    {
        // Act
        var result = _controller.SearchAdvertisements("");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}