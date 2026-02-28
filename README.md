# Solution Magasin

An ASP.NET Core MVC web application for store management (gestion de magasin).

## Features

- **Admin space**: Manage products, categories, suppliers, employees, sales, purchases, returns, reviews, and invoices.
- **Employee space**: Manage stock, purchases, orders, and attendance (presence).
- **Client/Visitor space**: Browse products, place orders, leave reviews, and view invoices.
- **Authentication & Authorization**: Role-based access control (Admin, Magasinier, Client).

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or remote)

### Configuration

1. Copy `appsettings.json` and update the `DefaultConnection` connection string to point to your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=dotnet_project;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

2. Apply migrations:

```bash
dotnet ef database update
```

3. Run the application:

```bash
dotnet run
```

On first launch, a default admin account is created:
- **Email**: `admin@magasin.com`
- **Password**: `Admin123!`

> **Important**: Change the default admin password after first login.

## Project Structure

- `Controllers/` – MVC controllers for each role space
- `Models/` – Entity models and DbContext
- `Views/` – Razor views
- `Services/` – Business logic services (invoice generation, image upload, DB seeding)
- `Migrations/` – EF Core migrations
- `Constants/` – Role name constants
- `wwwroot/` – Static assets