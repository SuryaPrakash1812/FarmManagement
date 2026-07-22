# API Documentation

Swagger: `/swagger` in development.

Authentication:

- `POST /api/auth/login`
- `POST /api/auth/logout`
- `POST /api/auth/users` Admin only
- `POST /api/auth/change-password`
- `POST /api/auth/forgot-password`

Dashboard:

- `GET /api/dashboard`

CRUD modules support `GET`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` with JWT authentication:

- `/api/animals`
- `/api/stock`
- `/api/sales`
- `/api/purchases`
- `/api/investments`
- `/api/expenses`
- `/api/incomes`
- `/api/payments`
- `/api/health`
- `/api/breeding`
- `/api/employees`

Special endpoints:

- `POST /api/animals/{id}/photo`: multipart `photo` file from upload or camera capture
- `GET /api/stock/low-stock`
- `GET /api/health/reminders`
- `GET /api/sales/{id}/invoice`
- `GET /api/reports/{report}/csv`
- `GET /api/reports/{report}/excel`
- `GET /api/reports/{report}/pdf`
- `GET /api/settings`
- `POST /api/settings/logo`
- `POST /api/settings/backup`
- `POST /api/settings/restore`
