using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmritivanWeb.Models;

namespace SmritivanWeb.Controllers
{
    [Route("api/gallery")]
    [ApiController]
    public class GalleryApiController : ControllerBase
    {
        private readonly IConfiguration _config;

        public GalleryApiController(IConfiguration config)
        {
            _config = config;
        }

        private string Conn => _config.GetConnectionString("DefaultConnection")!;

        // ─────────────────────────────────────────────
        // GET: /api/gallery/albums
        // ─────────────────────────────────────────────
        [HttpGet("albums")]
        public IActionResult GetAlbums()
        {
            var albums = new List<Album>();

            const string sql = @"
                SELECT 
                    Album_Name,
                    MIN(Gallery_Image) AS Gallery_Image
                FROM dbo.Gallery_Image
                WHERE (remove_flag IS NULL OR remove_flag != 'Yes')
                  AND Album_Name IS NOT NULL
                GROUP BY Album_Name
                ORDER BY Album_Name";

            try
            {
                using var con = new SqlConnection(Conn);
                con.Open();

                using var cmd = new SqlCommand(sql, con);
                using var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var rawImage = rdr.IsDBNull(1) ? null : rdr.GetString(1);

                    albums.Add(new Album
                    {
                        Album_Name = rdr.GetString(0),

                        Gallery_Image = string.IsNullOrWhiteSpace(rawImage)
                            ? null
                            : rawImage.Trim().Replace(" ", "")
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return Ok(albums);
        }

        // ─────────────────────────────────────────────
        // GET: /api/gallery/images/{albumName}
        // ─────────────────────────────────────────────
        [HttpGet("images/{albumName}")]
        public IActionResult GetImages(string albumName)
        {
            var images = new List<GalleryImage>();

            const string sql = @"
                SELECT
                    pk,
                    Gallery_Image,
                    Gallery_Path,
                    Title,
                    Album_Name
                FROM dbo.Gallery_Image
                WHERE Album_Name = @albumName
                  AND (remove_flag IS NULL OR remove_flag != 'Yes')
                ORDER BY pk";

            try
            {
                using var con = new SqlConnection(Conn);
                con.Open();

                using var cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@albumName", albumName);

                using var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    images.Add(new GalleryImage
                    {
                        Pk = rdr.GetInt32(0),
                        Gallery_Image = rdr.GetString(1).Trim().Replace(" ", ""),
                        Gallery_Path = rdr.GetString(2).Trim(),
                        Title = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                        Album_Name = rdr.IsDBNull(4) ? null : rdr.GetString(4)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return Ok(images);
        }
    }
}