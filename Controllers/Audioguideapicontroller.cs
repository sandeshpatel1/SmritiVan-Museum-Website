using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmritivanWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AudioGuideApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AudioGuideApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("tracks")]
        public async Task<IActionResult> GetTracks([FromQuery] string lang = "English")
        {
            var tracks = new List<object>();
            var connStr = _configuration.GetConnectionString("DefaultConnection");

            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            // Parameterised – safe against SQL injection
            const string sql = @"
                SELECT [pk], [Video_language], [Description], [Path],
                       [Introduction], [Content], [Audio_no], [Display_name], [pknum]
                FROM   [sa_smritivan].[Audio_guide_Path]
                WHERE  [Video_language] = @lang
                ORDER  BY [pk]";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@lang", lang);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tracks.Add(new
                {
                    pk = reader.GetInt32(0),
                    lang = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    path = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    introduction = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    content = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    audioNo = reader.GetInt32(6),
                    displayName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    pknum = reader.GetInt32(8)
                });
            }

            return Ok(tracks);
        }
    }
}