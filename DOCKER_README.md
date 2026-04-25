# Docker Setup Instructions

## Quick Start

To run the project with Docker, simply execute:

```bash
docker-compose up --build
```

This single command will:
1. Build the Docker image for the API
2. Start the RabbitMQ service
3. Start the API service
4. Set up all required networking and configuration

## Accessing the Application

- **API Endpoint**: http://localhost:5140
- **Swagger UI**: http://localhost:5140/swagger
- **RabbitMQ Management**: http://localhost:15672 (default credentials: guest/guest)

## Services Running

### API Service
- **Port**: 5140
- **Environment**: Production
- **Logs**: Available in `./logs` directory

### RabbitMQ Service
- **AMQP Port**: 5672 (for message broker)
- **Management UI Port**: 15672
- **Default Username**: guest
- **Default Password**: guest

## Common Commands

### Start services in background
```bash
docker-compose up -d
```

### Stop services
```bash
docker-compose down
```

### Stop services and remove volumes
```bash
docker-compose down -v
```

### View logs
```bash
docker-compose logs -f api
docker-compose logs -f rabbitmq
```

### Rebuild without cache
```bash
docker-compose up --build --no-cache
```

## Configuration

The environment variables are configured in `docker-compose.yml`. To change any configuration:
1. Edit the `environment` section in `docker-compose.yml`
2. Run `docker-compose up --build` to apply changes

## Notes

- The API automatically connects to RabbitMQ using the service name `rabbitmq`
- All logs are persisted in the `./logs` directory for monitoring
- The application health is checked every 30 seconds
- Both services use a custom Docker network for secure internal communication
