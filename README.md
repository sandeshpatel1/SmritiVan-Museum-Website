# Smritivan Web — ASP.NET Core MVC (.NET 8)

Converted from the original HTML/CSS/JS + Node.js project to a full **ASP.NET Core MVC** web application targeting **.NET 8**, designed to run on **IIS Express** from **Visual Studio 2022** in **Google Chrome**.

---

## Prerequisites

- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET 8 SDK** — [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Google Chrome** (set as default browser in VS, or select from the run dropdown)

---

## How to Open & Run

1. **Open the solution** — double-click `SmritivanWeb.sln` (in the root folder)  
   OR in Visual Studio: **File → Open → Project/Solution** → select `SmritivanWeb.sln`

2. **Select IIS Express** — in the toolbar dropdown next to the green ▶ run button, choose **IIS Express**

3. **Select Chrome** — click the small arrow next to the IIS Express button → choose **Google Chrome**

4. Press **F5** (or click the ▶ button) — Visual Studio will build and launch the site in Chrome at `http://localhost:5050`

> **First run:** VS may ask you to trust the IIS Express SSL certificate — click **Yes** on both dialogs.

---

## Project Structure

```
SmritivanWeb.sln                   ← Open this in Visual Studio 2022
SmritivanWeb/
├── SmritivanWeb.csproj            ← .NET 8 project file
├── Program.cs                     ← App startup / DI configuration
├── SmritivanApiService.cs         ← C# backend (replaces Node.js server.js)
├── Controllers/
│   ├── HomeController.cs          ← Serves all website pages
│   └── ApiController.cs           ← REST API proxy (/api/albums, /api/images/:id, etc.)
├── Views/
│   ├── _ViewStart.cshtml          ← Sets shared layout
│   ├── _ViewImports.cshtml        ← Tag helpers & namespaces
│   ├── Shared/
│   │   └── _Layout.cshtml         ← Master layout (header + nav + footer)
│   └── Home/
│       ├── Index.cshtml           ← Home page (index.html)
│       ├── Museum.cshtml          ← museum.html
│       ├── Memorial.cshtml        ← memorial.html
│       ├── BriefHistory.cshtml    ← briefhistory.html
│       ├── Gallery.cshtml         ← gallery.html
│       ├── Events.cshtml          ← events.html
│       ├── Faq.cshtml             ← faq.html
│       ├── Contact.cshtml         ← contact.html
│       ├── Amenities.cshtml       ← amenities.html
│       ├── MapPage.cshtml         ← mappage.html
│       ├── MuseumTour.cshtml      ← museumtour.html
│       ├── AudioPlayer.cshtml     ← audioplayer.html
│       ├── AudioDetail1-4.cshtml  ← audiodetail1-4.html
│       ├── CheckDam.cshtml        ← checkdam.html
│       ├── Tickets.cshtml         ← tickets.html
│       ├── Privacy.cshtml         ← privacy.html
│       ├── Terms.cshtml           ← terms.html
│       └── Tour360.cshtml         ← Tour360.html (standalone, no layout)
├── wwwroot/
│   ├── css/                       ← All original CSS files (variables, reset, layout, components, pages)
│   └── js/
│       └── common.js              ← Original shared JS utility library
├── Properties/
│   └── launchSettings.json        ← IIS Express config (port 5050, launches browser)
├── appsettings.json
└── web.config                     ← IIS Express hosting config
```

---

## What Changed from the Original

| Original (Node.js)           | .NET Equivalent                        |
|------------------------------|----------------------------------------|
| `backend/server.js` (Express)| `SmritivanApiService.cs` + `ApiController.cs` |
| `GET /api/albums`            | `GET /api/albums` (same URL)           |
| `GET /api/images/:id`        | `GET /api/images/{id}` (same URL)      |
| `index.html`, `museum.html`, etc. | Razor Views (`.cshtml`) in `Views/Home/` |
| `.html` href links           | `/Home/ActionName` MVC routes          |
| `css/*.css`, `js/common.js`  | `wwwroot/css/`, `wwwroot/js/` (unchanged) |

**All original HTML, CSS, and JavaScript is preserved exactly** — only the file delivery mechanism changed from plain files to Razor views rendered by the MVC controller.

---

## API Endpoints (C# — replaces Node.js Express)

| Endpoint                     | Description                         |
|------------------------------|-------------------------------------|
| `GET /api/albums`            | Gallery albums                      |
| `GET /api/images/{id}`       | Images for an album                 |
| `GET /api/events`            | Past events                         |
| `GET /api/events/upcoming`   | Upcoming events                     |
| `GET /api/audio/pm`          | PM audio details                    |
| `GET /api/audio/details`     | Audio details by number & language  |
| `GET /api/testimonials`      | Testimonials                        |
| `GET /api/checkdam/dashboard`| Check dam dashboard data            |
| `GET /api/checkdam/cities`   | Check dam city list                 |
| `GET /api/checkdam/filter`   | Check dam filter                    |
| `GET /api/museum`            | Museum details                      |
| `GET /api/tickets`           | Ticket/rate details                 |
| `POST /api/feedback`         | Submit visitor feedback             |
