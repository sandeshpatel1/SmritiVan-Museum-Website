namespace SmritivanWeb;

/// <summary>
/// C# service that proxies calls to the Smritivan external web service.
/// Replaces the Node.js / Express backend (server.js).
/// </summary>
public class SmritivanApiService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://accountsandtaxminers.com/SmritivanApp_WebService.asmx";

    public SmritivanApiService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri(BaseUrl + "/");
        _http.Timeout = TimeSpan.FromSeconds(30);
    }

    // ── Gallery ─────────────────────────────────────────────────────────────

    public Task<string> GetGalleryAlbumsAsync()
        => GetRawAsync("GetGalleryAlbumData");

    public Task<string> GetAlbumImagesAsync(string albumId)
        => GetRawAsync($"Get_AlbumImages?Album_Id={Uri.EscapeDataString(albumId)}");

    // ── Events ───────────────────────────────────────────────────────────────

    public Task<string> GetEventsAsync()
        => GetRawAsync("GetSmritivanEvents");

    public Task<string> GetUpcomingEventsAsync()
        => GetRawAsync("GetSmritivanUpcomingEvents");

    // ── Audio / PM ───────────────────────────────────────────────────────────

    public Task<string> GetPmAudioAsync()
        => GetRawAsync("GetPmAudioDetails");

    public Task<string> GetAudioDetailsAsync(string audioNo, string lang)
        => GetRawAsync($"GetAudioDetailWithType?Audio_No={Uri.EscapeDataString(audioNo)}&audioLanguage={Uri.EscapeDataString(lang)}");

    // ── Testimonials ─────────────────────────────────────────────────────────

    public Task<string> GetTestimonialsAsync()
        => GetRawAsync("GetTestimonialDetails");

    // ── Check Dam ────────────────────────────────────────────────────────────

    public Task<string> GetCheckDamDashboardAsync()
        => GetRawAsync("Get_CheckDamDashbordData");

    public Task<string> GetCheckDamCitiesAsync()
        => GetRawAsync("Get_CheckDamDropDownList");

    public Task<string> GetCheckDamFilterAsync(string name, string city)
        => GetRawAsync($"Get_CheckDamFilterData?Name_english={Uri.EscapeDataString(name)}&city={Uri.EscapeDataString(city)}");

    public Task<string> GetCheckDamAllFilterAsync(string name, string city)
        => GetRawAsync($"Get_CheckDamAllFilterData?Name_english={Uri.EscapeDataString(name)}&city={Uri.EscapeDataString(city)}");

    // ── Museum ───────────────────────────────────────────────────────────────

    public Task<string> GetMuseumDetailsAsync()
        => GetRawAsync("GetMueseumDetails");

    // ── Tickets ──────────────────────────────────────────────────────────────

    public Task<string> GetTicketPricesAsync()
        => GetRawAsync("GetRateDetails");

    // ── Feedback ─────────────────────────────────────────────────────────────

    public Task<string> SubmitFeedbackAsync(Dictionary<string, string> fields)
    {
        var qs = string.Join("&", fields.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return GetRawAsync($"InsertFeedbackDetails?{qs}");
    }

    // ── Helper ───────────────────────────────────────────────────────────────

    private async Task<string> GetRawAsync(string relativeUrl)
    {
        try
        {
            var response = await _http.GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"{{\"error\":\"{ex.Message}\"}}";
        }
    }
}
