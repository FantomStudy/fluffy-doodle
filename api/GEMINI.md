# Fluffy Doodle

## Project Overview
Fluffy Doodle is a REST API built with Go using the Fiber framework. It provides authentication and user management features, utilizing GORM for database interactions (PostgreSQL) and JWT for secure authentication via HTTP-only cookies. The project follows a clean architecture pattern with distinct layers for routing, handlers, services, and repositories.

### Main Technologies
- **Language:** Go 1.25.4
- **Web Framework:** [Fiber v2](https://gofiber.io/)
- **ORM:** [GORM](https://gorm.io/)
- **Database:** PostgreSQL
- **Authentication:** JWT (JSON Web Tokens) with `golang-jwt/jwt` and `bcrypt` for password hashing.
- **Documentation:** [Swagger](https://github.com/swaggo/swag)
- **Containerization:** Docker & Docker Compose

## Architecture
The project is organized into the following directory structure:
- `cmd/server/`: Contains the main entry point (`main.go`).
- `internal/api/`:
  - `handlers/`: Request handlers that process input and return responses.
  - `middleware/`: Custom middleware (e.g., authentication).
  - `presenter/`: Data Transfer Objects (DTOs) for requests and responses.
  - `routes/`: API route definitions.
- `internal/config/`: Configuration loading from environment variables.
- `internal/models/`: GORM database models.
- `internal/repository/`: Data access layer.
- `internal/service/`: Business logic layer.
- `pkg/database/`: Database connection and migration utilities.
- `docs/`: Auto-generated Swagger documentation.

## Building and Running

### Prerequisites
- Go 1.25+
- PostgreSQL
- Docker (optional)

### Environment Variables
Create a `.env` file in the root directory based on `.env.example`:
```env
PORT=3000
DB_HOST=localhost
DB_PORT=5432
DB_USER=your_user
DB_PASSWORD=your_password
DB_NAME=your_db
CORS=*
JWT_SECRET=your_jwt_secret
REFRESH_SECRET=your_refresh_secret
```

### Local Execution
1. Install dependencies:
   ```bash
   go mod download
   ```
2. Run the application:
   ```bash
   go run cmd/server/main.go
   ```
3. Access Swagger documentation at: `http://localhost:3000/swagger/index.html`

### Docker Execution
1. Build and run using Docker Compose:
   ```bash
   docker-compose up --build
   ```

## Development Conventions

### API Design
- Follow RESTful principles.
- Use Fiber's `BodyParser` for request validation.
- Return structured JSON responses using the `presenter` package.

### Layers
- **Handlers:** Should only handle request parsing, calling services, and returning responses.
- **Services:** Should contain business logic and validation.
- **Repositories:** Should focus on database operations.

### Authentication
- Authentication is handled via `access_token` and `refresh_token` cookies.
- Use the `middleware.Auth` middleware to protect routes.

### Documentation
- Use Swag comments in handlers to document endpoints.
- Regenerate Swagger docs after changes:
  ```bash
  swag init -g cmd/server/main.go
  ```

### Code Style
- Use `go fmt` for formatting.
- Error handling should be explicit; avoid ignoring errors.
- Comments for complex logic are encouraged.
