# FacturationApp 🧾

A professional, lightweight invoicing and fiscal management web application built with .NET 8 and Blazor — tailored to Tunisian invoicing needs and fiscal rules. The app helps small businesses and freelancers manage clients, products, invoices, and basic fiscal settings with a clean UI and exportable documents.

## Key Features ✅
- Client management (create, edit, list) with contact details
- Product catalog with prices and taxes
- Create, edit and list invoices with multiple invoice lines
- Automatic invoice numbering (configurable)
- Fiscal settings and Tunisian currency support
- Dashboard with basic analytics and charts (totals, sales by client/product)
- Data persistence using Entity Framework Core migrations
- Lightweight services layer (`Services/`) for business logic

## Technical Stack 🛠️
- .NET 8, ASP.NET Core, and Blazor Server
- Entity Framework Core for data access (SQL Server / SQLite compatible)
- C# 11 language features
- Razor components for UI (see `Components/` and `Pages/`)
- Client, Product and Invoice services in `Services/`

## Architecture Overview 🏗️

- Presentation: Blazor Server components in `Components/` and pages in `Pages/`.
- Data: EF Core `AppDbContext` located in `Data/AppDbContext.cs`, migrations in `Migrations/`.
- Services: `Services/` contains `ClientService`, `ProductService`, and `InvoiceService` providing business logic and data access abstractions.
- Models: Domain models are in `Models/` (e.g., `Invoice`, `InvoiceLine`, `Product`, `Client`).

This layered approach keeps UI, business logic and persistence concerns separated for easier maintenance and testing.

## Dashboard Analytics 📊

The dashboard provides quick visual insights into business activity:
- Total revenue and invoice counts for configurable date ranges
- Sales breakdown by client and by product
- Recent invoices and outstanding amounts

Charts are rendered using simple client-side scripts in `wwwroot/js/charts.js` and data is aggregated by the services layer.

## Installation & Run Locally ▶️

Prerequisites:
- .NET 8 SDK
- Optional: Visual Studio 2022/2026 or Visual Studio Code

From the project root, restore, build and run the app:

```powershell
# restore, build and run
dotnet restore
dotnet build
dotnet run
```

Open the browser at the URL printed by `dotnet run` or run the project from Visual Studio.

Database & Migrations:
- The project uses EF Core migrations located in `Migrations/`.
- Update the connection string in `appsettings.json` or `appsettings.Development.json` to point to your SQL Server, SQLite, or other supported provider.
- To apply migrations manually:

```powershell
dotnet ef database update
```

Secrets & configuration:
- Do not store production secrets in source control. Use `dotnet user-secrets`, environment variables, or a secure vault for API keys and credentials.

## Usage Notes 🇹🇳

- The application is adapted for Tunisian invoicing scenarios: currency support, fiscal settings, and invoice numbering conventions can be configured via `FiscalSettings` (see `Models/FiscalSettings.cs`).

## Contributing 🤝

- Create a new branch for your work: `git checkout -b feat/your-feature`
- Keep commits small and focused, open a pull request when ready
- Do not commit secrets — add any API keys to user secrets or environment variables

## Future Improvements ✨
- Export invoices to PDF (templated, printable invoices)
- Multi-currency and exchange-rate support with auto-conversion
- Advanced reporting (VAT reports, fiscal summaries for local tax filing)
- User authentication/roles (admin, accountant, viewer)
- Automated tests (unit and integration) and CI pipeline setup

## Contact & Support

If you encounter issues or have feature requests, please open an issue or submit a pull request.

# Authors

Developed by:

- Manel Souissi
- Mohamed Bayrem Ben Khoud
- Mohamed Raed Trifi

