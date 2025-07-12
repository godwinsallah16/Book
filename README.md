# BookStore Application

A modern, full-stack bookstore application built with .NET 8 and React. This application provides a complete e-commerce solution for buying and selling books with user authentication, shopping cart functionality, and favorites management.

## 🚀 Features

### Backend (API)
- **User Authentication & Authorization**: JWT-based authentication with email verification
- **Book Management**: CRUD operations for books with user ownership
- **Shopping Cart**: Add, update, and remove items from cart
- **Favorites**: Save and manage favorite books
- **Email Service**: Real email notifications for user verification
- **Health Checks**: API health monitoring endpoints
- **Swagger Documentation**: Interactive API documentation
- **Entity Framework**: SQL Server database with migrations
- **Logging**: Structured logging with Serilog

### Frontend (React)
- **Modern React UI**: Built with TypeScript and Vite
- **User Authentication**: Login, register, forgot password, email verification
- **Book Browsing**: Search, filter, and browse books
- **Shopping Cart**: Interactive cart with quantity management
- **Favorites**: Personal book favorites collection
- **Responsive Design**: Mobile-friendly interface
- **State Management**: Context API for global state

### Testing
- **Unit Tests**: Comprehensive test coverage with xUnit
- **Integration Tests**: API endpoint testing
- **Mock Services**: Testing with Moq framework

## 🛠️ Tech Stack

### Backend
- **.NET 8**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Database
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation
- **JWT**: Authentication tokens
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation

### Frontend
- **React 18**: Modern React with hooks
- **TypeScript**: Type-safe JavaScript
- **Vite**: Fast build tool and dev server
- **Axios**: HTTP client
- **React Router**: Client-side routing
- **CSS3**: Modern styling

### DevOps & Deployment
- **Docker**: Containerization
- **Docker Compose**: Multi-container orchestration
- **GitHub**: Version control and CI/CD ready

## 📋 Prerequisites

- **.NET 8 SDK**
- **Node.js** (v18 or higher)
- **SQL Server** (LocalDB or full instance)
- **Git**
- **Docker** (optional, for containerized deployment)

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/godwinsallah16/Book.git
cd Book
```

### 2. Backend Setup

#### Database Setup
```bash
cd BookStore.API
dotnet ef database update
```

#### Install Dependencies & Run
```bash
dotnet restore
dotnet run
```

The API will be available at `https://localhost:7000` and `http://localhost:5000`

### 3. Frontend Setup

```bash
cd BookStore-frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

### 4. Environment Configuration

#### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BookStoreDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "BookStoreAPI",
    "Audience": "BookStoreClient",
    "ExpirationHours": 24
  },
  "EmailSettings": {
    "SmtpServer": "your-smtp-server",
    "SmtpPort": 587,
    "SmtpUsername": "your-email",
    "SmtpPassword": "your-password",
    "FromEmail": "noreply@bookstore.com",
    "FromName": "BookStore"
  }
}
```

#### Frontend (.env)
```env
VITE_API_BASE_URL=https://localhost:7000/api
VITE_APP_NAME=BookStore
```

## 🐳 Docker Deployment

### Production Deployment
```bash
docker-compose -f docker-compose.prod.yml up -d
```

### Development with Docker
```bash
docker-compose up -d
```

## 🧪 Running Tests

### Backend Tests
```bash
cd BookStore.API.Tests
dotnet test
```

### Frontend Tests
```bash
cd BookStore-frontend
npm test
```

## 📚 API Documentation

Once the backend is running, visit:
- **Swagger UI**: `https://localhost:7000/swagger`
- **Health Check**: `https://localhost:7000/health`

### Key API Endpoints

#### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/verify-email` - Email verification
- `POST /api/auth/forgot-password` - Password reset

#### Books
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create new book (authenticated)
- `PUT /api/books/{id}` - Update book (authenticated)
- `DELETE /api/books/{id}` - Delete book (authenticated)

#### Cart
- `GET /api/cart` - Get user's cart
- `POST /api/cart` - Add item to cart
- `PUT /api/cart` - Update cart item
- `DELETE /api/cart/{id}` - Remove item from cart

#### Favorites
- `GET /api/favorites` - Get user's favorites
- `POST /api/favorites` - Add to favorites
- `DELETE /api/favorites/{id}` - Remove from favorites

## 🏗️ Project Structure

```
BookStore/
├── BookStore.API/                 # Backend API
│   ├── Controllers/              # API controllers
│   ├── Services/                 # Business logic
│   ├── Models/                   # Data models
│   ├── DTOs/                     # Data transfer objects
│   ├── Data/                     # Database context
│   ├── Migrations/               # EF migrations
│   └── ...
├── BookStore-frontend/            # Frontend React app
│   ├── src/
│   │   ├── components/          # React components
│   │   ├── services/            # API services
│   │   ├── types/               # TypeScript types
│   │   ├── context/             # React context
│   │   └── utils/               # Utility functions
│   └── ...
├── BookStore.API.Tests/           # Test project
├── docker-compose.yml            # Docker composition
├── docker-compose.prod.yml       # Production Docker setup
└── BookStore.sln                 # Visual Studio solution
```

## 🔧 Development

### Code Style
- **Backend**: Follow Microsoft's C# coding conventions
- **Frontend**: ESLint configuration with TypeScript rules
- **Formatting**: Prettier for consistent code formatting

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🚀 Deployment

### Production Checklist
- [ ] Update connection strings
- [ ] Configure email settings
- [ ] Set secure JWT secret keys
- [ ] Enable HTTPS
- [ ] Configure CORS for production domains
- [ ] Set up proper logging
- [ ] Configure health checks monitoring

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- **Godwin Sallah** - *Initial work* - [@godwinsallah16](https://github.com/godwinsallah16)

## 🙏 Acknowledgments

- Built with modern web technologies
- Inspired by e-commerce best practices
- Thanks to the open-source community

## 📞 Support

For support, email godwinsallah16@example.com or create an issue in this repository.

---

⭐ **Star this repo if you find it helpful!**
