using EffectiveMobile.Services;
using FluentAssertions;
using Xunit;

namespace TestProject;

public class AdvertisementServiceTests
{
    private readonly AdvertisementService _service;

    public AdvertisementServiceTests()
    {
        _service = new AdvertisementService();
    }

    /// <summary>
    /// Проверяет корректность загрузки данных из файла и поиска рекламных площадок
    /// для конкретной локации с учетом вложенности
    /// </summary>
    [Fact]
    public void LoadAdvertisements_ValidData_ShouldLoadCorrectly()
    {
        // Arrange
        var testData = "Яндекс.Директ:/ru\nРевдинский рабочий:/ru/svr d/r ev da, /ru/svr d/per v i k";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testData));

        // Act
        _service.LoadAdvertisements(stream);

        // Assert
        var result = _service.FindAdvertisements("/ru/svr d/r ev da");
        result.Should().Contain("Яндекс.Директ");
        result.Should().Contain("Ревдинский рабочий");
    }

    /// <summary>
    /// Проверяет, что поиск рекламных площадок работает корректно для вложенных локаций,
    /// находя площадки как для конкретной локации, так и для её родительских локаций
    /// </summary>
    [Fact]
    public void FindAdvertisements_ShouldFindParentLocations()
    {
        // Arrange
        var testData = "Яндекс.Директ:/ru\nКрутая реклама:/ru/svr d";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testData));
        _service.LoadAdvertisements(stream);

        // Act
        var result = _service.FindAdvertisements("/ru/svr d/r ev da");

        // Assert
        result.Should().Contain("Яндекс.Директ");
        result.Should().Contain("Крутая реклама");
    }

    /// <summary>
    /// Проверяет, что для несуществующей локации возвращается пустой список площадок
    /// </summary>
    [Fact]
    public void FindAdvertisements_NonExistentLocation_ShouldReturnEmpty()
    {
        // Arrange
        var testData = "Яндекс.Директ:/ru";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testData));
        _service.LoadAdvertisements(stream);

        // Act
        var result = _service.FindAdvertisements("/non/existent");

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Проверяет, что метод загрузки данных не выбрасывает исключение при получении пустого файла
    /// </summary>
    [Fact]
    public void LoadAdvertisements_EmptyFile_ShouldNotThrow()
    {
        // Arrange
        var emptyStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(""));

        // Act & Assert
        var act = () => _service.LoadAdvertisements(emptyStream);
        act.Should().NotThrow();
    }
}