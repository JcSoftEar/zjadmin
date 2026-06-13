# ZJAdmin

A minimal, lightweight, and ready-to-use admin management system built with **.NET Core 8** + **Vue 3**. It provides only the most essential RBAC permission system and operation logging for enterprise backends.

## Features

- **RBAC Permission Management** — Three-tier user-role-permission model with menu and button-level access control
- **Operation Logging** — Automatically records all user actions with detail view and Excel export
- **Exception Logging** — Automatically captures system exceptions with full stack traces
- **JWT Authentication** — Stateless token-based authentication with 2-hour expiration
- **Dynamic Menus** — Sidebar menus generated dynamically based on user permissions
- **Permission Directives** — Frontend `v-permission` directive for button-level control
- **Login Protection** — Account locks for 15 minutes after 5 failed login attempts, BCrypt password hashing
- **Ready to Use** — Default SQLite database, no additional configuration required

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQLite (supports SQL Server / MySQL) |
| Authentication | JWT (JSON Web Token) |
| Logging | Serilog |
| Frontend Framework | Vue 3 (Composition API) |
| Build Tool | Vite |
| UI Library | Element Plus |
| State Management | Pinia |
| HTTP Client | Axios |

## Quick Start

### Prerequisites

- .NET 8 SDK
- Node.js 18+

### Start Backend

```bash
cd backend/ZJAdmin.Api
dotnet run --urls "http://localhost:5000"
```

The SQLite database will be created automatically on first run with seed data initialized.

### Start Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend dev server runs at `http://localhost:3417` by default, with API proxy configured to `http://localhost:5000`.

### Default Account

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Super Admin (full access) |

## Project Structure

```
├── backend/                      # .NET Core 8 Backend
│   └── ZJAdmin.Api/
│       ├── Controllers/          # API Controllers
│       ├── Models/               # Entity Models
│       ├── DTOs/                 # Data Transfer Objects
│       ├── Services/             # Business Logic Layer
│       ├── Middleware/           # Middleware (exception handling, operation log, permission filter)
│       ├── Data/                 # DbContext + Seed Data
│       ├── Attributes/           # Custom Attributes (permission annotations)
│       └── Program.cs            # Startup Configuration
├── frontend/                     # Vue 3 Frontend
│   └── src/
│       ├── api/                  # API Modules
│       ├── layouts/              # Layout Components
│       ├── views/                # Page Views
│       ├── stores/               # Pinia Stores
│       ├── router/               # Router Configuration
│       └── utils/                # Utility Functions
├── spec.md                       # Technical Specification
├── README.md                     # Project Documentation (Chinese)
└── README.en.md                  # Project Documentation (English)
```

## API Endpoints

All API responses follow a unified format:

```json
{
  "code": 200,
  "message": "success",
  "data": {},
  "total": 100
}
```

### Main Endpoints

| Module | Path | Description |
|--------|------|-------------|
| Auth | `/api/v1/auth/*` | Login, logout, profile, change password |
| Users | `/api/v1/users/*` | User CRUD, reset password, enable/disable |
| Roles | `/api/v1/roles/*` | Role CRUD, assign permissions |
| Permissions | `/api/v1/permissions/*` | Permission CRUD, permission tree |
| Operation Logs | `/api/v1/operation-logs/*` | Log query, detail, export, cleanup |
| Exception Logs | `/api/v1/exception-logs/*` | Log query, detail, export, cleanup |

## License

MIT
