using EffectiveMobile.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EffectiveMobile.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowAll")]
public class AdvertisementController : ControllerBase
{
    private readonly IAdvertisementService _advertisementService;

    public AdvertisementController(IAdvertisementService advertisementService)
    {
        _advertisementService = advertisementService;
    }

    [HttpPost("load")]
    public IActionResult LoadAdvertisements(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        try
        {
            using var stream = file.OpenReadStream();
            _advertisementService.LoadAdvertisements(stream);
            return Ok("Advertisements loaded successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error loading advertisements: {ex.Message}");
        }
    }

    [HttpGet("search")]
    public IActionResult SearchAdvertisements([FromQuery] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Location parameter is required");

        if (!location.StartsWith("/"))
            return BadRequest("Location must start with '/'");

        try
        {
            var advertisements = _advertisementService.FindAdvertisements(location);
            return Ok(advertisements);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error searching advertisements: {ex.Message}");
        }
    }
}