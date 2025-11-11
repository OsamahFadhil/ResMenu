# Getting Started with Menufy

## Quick Start (Recommended)

### Using Docker (Easiest Method)

1. **Make sure Docker Desktop is running**

2. **Run the startup script**
   - Windows: Double-click `start.bat`
   - Mac/Linux: Run `docker-compose up --build`

3. **Wait for all services to start** (first time may take 5-10 minutes)

4. **Access the application**:
   - **Frontend**: http://localhost:3000
   - **API**: http://localhost:5000
   - **Swagger Docs**: http://localhost:5000/swagger
   - **pgAdmin**: http://localhost:5050

## First Steps

### 1. Register a Restaurant

1. Go to http://localhost:3000
2. Click "Get Started" or "Sign up"
3. Fill in the registration form:
   - Your Name
   - Email
   - Password
   - Restaurant Name
   - Contact Phone (optional)
4. Click "Create account"

### 2. Access Dashboard

After registration, you'll be automatically redirected to the dashboard where you can:
- Generate QR codes for your restaurant
- Create menu categories
- Add menu items

### 3. Create Your Menu

1. **Add a Category**:
   - Click "Add Category"
   - Enter category name (e.g., "Appetizers", "Main Courses")
   - Set display order (1, 2, 3, etc.)
   - Click "Save Category"

2. **Add Menu Items**:
   - Items can be added to categories via the API or by extending the dashboard UI

### 4. Generate QR Code

1. In the dashboard, click "Generate QR"
2. Your QR code will be displayed
3. Customers can scan this QR code to view your menu

### 5. View Public Menu

Visit: `http://localhost:3000/menu/{your-restaurant-slug}`

The slug is generated automatically from your restaurant name.

## API Usage

### Authentication

All authenticated endpoints require a JWT token in the Authorization header:

```
Authorization: Bearer {your-token}
```

### Key Endpoints

**Register**
```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "restaurantName": "John's Restaurant",
  "contactPhone": "555-1234"
}
```

**Login**
```bash
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

**Create Category**
```bash
POST http://localhost:5000/api/restaurants/{restaurantId}/categories
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Appetizers",
  "displayOrder": 1
}
```

**Create Menu Item**
```bash
POST http://localhost:5000/api/categories/{categoryId}/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Caesar Salad",
  "description": "Fresh romaine lettuce with parmesan cheese",
  "price": 8.99,
  "imageUrl": "https://example.com/image.jpg",
  "isAvailable": true,
  "displayOrder": 1
}
```

**Generate QR Code**
```bash
GET http://localhost:5000/api/qrcode/{restaurantId}
Authorization: Bearer {token}
```

**View Public Menu**
```bash
GET http://localhost:5000/api/menu/{restaurant-slug}
```

## Testing with Swagger

1. Go to http://localhost:5000/swagger
2. Try the `/api/auth/register` endpoint first
3. Copy the token from the response
4. Click the "Authorize" button (🔒)
5. Enter: `Bearer {your-token}`
6. Now you can test authenticated endpoints

## Database Access (pgAdmin)

1. Go to http://localhost:5050
2. Login with:
   - Email: `admin@menufy.com`
   - Password: `admin`
3. Add server:
   - Name: Menufy
   - Host: `db`
   - Port: `5432`
   - Database: `menufy`
   - Username: `postgres`
   - Password: `postgres`

## Troubleshooting

### Services won't start
```bash
# Stop all containers
docker-compose down

# Remove volumes and start fresh
docker-compose down -v
docker-compose up --build
```

### Port already in use
- Make sure no other services are using ports 3000, 5000, 5432, or 5050
- Or modify ports in `docker-compose.yml`

### Database connection errors
- Wait a few seconds for PostgreSQL to fully start
- The API will automatically retry the connection

### Frontend can't connect to API
- Check that the API container is running: `docker ps`
- Verify API is accessible at http://localhost:5000

## Development Mode

### Run Backend Locally (without Docker)

```bash
cd backend/src/Menufy.API
dotnet run
```

### Run Frontend Locally (without Docker)

```bash
cd frontend
npm install
npm run dev
```

## Next Steps

1. **Customize the UI**: Edit files in `frontend/pages` and `frontend/components`
2. **Add Features**: Extend the CQRS commands/queries in the backend
3. **Deploy to Production**: Use the Docker setup with a cloud provider
4. **Add Image Upload**: Implement file storage (AWS S3, Azure Blob, etc.)
5. **Add Analytics**: Track QR code scans and menu views

## Support

- Check the main README.md for detailed documentation
- Review the code structure
- Open an issue if you encounter problems

Happy building! 🚀
