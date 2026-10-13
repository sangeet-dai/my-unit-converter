# LearnHub — Web-Based Learning System (CT050-3-2 WAPP Final Assignment)

A fully responsive web-based learning system built with **ASP.NET Core MVC (.NET 8)**,
**Entity Framework Core** and a **local SQLite database**.

## Run

```bash
cd LearnHub
dotnet run          # serves http://localhost:3000
```

The database (`LearnHub.db`) is created and seeded automatically on first run.

## Demo accounts

| Role          | Email                 | Password     |
|---------------|-----------------------|--------------|
| Administrator | admin@learnhub.test   | Admin123!    |
| Member        | alice@learnhub.test   | Student123!  |
| Member        | bob@learnhub.test     | Student123!  |
| Member        | cara@learnhub.test    | Student123!  |

## Assignment requirements → where they are implemented

| Requirement | Where |
|---|---|
| Interlinked webpages | Shared layout navbar, breadcrumbs, footer links, related-resource links |
| HTML5 semantic elements | `header`, `nav`, `main`, `section`, `article`, `aside`, `footer`, `figure` in all views |
| External CSS | `wwwroot/css/site.css` (full design system) |
| Internal CSS | `<style>` blocks in `Views/Home/Index.cshtml`, `Views/Resources/Index.cshtml` |
| Inline styles | e.g. CTA panel, page headers, hero text |
| Database connectivity | EF Core + SQLite (`Data/AppDbContext.cs`) |
| Insert records | Register, contact form, enrol, review, admin CreateResource |
| Display records | Catalogue, details, dashboards, admin tables |
| Update records | Profile, progress, admin EditResource, toggle read/role |
| Delete records | Admin delete resource/member/review/message, unenrol, delete review |
| Registration page | `Views/Account/Register.cshtml` |
| Member module (login required) | `Controllers/MemberController.cs` + `Views/Member/*` |
| Administrator module | `Controllers/AdminController.cs` + `Views/Admin/*` |
| Form validation | DataAnnotations (server) + HTML5 attributes & jQuery Unobtrusive (client) |
| Navigation support | Responsive Bootstrap navbar, breadcrumbs, admin sidebar, footer |
| File organization | `Controllers/`, `Models/`, `Data/`, `Helpers/`, `Views/`, `wwwroot/` |
| Fully responsive | Bootstrap 5 grid + custom breakpoints at 992/768/576 px in `site.css` |
| Error handling | `UseStatusCodePagesWithReExecute` renders a designed 404 / error page |
| Multimedia elements | Self-hosted `<video>` with poster, dual sources and WebVTT captions, plus SVG imagery |

## Multimedia

| Asset | Location |
|---|---|
| Introduction clips (3) | `wwwroot/media/*.mp4` (H.264) with `.webm` (VP9) fallback |
| Poster frames | `wwwroot/media/posters/*.jpg` |
| Captions | `wwwroot/media/captions/*.vtt` (WebVTT subtitles) |

Resources that carry a clip store its name in `Resources.MediaSlug`, and the details page
renders a `<video>` element with `controls`, a `poster`, two `<source>` entries for format
fallback, and a `<track kind="captions">` for accessibility. WebVTT and WebM MIME types are
registered in `Program.cs` so the files are served with the correct content types.

## Design system

| Element | Choice |
|---|---|
| Headings | **Source Serif 4** (variable, self-hosted) |
| Interface / body | **Inter** (self-hosted) |
| Data, codes, numerals | **JetBrains Mono** |
| Icons | **Tabler Icons** webfont (5,936 icons, 2px stroke, self-hosted) |
| Palette | ivory paper `#fbfaf8`, ink `#101418`, evergreen accent `#0e4c3c`, brass detail `#a8843c` |
| Logo | `wwwroot/images/logo.svg` (mark) and `favicon.svg` |

All fonts and icons are vendored under `wwwroot/fonts` and `wwwroot/lib/tabler-icons`,
so the site renders correctly with no internet connection.

Design rules followed: no gradients, an inline 4px radius rather than pill shapes,
hairline 1px borders instead of drop shadows, and a restrained two-colour accent.

## Project structure

```
LearnHub/
├── Controllers/        # Home, Resources, Account, Member, Admin (+ Base)
├── Data/               # AppDbContext, DbSeeder
├── Helpers/            # PasswordHasher (PBKDF2)
├── Models/             # Entities + view models
├── Views/              # Razor views (Home, Resources, Account, Member, Admin, Shared)
└── wwwroot/            # css, js, images, self-hosted fonts, icons, bootstrap/jquery
```

## Security notes

- Passwords stored as PBKDF2 hashes (100,000 iterations, SHA-256).
- Cookie authentication with role-based authorization (`Member`, `Admin`).
- Anti-forgery tokens on every POST form; foreign POSTs are rejected (400).
- Server-side validation re-checked even when client-side validation is bypassed.
