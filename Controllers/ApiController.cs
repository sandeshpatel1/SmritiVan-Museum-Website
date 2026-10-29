using Microsoft.AspNetCore.Mvc;

namespace SmritivanWeb.Controllers;

/// <summary>
/// REST API controller that replaces the Node.js Express backend.
/// Original routes:
///   GET /api/albums          → SmritivanApiService.GetGalleryAlbumsAsync()
///   GET /api/images/:id      → SmritivanApiService.GetAlbumImagesAsync(id)
/// Additional endpoints for all JS calls in common.js.
/// </summary>
[Route("api")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly SmritivanApiService _api;

    public ApiController(SmritivanApiService api) => _api = api;

    // ── Gallery ─────────────────────────────────────────────────────────────

    [HttpGet("albums")]
    public async Task<ContentResult> GetAlbums()
    {
        var json = await _api.GetGalleryAlbumsAsync();
        return Content(json, "application/json");
    }

    [HttpGet("images/{id}")]
    public async Task<ContentResult> GetImages(string id)
    {
        var json = await _api.GetAlbumImagesAsync(id);
        return Content(json, "application/json");
    }

    // ── Events ───────────────────────────────────────────────────────────────

    [HttpGet("events")]
    public async Task<ContentResult> GetEvents()
        => Content(await _api.GetEventsAsync(), "application/json");

    [HttpGet("events/upcoming")]
    public async Task<ContentResult> GetUpcomingEvents()
        => Content(await _api.GetUpcomingEventsAsync(), "application/json");

    // ── Audio ────────────────────────────────────────────────────────────────

    [HttpGet("audio/pm")]
    public async Task<ContentResult> GetPmAudio()
        => Content(await _api.GetPmAudioAsync(), "application/json");

    [HttpGet("audio/details")]
    public async Task<ContentResult> GetAudioDetails([FromQuery] string audioNo, [FromQuery] string lang = "english")
        => Content(await _api.GetAudioDetailsAsync(audioNo ?? "", lang), "application/json");

    // ── Testimonials ─────────────────────────────────────────────────────────

    [HttpGet("testimonials")]
    public async Task<ContentResult> GetTestimonials()
        => Content(await _api.GetTestimonialsAsync(), "application/json");

    // ── Check Dam ────────────────────────────────────────────────────────────

    [HttpGet("checkdam/dashboard")]
    public async Task<ContentResult> GetCheckDamDashboard()
        => Content(await _api.GetCheckDamDashboardAsync(), "application/json");

    [HttpGet("checkdam/cities")]
    public async Task<ContentResult> GetCheckDamCities()
        => Content(await _api.GetCheckDamCitiesAsync(), "application/json");

    [HttpGet("checkdam/filter")]
    public async Task<ContentResult> GetCheckDamFilter([FromQuery] string name = "", [FromQuery] string city = "")
        => Content(await _api.GetCheckDamFilterAsync(name, city), "application/json");

    [HttpGet("checkdam/allfilter")]
    public async Task<ContentResult> GetCheckDamAllFilter([FromQuery] string name = "", [FromQuery] string city = "")
        => Content(await _api.GetCheckDamAllFilterAsync(name, city), "application/json");

    // ── Museum ───────────────────────────────────────────────────────────────

    [HttpGet("museum")]
    public async Task<ContentResult> GetMuseum()
        => Content(await _api.GetMuseumDetailsAsync(), "application/json");

    // ── Tickets ──────────────────────────────────────────────────────────────

    [HttpGet("tickets")]
    public async Task<ContentResult> GetTickets()
        => Content(await _api.GetTicketPricesAsync(), "application/json");

    // ── Feedback ─────────────────────────────────────────────────────────────

    [HttpPost("feedback")]
    public async Task<ContentResult> SubmitFeedback([FromForm] Dictionary<string, string> fields)
    {
        var json = await _api.SubmitFeedbackAsync(fields);
        return Content(json, "application/json");
    }
}
