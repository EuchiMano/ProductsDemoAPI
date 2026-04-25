# NewProjectFromScratch - API Documentation

A production-ready ASP.NET Core 8.0 API with JWT authentication, RabbitMQ messaging, and comprehensive product management endpoints. This project demonstrates best practices for building scalable APIs with security, logging, and event-driven architecture.

## 📋 Table of Contents

- [Features](#features)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Authentication](#authentication)
- [API Endpoints](#api-endpoints)
- [Docker Commands](#docker-commands)
- [Development](#development)
- [Troubleshooting](#troubleshooting)

---

## ✨ Features

- **JWT Authentication**: Secure token-based authentication with role-based access control
- **Product Management**: CRUD operations for products with stock management
- **Event-Driven Architecture**: RabbitMQ integration for asynchronous event publishing
- **Comprehensive Logging**: File-based and console logging for monitoring
- **Request Logging**: Middleware for tracking all API requests
- **Swagger/OpenAPI**: Interactive API documentation
- **Health Checks**: Built-in health monitoring for Docker
- **Role-Based Authorization**: Admin and User roles with different permissions

---

## 📂 Project Structure

```
NewProjectFromScratch/
├── Application/              # Business logic layer
│   ├── DTOs/               # Data transfer objects
│   ├── Interfaces/         # Service contracts
│   └── Services/           # Business services
├── Controllers/            # API endpoints
├── Domain/                 # Entity models
│   ├── Entities/          # Core domain models
│   └── Events/            # Domain events
├── Infrastructure/        # External integrations
│   ├── Data/             # Data repository implementations
│   ├── Logging/          # Logging providers
│   ├── Messaging/        # RabbitMQ implementation
│   └── Security/         # Authentication & user management
├── Properties/           # Launch settings
├── logs/                 # Application logs (created at runtime)
├── Dockerfile           # Docker image configuration
├── docker-compose.yml   # Multi-container orchestration
└── appsettings.json    # Configuration settings
```

---

## 🚀 Prerequisites

### Option 1: Run with Docker (Recommended)
- Docker Desktop (includes Docker & Docker Compose)
- No additional setup needed

### Option 2: Run Locally
- .NET 8.0 SDK or Runtime
- RabbitMQ server running (port 5672)

---

## ⚡ Quick Start

### Using Docker (One Command!)

```bash
docker-compose up --build
```

This command will:
1. Build the API Docker image
2. Start RabbitMQ container
3. Start the API container
4. Set up networking and volumes
5. Wait for all services to be healthy

The application will be ready in about 30 seconds.

### Verify Everything is Running

```bash
docker-compose ps
```

You should see both services in a "running" state.

---

## 🔐 Authentication

The API uses JWT (JSON Web Token) authentication. All endpoints require authentication except:
- `POST /api/auth/login` - Login endpoint
- `GET /api/products` - Get products (public read-only)

### Pre-configured Users

Two users are pre-configured in the application:

| Username | Password | Role  |
|----------|----------|-------|
| `admin`  | `test123` | Admin |
| `user`   | `test123` | User  |

### Authentication Flow

#### Step 1: Get a JWT Token

Send a POST request to the login endpoint:

**Request:**
```bash
curl -X POST http://localhost:5140/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "test123"
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "role": "Admin"
}
```

#### Step 2: Use the Token in Subsequent Requests

Add the token to the `Authorization` header with the `Bearer` scheme:

```bash
curl -X GET http://localhost:5140/api/products \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### Step 3: Using Swagger UI (Easiest)

1. Navigate to http://localhost:5140/swagger
2. Click the **Authorize** button (top right)
3. Paste the token you received from the login endpoint
4. Click **Authorize**
5. All subsequent requests will include the token automatically

---

## 📡 API Endpoints

### Authentication

#### Login
- **Endpoint**: `POST /api/auth/login`
- **Authentication**: Not required
- **Request Body**:
  ```json
  {
    "username": "admin",
    "password": "test123"
  }
  ```
- **Response**: JWT token with username and role

---

### Products

#### Get All Products (Public)
- **Endpoint**: `GET /api/products`
- **Authentication**: Not required
- **Query Parameters**:
  - `category` (optional): Filter by category
  - `minPrice` (optional): Minimum price filter
  - `maxPrice` (optional): Maximum price filter
- **Example**:
  ```bash
  curl http://localhost:5140/api/products?category=Electronics&minPrice=10&maxPrice=1000
  ```
- **Response**:
  ```json
  [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "Laptop",
      "price": 999.99,
      "stock": 5,
      "category": "Electronics",
      "isActive": true
    }
  ]
  ```

#### Create Product
- **Endpoint**: `POST /api/products`
- **Authentication**: Required (Admin or User role)
- **Request Body**:
  ```json
  {
    "name": "Laptop",
    "price": 999.99,
    "stock": 10,
    "category": "Electronics"
  }
  ```
- **Response**: Created product with ID (201 Created)

#### Adjust Product Stock
- **Endpoint**: `PATCH /api/products/{id}/stock`
- **Authentication**: Required (Admin or User role)
- **Request Body**:
  ```json
  {
    "quantityChange": 5
  }
  ```
- **Notes**: `quantityChange` can be positive (add stock) or negative (reduce stock)

#### Deactivate Product
- **Endpoint**: `DELETE /api/products/{id}`
- **Authentication**: Required (Admin or User role)
- **Response**: 204 No Content

---

### Users (Admin Only)

#### Get All Users
- **Endpoint**: `GET /api/users`
- **Authentication**: Required (Admin role only)
- **Response**:
  ```json
  [
    {"username": "admin", "role": "Admin"},
    {"username": "user", "role": "User"}
  ]
  ```

#### Get Specific User
- **Endpoint**: `GET /api/users/{username}`
- **Authentication**: Required (Admin role only)
- **Response**:
  ```json
  {"username": "admin", "role": "Admin"}
  ```

#### Create User
- **Endpoint**: `POST /api/users`
- **Authentication**: Required (Admin role only)
- **Request Body**:
  ```json
  {
    "username": "newuser",
    "password": "securepassword123",
    "role": "User"
  }
  ```

#### Update User
- **Endpoint**: `PUT /api/users/{username}`
- **Authentication**: Required (Admin role only)
- **Request Body** (at least one field required):
  ```json
  {
    "password": "newpassword123",
    "role": "Admin"
  }
  ```

#### Delete User
- **Endpoint**: `DELETE /api/users/{username}`
- **Authentication**: Required (Admin role only)
- **Response**: 204 No Content

---

## 🐳 Docker Commands

### Start Services
```bash
# Start in foreground (see logs)
docker-compose up --build

# Start in background
docker-compose up -d --build
```

### Stop Services
```bash
# Stop containers (keeps volumes)
docker-compose down

# Stop containers and remove volumes
docker-compose down -v
```

### View Logs
```bash
# View all logs
docker-compose logs -f

# View API logs only
docker-compose logs -f api

# View RabbitMQ logs only
docker-compose logs -f rabbitmq

# View last 100 lines
docker-compose logs --tail=100
```

### Restart Services
```bash
docker-compose restart

# Restart specific service
docker-compose restart api
```

### Rebuild Images
```bash
# Rebuild without cache
docker-compose up --build --no-cache

# Just rebuild without starting
docker-compose build --no-cache
```

### Remove Everything
```bash
# Remove stopped containers, networks, and images
docker-compose down -v --rmi all
```

---

## 💻 Development

### Running Locally

#### Prerequisites
```bash
# Install .NET 8.0
# Start RabbitMQ locally (or Docker):
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.13-management
```

#### Build and Run
```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run --launch-profile https
```

#### Run Tests
```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test tests/NewProjectFromScratch.Tests/AuthControllerTests.cs
```

### Environment Variables

Configure these in `docker-compose.yml` environment section or as system variables:

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5140
RabbitMq__Host=rabbitmq
RabbitMq__Port=5672
RabbitMq__Username=guest
RabbitMq__Password=guest
RabbitMq__ExchangeName=products.exchange
Jwt__Key=ThisIsASecretKeyForTestingAtLeast32Chars
Jwt__Issuer=NewProjectFromScratch
Jwt__Audience=NewProjectFromScratchUsers
Jwt__ExpiryMinutes=60
```

---

## 📊 Accessing Services

### API
- **Base URL**: http://localhost:5140
- **Swagger UI**: http://localhost:5140/swagger
- **Health Check**: http://localhost:5140/health (if configured)

### RabbitMQ Management UI
- **URL**: http://localhost:15672
- **Username**: guest
- **Password**: guest

---

## 🔧 Troubleshooting

### API Container Won't Start

**Check logs:**
```bash
docker-compose logs api
```

**Common issues:**
- RabbitMQ not ready: Wait 10 seconds and check `docker-compose logs rabbitmq`
- Port 5140 already in use: Change port in `docker-compose.yml` or stop other services

### Can't Connect to RabbitMQ

**Solution 1: Check RabbitMQ health**
```bash
docker-compose logs rabbitmq
```

**Solution 2: Restart RabbitMQ**
```bash
docker-compose restart rabbitmq
```

**Solution 3: Use Docker hostname**
Ensure `RabbitMq__Host` is set to `rabbitmq` (not `localhost`) in docker-compose.yml

### Authentication Failing

**Verify credentials:**
- Username: `admin`
- Password: `test123`
- Both are case-sensitive

**Check token expiry:**
- Default token expiration: 60 minutes
- After expiry, you need to login again

### Swagger Not Loading

**Solution:**
```bash
# Rebuild and restart
docker-compose down
docker-compose up --build
```

Then access http://localhost:5140/swagger

### Out of Disk Space

**Clean up Docker:**
```bash
docker-compose down -v --rmi all
docker system prune -a
```

---

## 📝 Example Workflow

### 1. Start the Application
```bash
docker-compose up --build
```

### 2. Login as Admin
```bash
curl -X POST http://localhost:5140/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test123"}'
```

**Save the token from the response**

### 3. Create a Product
```bash
curl -X POST http://localhost:5140/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Gaming Mouse",
    "price":49.99,
    "stock":25,
    "category":"Accessories"
  }'
```

### 4. Get All Products
```bash
curl http://localhost:5140/api/products
```

### 5. Adjust Stock
```bash
curl -X PATCH http://localhost:5140/api/products/{PRODUCT_ID}/stock \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"quantityChange":-5}'
```

### 6. Monitor in Swagger
Visit http://localhost:5140/swagger and use the Authorize button to test all endpoints interactively.

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [JWT Authentication](https://jwt.io)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)
- [Docker Documentation](https://docs.docker.com)
- [Swagger/OpenAPI](https://swagger.io)

---

## 📞 Support

If you encounter any issues:

1. Check the logs: `docker-compose logs`
2. Verify all services are running: `docker-compose ps`
3. Review this documentation
4. Ensure Docker Desktop is updated

---

## 📄 License

This project is provided as-is for development and testing purposes.
