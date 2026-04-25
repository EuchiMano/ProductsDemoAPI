# Quick Reference Guide

A handy cheat sheet for common commands and endpoints.

---

## 🚀 Getting Started

```bash
# Start everything with one command
docker-compose up --build

# Stop everything
docker-compose down

# View logs in real-time
docker-compose logs -f

# Check status
docker-compose ps
```

---

## 🔐 Authentication (Do This First!)

```bash
# 1. Login to get a token
curl -X POST http://localhost:5140/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test123"}'

# Response: Copy the "token" value

# 2. Use token in requests (replace TOKEN_HERE)
curl http://localhost:5140/api/products \
  -H "Authorization: Bearer TOKEN_HERE"
```

**Pre-configured Users:**
- `admin` / `test123` (Admin role)
- `user` / `test123` (User role)

---

## 📊 API Quick Reference

### Products (Public Read)
```bash
# Get all products (no auth needed)
curl http://localhost:5140/api/products

# Filter by category
curl "http://localhost:5140/api/products?category=Electronics"

# Filter by price range
curl "http://localhost:5140/api/products?minPrice=10&maxPrice=100"
```

### Products (Protected - Requires Token)
```bash
# Create product (Admin/User)
curl -X POST http://localhost:5140/api/products \
  -H "Authorization: Bearer TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Laptop",
    "price":999.99,
    "stock":5,
    "category":"Electronics"
  }'

# Adjust stock (Admin/User)
curl -X PATCH http://localhost:5140/api/products/{ID}/stock \
  -H "Authorization: Bearer TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"quantityChange":10}'

# Deactivate product (Admin/User)
curl -X DELETE http://localhost:5140/api/products/{ID} \
  -H "Authorization: Bearer TOKEN_HERE"
```

### Users (Admin Only)
```bash
# Get all users
curl http://localhost:5140/api/users \
  -H "Authorization: Bearer TOKEN_HERE"

# Get specific user
curl http://localhost:5140/api/users/admin \
  -H "Authorization: Bearer TOKEN_HERE"

# Create user
curl -X POST http://localhost:5140/api/users \
  -H "Authorization: Bearer TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "username":"newuser",
    "password":"pass123",
    "role":"User"
  }'

# Update user
curl -X PUT http://localhost:5140/api/users/newuser \
  -H "Authorization: Bearer TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"role":"Admin"}'

# Delete user
curl -X DELETE http://localhost:5140/api/users/newuser \
  -H "Authorization: Bearer TOKEN_HERE"
```

---

## 🐳 Docker Commands

```bash
# Start services
docker-compose up --build          # Foreground with logs
docker-compose up -d --build       # Background

# Stop services
docker-compose down                # Stop containers
docker-compose down -v             # Stop + remove volumes

# View logs
docker-compose logs -f             # All logs, real-time
docker-compose logs -f api         # API logs only
docker-compose logs -f rabbitmq    # RabbitMQ logs only
docker-compose logs --tail=100     # Last 100 lines

# Manage services
docker-compose restart             # Restart all services
docker-compose restart api         # Restart specific service

# Rebuild
docker-compose build --no-cache    # Rebuild without cache

# Clean up
docker-compose down -v --rmi all   # Remove everything
```

---

## 🌐 Service URLs

| Service | URL | Purpose |
|---------|-----|---------|
| API | http://localhost:5140 | Main API endpoint |
| Swagger | http://localhost:5140/swagger | Interactive API docs |
| RabbitMQ UI | http://localhost:15672 | Message broker management |

**RabbitMQ Credentials:** guest / guest

---

## 🔄 Typical Workflow

### 1. Start Everything
```bash
docker-compose up --build
```

### 2. Login (in another terminal)
```bash
TOKEN=$(curl -s -X POST http://localhost:5140/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test123"}' | jq -r '.token')

echo $TOKEN
```

### 3. Create a Product
```bash
curl -X POST http://localhost:5140/api/products \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Gaming Mouse",
    "price":49.99,
    "stock":25,
    "category":"Accessories"
  }'
```

### 4. Get Products
```bash
curl http://localhost:5140/api/products
```

### 5. Update Stock
```bash
# Replace {ID} with actual product ID
curl -X PATCH http://localhost:5140/api/products/{ID}/stock \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"quantityChange":5}'
```

---

## 📌 Important Notes

- **Token expires in 60 minutes** - Login again if needed
- **Usernames are case-insensitive** - "Admin" and "admin" are the same
- **Passwords are case-sensitive** - "Test123" ≠ "test123"
- **Swagger is your friend** - Use http://localhost:5140/swagger for interactive testing
- **Check logs on errors** - `docker-compose logs api`

---

## ❓ Need Help?

1. **Setup issues?** → See [SETUP_VERIFICATION.md](SETUP_VERIFICATION.md)
2. **Docker questions?** → See [DOCKER_README.md](DOCKER_README.md)
3. **API documentation?** → See [README.md](README.md)
4. **Having problems?** → Check logs: `docker-compose logs`

---

## 💾 Save This Token Pattern

For scripting, you can extract and use tokens:

```bash
# Save token to variable
TOKEN=$(curl -s -X POST http://localhost:5140/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"test123"}' | grep -o '"token":"[^"]*' | cut -d'"' -f4)

# Use in subsequent requests
curl http://localhost:5140/api/users -H "Authorization: Bearer $TOKEN"
```

---

**Last Updated:** 2026-04-25  
**Project:** NewProjectFromScratch  
**Framework:** ASP.NET Core 8.0
