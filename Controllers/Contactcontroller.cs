using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace SmritivanWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IConfiguration config, ILogger<ContactController> logger)
        {
            _config = config;
            _logger = logger;
        }

        // GET /Contact
        public IActionResult Index()
        {
            return View();
        }

        // POST /Contact/SendAcknowledgement
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SendAcknowledgement([FromBody] ContactFormDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { success = false, message = "Invalid payload." });

            try
            {
                // ✅ STEP 1: CAPTCHA VALIDATION START
                var captchaToken = dto.CaptchaToken;
                var secretKey = _config["Recaptcha:SecretKey"];

                using var http = new HttpClient();

                var response = await http.GetStringAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={captchaToken}");

                var captchaResult = System.Text.Json.JsonDocument.Parse(response);

                var success = captchaResult.RootElement.GetProperty("success").GetBoolean();

                if (!success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Captcha validation failed"
                    });
                }
                // ✅ STEP 1 END

                // ✅ STEP 2: CONTINUE NORMAL FLOW
                await SendAcknowledgementEmailAsync(dto);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send acknowledgement email to {Email}", dto.Email);
                return Ok(new { success = false, message = "Email delivery failed but record was saved." });
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  PRIVATE EMAIL SENDER
        // ─────────────────────────────────────────────────────────────
        private async Task SendAcknowledgementEmailAsync(ContactFormDto dto)
        {
            var smtpHost = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpUser = _config["Smtp:Username"] ?? "contact@smritivanearthquakemuseum.org";
            var smtpPass = _config["Smtp:Password"] ?? "";
            var smtpFrom = _config["Smtp:From"] ?? "contact@smritivanearthquakemuseum.org";
            var smtpFromName = _config["Smtp:FromName"] ?? "Smritivan Earthquake Museum";

            var body = BuildEmailBody(dto);

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(smtpFrom, smtpFromName),
                Subject = $"We've Received Your {dto.RequestType} – Smritivan Earthquake Museum",
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(new MailAddress(dto.Email, $"{dto.FirstName} {dto.LastName}".Trim()));

            var bccAddress = _config["Smtp:BccAddress"] ?? "";
            if (!string.IsNullOrWhiteSpace(bccAddress))
                mail.Bcc.Add(bccAddress);

            await client.SendMailAsync(mail);
        }

        // ─────────────────────────────────────────────────────────────
        //  FULL BRANDED EMAIL TEMPLATE
        // ─────────────────────────────────────────────────────────────
        private static string BuildEmailBody(ContactFormDto dto)
        {
            var fullName = $"{dto.FirstName} {dto.LastName}".Trim();
            var submittedAt = DateTime.Now.ToString("dd MMM yyyy, hh:mm tt");
            var encodedMsg = WebUtility.HtmlEncode(dto.Message).Replace("\n", "<br/>");

            // Request-type badge colour
            var badgeColor = dto.RequestType switch
            {
                "Complaint" => "#c0392b",
                "Feedback" => "#c9a96e",
                "Suggestion" => "#27ae60",
                _ => "#8e7bb5"
            };

            return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8""/>
  <meta name=""viewport"" content=""width=device-width,initial-scale=1""/>
  <title>Smritivan – Request Received</title>
</head>
<body style=""margin:0;padding:0;background:#060504;font-family:'Georgia',serif;"">

<!-- ░░ OUTER WRAPPER ░░ -->
<table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""
       style=""background:#060504;padding:48px 0;"">
<tr><td align=""center"">

  <!-- ░░ CARD ░░ -->
  <table width=""600"" cellpadding=""0"" cellspacing=""0"" border=""0""
         style=""max-width:600px;width:100%;background:#0a0807;
                border:1px solid rgba(201,169,110,0.22);"">

    <!-- ══ TOP ACCENT BAR ══ -->
    <tr>
      <td style=""height:3px;
                  background:linear-gradient(90deg,
                    transparent 0%,
                    #9a7a47 15%,
                    #c9a96e 40%,
                    #e8d5a3 50%,
                    #c9a96e 60%,
                    #9a7a47 85%,
                    transparent 100%);"">
      </td>
    </tr>

    <!-- ══ HEADER ══ -->
    <tr>
      <td style=""background:#000;padding:40px 52px 36px;text-align:center;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:22px;"">
          <tr>
            <td style=""height:1px;background:linear-gradient(90deg,transparent,rgba(201,169,110,0.3),transparent);""></td>
          </tr>
        </table>

        <p style=""font-size:9px;letter-spacing:7px;text-transform:uppercase;
                   color:#c9a96e;margin:0 0 14px;font-family:'Georgia',serif;"">
          ✦ &nbsp; SMRITIVAN &nbsp; ✦
        </p>
        <h1 style=""font-family:'Georgia',serif;font-size:30px;font-weight:700;
                    color:#f2ece2;margin:0 0 6px;letter-spacing:2px;line-height:1.1;"">
          Earthquake Museum
        </h1>
        <p style=""font-size:10px;letter-spacing:4px;text-transform:uppercase;
                   color:rgba(201,169,110,0.55);margin:0 0 22px;"">
          Bhuj &nbsp;·&nbsp; Gujarat &nbsp;·&nbsp; India
        </p>

        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""height:1px;background:linear-gradient(90deg,transparent,rgba(201,169,110,0.3),transparent);""></td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ══ HERO GREETING ══ -->
    <tr>
      <td style=""padding:48px 52px 0;"">
        <p style=""font-size:8.5px;letter-spacing:5px;text-transform:uppercase;
                   color:#c9a96e;margin:0 0 10px;"">── &nbsp; Acknowledgement</p>
        <table cellpadding=""0"" cellspacing=""0"" style=""margin:0 0 22px;"">
          <tr><td style=""width:36px;height:1px;background:#c9a96e;""></td></tr>
        </table>

        <h2 style=""font-family:'Georgia',serif;font-size:34px;font-weight:700;
                    color:#f2ece2;margin:0 0 4px;line-height:1.05;letter-spacing:-0.5px;"">
          Thank You,
        </h2>
        <h2 style=""font-family:'Georgia',serif;font-size:28px;font-weight:300;
                    font-style:italic;color:rgba(201,169,110,0.9);margin:0 0 26px;
                    line-height:1.05;letter-spacing:1px;"">
          {fullName}
        </h2>

        <p style=""font-size:14px;color:rgba(242,236,226,0.55);font-weight:300;
                   line-height:1.9;margin:0 0 10px;"">
          We have received your
          <span style=""display:inline-block;background:rgba(201,169,110,0.08);
                        border:1px solid {badgeColor};color:{badgeColor};
                        font-size:9.5px;letter-spacing:2px;text-transform:uppercase;
                        padding:2px 10px;margin:0 3px;vertical-align:middle;"">
            {dto.RequestType}
          </span>
          and our team will review it carefully.
        </p>
        <p style=""font-size:14px;color:rgba(242,236,226,0.55);font-weight:300;
                   line-height:1.9;margin:0;"">
          You can expect a personalised response within
          <strong style=""color:#c9a96e;font-weight:400;"">2–3 working days</strong>.
        </p>
      </td>
    </tr>

    <!-- ══ SUBMISSION SUMMARY ══ -->
    <tr>
      <td style=""padding:36px 52px 0;"">
        <p style=""font-size:8.5px;letter-spacing:5px;text-transform:uppercase;
                   color:#c9a96e;margin:0 0 14px;"">── &nbsp; Your Submission</p>

        <table width=""100%"" cellpadding=""0"" cellspacing=""0""
               style=""border:1px solid rgba(201,169,110,0.15);background:rgba(255,255,255,0.015);"">

          <tr>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         width:36%;vertical-align:top;font-size:9px;letter-spacing:2.5px;
                         text-transform:uppercase;color:rgba(201,169,110,0.45);"">Full Name</td>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:13.5px;color:#f2ece2;font-weight:300;"">{fullName}</td>
          </tr>

          <tr style=""background:rgba(201,169,110,0.02);"">
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:9px;letter-spacing:2.5px;
                         text-transform:uppercase;color:rgba(201,169,110,0.45);"">Email</td>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:13.5px;color:#f2ece2;font-weight:300;"">{dto.Email}</td>
          </tr>

          <tr>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:9px;letter-spacing:2.5px;
                         text-transform:uppercase;color:rgba(201,169,110,0.45);"">Mobile</td>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:13.5px;color:#f2ece2;font-weight:300;"">{dto.Mobile}</td>
          </tr>

          <tr style=""background:rgba(201,169,110,0.02);"">
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:9px;letter-spacing:2.5px;
                         text-transform:uppercase;color:rgba(201,169,110,0.45);"">Request</td>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;"">
              <span style=""font-size:9.5px;letter-spacing:2px;text-transform:uppercase;
                            color:{badgeColor};border:1px solid {badgeColor};
                            padding:3px 12px;background:rgba(201,169,110,0.04);"">
                {dto.RequestType}
              </span>
            </td>
          </tr>

          <tr>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:9px;letter-spacing:2.5px;
                         text-transform:uppercase;color:rgba(201,169,110,0.45);"">Submitted</td>
            <td style=""padding:14px 20px;border-bottom:1px solid rgba(201,169,110,0.08);
                         vertical-align:top;font-size:13px;color:rgba(242,236,226,0.35);font-weight:300;"">
              {submittedAt}
            </td>
          </tr>

          <tr style=""background:rgba(201,169,110,0.02);"">
            <td style=""padding:14px 20px;vertical-align:top;font-size:9px;letter-spacing:2.5px;
                         text-transform:uppercase;color:rgba(201,169,110,0.45);"">Message</td>
            <td style=""padding:14px 20px;vertical-align:top;font-size:13.5px;
                         color:rgba(242,236,226,0.5);font-weight:300;line-height:1.8;
                         border-left:2px solid rgba(201,169,110,0.25);"">
              {encodedMsg}
            </td>
          </tr>

        </table>
      </td>
    </tr>

    <!-- ══ WHAT HAPPENS NEXT ══ -->
    <tr>
      <td style=""padding:36px 52px 0;"">
        <p style=""font-size:8.5px;letter-spacing:5px;text-transform:uppercase;
                   color:#c9a96e;margin:0 0 20px;"">── &nbsp; What Happens Next</p>

        <!-- Step 1 -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:14px;"">
          <tr>
            <td style=""width:36px;vertical-align:top;padding-top:1px;"">
              <div style=""width:24px;height:24px;border:1px solid rgba(201,169,110,0.35);
                           background:rgba(201,169,110,0.08);text-align:center;
                           line-height:24px;font-size:10px;color:#c9a96e;"">1</div>
            </td>
            <td style=""padding-left:14px;font-size:13px;color:rgba(242,236,226,0.45);
                         font-weight:300;line-height:1.75;"">
              Your <strong style=""color:#f2ece2;font-weight:400;"">{dto.RequestType.ToLower()}</strong>
              has been logged in our system with a unique reference.
            </td>
          </tr>
        </table>

        <!-- Step 2 -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin-bottom:14px;"">
          <tr>
            <td style=""width:36px;vertical-align:top;padding-top:1px;"">
              <div style=""width:24px;height:24px;border:1px solid rgba(201,169,110,0.35);
                           background:rgba(201,169,110,0.08);text-align:center;
                           line-height:24px;font-size:10px;color:#c9a96e;"">2</div>
            </td>
            <td style=""padding-left:14px;font-size:13px;color:rgba(242,236,246,0.45);
                         font-weight:300;line-height:1.75;"">
              Our team will review your message within
              <strong style=""color:#f2ece2;font-weight:400;"">1 working day</strong>.
            </td>
          </tr>
        </table>

        <!-- Step 3 -->
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""width:36px;vertical-align:top;padding-top:1px;"">
              <div style=""width:24px;height:24px;border:1px solid rgba(201,169,110,0.35);
                           background:rgba(201,169,110,0.08);text-align:center;
                           line-height:24px;font-size:10px;color:#c9a96e;"">3</div>
            </td>
            <td style=""padding-left:14px;font-size:13px;color:rgba(242,236,226,0.45);
                         font-weight:300;line-height:1.75;"">
              A personalised reply will be sent to
              <strong style=""color:#c9a96e;font-weight:400;"">{dto.Email}</strong>
              within 2–3 working days.
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ══ DIVIDER ══ -->
    <tr>
      <td style=""padding:36px 52px 0;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""height:1px;background:linear-gradient(90deg,transparent,rgba(201,169,110,0.2),transparent);""></td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ══ CONTACT INFO ══ -->
    <tr>
      <td style=""padding:28px 52px 0;"">
        <p style=""font-size:8.5px;letter-spacing:5px;text-transform:uppercase;
                   color:#c9a96e;margin:0 0 14px;"">── &nbsp; Reach Us Directly</p>
        <p style=""font-size:13px;color:rgba(242,236,226,0.35);font-weight:300;
                   line-height:1.85;margin:0 0 14px;"">
          For urgent matters, our team is available Tuesday – Sunday,
          <strong style=""color:rgba(242,236,226,0.55);"">10:00 AM – 6:00 PM</strong>.
        </p>
        <table cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""padding:0 0 10px;font-size:13px;color:rgba(242,236,226,0.35);font-weight:300;"">
              <a href=""tel:+916357199991"" style=""color:#c9a96e;text-decoration:none;font-weight:400;"">+91 63571 99991</a>
              &nbsp;·&nbsp; Phone
            </td>
          </tr>
          <tr>
            <td style=""font-size:13px;color:rgba(242,236,226,0.35);font-weight:300;"">
              <a href=""mailto:contact@smritivanearthquakemuseum.org""
                 style=""color:#c9a96e;text-decoration:none;font-weight:400;"">contact@smritivanearthquakemuseum.org</a>
              &nbsp;·&nbsp; Email
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ══ SOCIAL ROW ══ -->
    <tr>
      <td style=""padding:28px 52px 0;text-align:center;"">
        <table align=""center"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""padding:0 5px;"">
              <a href=""https://www.instagram.com/smritivanearthquakemuseum/""
                 style=""display:inline-block;padding:8px 16px;
                         border:1px solid rgba(201,169,110,0.25);
                         font-size:9px;letter-spacing:2px;text-transform:uppercase;
                         color:rgba(201,169,110,0.6);text-decoration:none;"">Instagram</a>
            </td>
            <td style=""padding:0 5px;"">
              <a href=""https://www.facebook.com/SmritivanEarthquakeMuseum/""
                 style=""display:inline-block;padding:8px 16px;
                         border:1px solid rgba(201,169,110,0.25);
                         font-size:9px;letter-spacing:2px;text-transform:uppercase;
                         color:rgba(201,169,110,0.6);text-decoration:none;"">Facebook</a>
            </td>
            <td style=""padding:0 5px;"">
              <a href=""https://twitter.com/smritivan/""
                 style=""display:inline-block;padding:8px 16px;
                         border:1px solid rgba(201,169,110,0.25);
                         font-size:9px;letter-spacing:2px;text-transform:uppercase;
                         color:rgba(201,169,110,0.6);text-decoration:none;"">Twitter</a>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ══ DIVIDER ══ -->
    <tr>
      <td style=""padding:28px 52px 0;"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
          <tr>
            <td style=""height:1px;background:linear-gradient(90deg,transparent,rgba(201,169,110,0.2),transparent);""></td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- ══ FOOTER ══ -->
    <tr>
      <td style=""background:#000;padding:28px 52px 32px;text-align:center;"">
        <p style=""font-family:'Georgia',serif;font-size:18px;font-weight:700;
                   color:#f2ece2;letter-spacing:1px;margin:0 0 4px;"">Smritivan</p>
        <p style=""font-size:9px;color:rgba(201,169,110,0.5);letter-spacing:3.5px;
                   text-transform:uppercase;margin:0 0 16px;"">Earthquake Museum</p>
        <p style=""font-size:11.5px;color:rgba(255,255,255,0.2);margin:0 0 6px;line-height:1.7;"">
          Bhujio Dungar, Bhuj, Gujarat – 370020, India
        </p>
        <p style=""font-size:10.5px;color:rgba(255,255,255,0.12);margin:0;line-height:1.7;"">
          This is an automated acknowledgement — please do not reply to this email.<br/>
          © {DateTime.Now.Year} Smritivan Earthquake Museum. All rights reserved.
        </p>
      </td>
    </tr>

    <!-- ══ BOTTOM ACCENT BAR ══ -->
    <tr>
      <td style=""height:3px;
                  background:linear-gradient(90deg,
                    transparent 0%,
                    #9a7a47 15%,
                    #c9a96e 40%,
                    #e8d5a3 50%,
                    #c9a96e 60%,
                    #9a7a47 85%,
                    transparent 100%);"">
      </td>
    </tr>

  </table>
</td></tr>
</table>

</body>
</html>";
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  DTO
    // ─────────────────────────────────────────────────────────────
    public class ContactFormDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string RequestType { get; set; } = "General Inquiry";
        public string Message { get; set; } = "";

        // ✅ ADD THIS LINE
        public string CaptchaToken { get; set; } = "";
    }
}