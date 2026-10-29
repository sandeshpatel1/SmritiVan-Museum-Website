using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Data.SqlClient;

namespace SmritivanWeb.Controllers
{
    /// <summary>
    /// Main controller — serves every page of the Smritivan website.
    /// Visitor counter increments ONLY on the Home (Index) action.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly SmritivanApiService _api;
        private readonly IConfiguration _configuration;

        public HomeController(SmritivanApiService api, IConfiguration configuration)
        {
            _api = api;
            _configuration = configuration;
        }

        /// <summary>
        /// Increments the Home-page visitor count and returns the new total.
        /// Called exclusively by Index() — no other action touches this.
        /// </summary>
        private long IncrementAndGetVisitorCount()
        {
            long count = 0;
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE SiteVisitorCount
                          SET    TotalVisits = TotalVisits + 1
                          OUTPUT INSERTED.TotalVisits
                          WHERE  PageName = @PageName", con))
                    {
                        cmd.Parameters.AddWithValue("@PageName", "Home");
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            count = Convert.ToInt64(result);
                    }
                }
            }
            catch (Exception)
            {
                // Return 0 silently — the UI handles a missing count gracefully
                count = 0;
            }
            return count;
        }

        /// <summary>
        /// Returns the current visitor count WITHOUT incrementing.
        /// Called by the footer JS on all pages to poll for live updates.
        /// GET /Home/GetVisitorCount
        /// </summary>
        [HttpGet]
        public IActionResult GetVisitorCount()
        {
            long count = 0;
            try
            {
                string conString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT TotalVisits
                          FROM   SiteVisitorCount
                          WHERE  PageName = @PageName", con))
                    {
                        cmd.Parameters.AddWithValue("@PageName", "Home");
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            count = Convert.ToInt64(result);
                    }
                }
            }
            catch (Exception)
            {
                count = 0;
            }

            // Indian number format: ##,##,##0  →  e.g. "12,34,567"
            return Json(new { count = count.ToString("##,##,##0") });
        }

        // ── Page actions ───────────────────────────────────────────────────────

        // GET /  ← ONLY place the counter increments
        public IActionResult Index()
        {
            long visitorCount = IncrementAndGetVisitorCount();
            // Written into <body data-visitor-count="..."> by _Layout.cshtml
            ViewBag.VisitorCount = visitorCount.ToString("##,##,##0");
            return View();
        }

        // All other pages — no counter logic
        public IActionResult Amenities() => View();
        public IActionResult AudioDetail1() => View();
        public IActionResult AudioDetail2() => View();
        public IActionResult AudioDetail3() => View();
        public IActionResult AudioDetail4() => View();
        public IActionResult AudioPlayer() => View();
        public IActionResult BriefHistory() => View();
        public IActionResult CheckDam() => View();
        public IActionResult Contact() => View();
        public IActionResult Events() => View();
        public IActionResult Faq() => View();
        public IActionResult Gallery() => View();
        public IActionResult MapPage() => View();
        public IActionResult Memorial() => View();
        public IActionResult Museum() => View();
        public IActionResult MuseumTour() => View();
        public IActionResult Tour360() => View();
        public IActionResult Privacy() => View();
        public IActionResult Terms() => View();
        public IActionResult Tickets() => View();
        public IActionResult Error() => View();
    }
}