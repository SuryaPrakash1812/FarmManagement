# Animal Farm Management Web Application

A modern full-stack farm management system built with React/Vite, Material UI, ASP.NET Core Web API, EF Core, JWT auth, SQLite for development, and PostgreSQL for production.

## Modules

- Dashboard analytics: animals, stock, sales, income, expenses, payments, investment ROI, activity feed, charts
- User management: JWT login, logout endpoint, forgot/change password scaffolds, role-based authorization
- Animal management: full animal profile, search, soft delete, parent links, health status, photo upload, mobile camera capture
- Stock, sales, purchases, investments, expenses, payments, income, health, breeding, employee, reports, settings
- Exports: CSV, Excel, PDF endpoint scaffold
- Audit/activity log data model
- Backup endpoint and restore workflow scaffold
- PWA manifest and responsive mobile UI

## Default Login

Email: `admin@farm.local`
Password: `Admin@12345`

Change this immediately for production.

## Folder Structure

```text
backend/FarmManagement.Api   ASP.NET Core API
frontend                     React/Vite app
docs                         schema, ERD, API, deployment docs
render.yaml                  Render backend deployment blueprint
FarmManagement.slnx          .NET solution file
```

## Local Setup

### Backend

```powershell
cd backend/FarmManagement.Api
dotnet restore
dotnet build
dotnet run --urls http://localhost:5000
```

Swagger runs at `http://localhost:5000/swagger` in development.

### Frontend

```powershell
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173`.

## Environment Variables

Backend production:

- `DatabaseProvider=PostgreSQL`
- `ConnectionStrings__DefaultConnection=<Supabase pooled PostgreSQL connection string>`
- `Jwt__Secret=<64+ character random secret>`
- `Jwt__Issuer=FarmManagement`
- `Jwt__Audience=FarmManagement.Web`
- `Cors__AllowedOrigins__0=https://your-vercel-app.vercel.app`

Frontend:

- `VITE_API_URL=https://your-render-api.onrender.com/api`

## GitHub Deployment Target

Use repository: `https://github.com/SuryaPrakash1812/FarmManagement`

```powershell
git remote add origin https://github.com/SuryaPrakash1812/FarmManagement.git
git add .
git commit -m "Initial farm management application"
git push -u origin main
```

## Notes

The API auto-applies EF migrations on startup and seeds sample data. For larger farms, replace auto-migration with an explicit release migration step.
