# Deployment Guide

## 1. Push to GitHub

Repository: `SuryaPrakash1812/FarmManagement`

```powershell
git remote add origin https://github.com/SuryaPrakash1812/FarmManagement.git
git push -u origin main
```

## 2. Supabase PostgreSQL

1. Create a free Supabase project.
2. Copy the pooled connection string.
3. Use it as `ConnectionStrings__DefaultConnection` in Render.

## 3. Render Backend

1. Create a new Web Service from GitHub.
2. Select this repo.
3. Use Docker environment or import `render.yaml`.
4. Set environment variables:
   - `DatabaseProvider=PostgreSQL`
   - `ConnectionStrings__DefaultConnection=<supabase connection string>`
   - `Jwt__Secret=<strong random secret>`
   - `Cors__AllowedOrigins__0=<vercel frontend url>`

## 4. Vercel Frontend

1. Import this repo in Vercel.
2. Set root directory to `frontend`.
3. Build command: `npm run build`.
4. Output directory: `dist`.
5. Add `VITE_API_URL=https://your-render-api.onrender.com/api`.

## 5. Mobile Camera Support

For camera capture to work, browsers require HTTPS except on localhost. Vercel provides HTTPS automatically.
