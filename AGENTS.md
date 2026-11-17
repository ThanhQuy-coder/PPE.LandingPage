# AGENTS.md

## Project
PPE.LandingPage - Landing page giới thiệu doanh nghiệp kinh doanh thiết bị bảo hộ lao động.

## Technology
- .NET 10
- Blazor WebAssembly Standalone
- Razor Components
- HTML5, CSS3, Bootstrap 5
- C#
- JSON for static content
- JavaScript only when necessary

## Architecture Rules
- Single Blazor WebAssembly project.
- No backend, API, database, authentication, admin panel or CMS.
- No unnecessary design patterns or complex architecture.
- Use reusable Razor Components.
- Keep each landing page section in its own component.
- Store public static data in wwwroot/data.
- Keep images in wwwroot/images.
- Keep styling in wwwroot/css.

## UI/UX Requirements
- Modern, professional industrial safety design.
- Responsive for mobile, tablet and desktop.
- Clear typography and visual hierarchy.
- Consistent spacing, colors and buttons.
- Use real product imagery where available.
- Optimize image sizes and loading performance.
- Accessible semantic HTML.

## Development Rules
- Implement one section at a time.
- Reuse existing components instead of duplicating code.
- Do not modify unrelated files.
- Run dotnet build after major changes.
- Explain important Blazor concepts when introducing them.
- Do not add dependencies without a clear reason.

## Deployment
- Target Cloudflare Pages.
- Build as static Blazor WebAssembly output.
- Ensure all asset paths work from the deployed root.
- Do not introduce server-side dependencies.
