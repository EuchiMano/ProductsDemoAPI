# Setup Verification Guide

This guide helps you verify that everything is working correctly after running `docker-compose up --build`.

---

## ✅ Step 1: Verify Containers are Running

Run this command to check if all services are up:

```bash
docker-compose ps
```

**Expected Output:**
```
NAME                    STATUS              PORTS
newproject_api          Up (healthy)        0.0.0.0:5140->5140/tcp
newproject_rabbitmq     Up (healthy)        0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
```

✅ **Success**: Both containers show "Up (healthy)"  
❌ **Issue**: If either container is not running, check logs: `docker-compose logs`

---

## ✅ Step 2: Test API Connectivity

Open your browser or use curl to verify the API is accessible:

**Browser:** http://localhost:5140/swagger

**or cURL:**
```bash
curl http://localhost:5140/swagger
```

✅ **Success**: Swagger UI loads with all endpoints listed  
❌ **Issue**: If you get "Connection refused", API might still be starting. Wait 10 seconds and try again.

---

## ✅ Step 3: Test Authentication

Try logging in with the pre-configured admin user:

```bash
curl -X POST http://localhost:5140/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "test123"
  }'
```

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "role": "Admin"
}
```

✅ **Success**: You receive a JWT token  
❌ **Issue**: If you get "Invalid username or password", verify credentials (case-sensitive)

---

## ✅ Step 4: Test Public Products Endpoint

Verify you can fetch products without authentication:

```bash
curl http://localhost:5140/api/products
```

**Expected Response:**
```json
[]
```
(Empty array if no products yet, or list of products if any exist)

✅ **Success**: You receive a JSON response  
❌ **Issue**: Check API logs: `docker-compose logs api`

---

## ✅ Step 5: Test Protected Endpoint

Use the token from Step 3 to create a product:

```bash
# Replace YOUR_TOKEN_HERE with the actual token from Step 3
curl -X POST http://localhost:5140/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Product",
    "price": 29.99,
    "stock": 10,
    "category": "Test"
  }'
```

**Expected Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Test Product",
  "price": 29.99,
  "stock": 10,
  "category": "Test",
  "isActive": true
}
```

✅ **Success**: Product created successfully  
❌ **Issue**: 
- "401 Unauthorized" = Token expired or malformed
- "400 Bad Request" = Check request body format
- Check API logs: `docker-compose logs api`

---

## ✅ Step 6: Test RabbitMQ

Verify RabbitMQ management UI is accessible:

**Browser:** http://localhost:15672

**Credentials:**
- Username: `guest`
- Password: `guest`

✅ **Success**: RabbitMQ Dashboard loads  
❌ **Issue**: If inaccessible, check RabbitMQ logs: `docker-compose logs rabbitmq`

---

## ✅ Step 7: Test Swagger UI (Interactive)

1. Navigate to: http://localhost:5140/swagger
2. Click the **Authorize** button (top right)
3. Paste a valid token from Step 3
4. Click **Authorize**
5. Try the endpoints interactively

✅ **Success**: You can execute API calls from Swagger UI  
❌ **Issue**: Check browser console for errors (F12)

---

## ✅ Complete!

If all steps above pass, your environment is properly configured and ready to use.

---

## 🆘 Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| Containers won't start | Run `docker-compose down -v` then `docker-compose up --build` |
| Port 5140 already in use | Stop other services or change port in `docker-compose.yml` |
| Can't connect to RabbitMQ | Wait 15 seconds for startup, then restart: `docker-compose restart rabbitmq` |
| API won't connect to RabbitMQ | Verify `RabbitMq__Host=rabbitmq` in docker-compose.yml |
| Logs are too verbose | Use `docker-compose logs api --tail=50` for last 50 lines |
| Want to clear everything | Run `docker-compose down -v --rmi all` |

---

## 📝 Next Steps

After verification:
1. Read the main [README.md](README.md) for full API documentation
2. Try the example workflow in the README
3. Integrate with your application
4. Check [DOCKER_README.md](DOCKER_README.md) for advanced Docker commands

---

## 💡 Pro Tips

- Use Swagger UI at http://localhost:5140/swagger for interactive testing
- Check real-time logs: `docker-compose logs -f api`
- Keep tokens handy: They expire after 60 minutes (configurable)
- For development, use: `docker-compose up` (not `-d`) to see all logs in real-time
