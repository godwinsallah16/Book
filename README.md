# BookStore Application

A modern, full-stack bookstore application built with React, TypeScript, and .NET Core.

## 🚀 Quick Start

### Prerequisites

- Node.js 18+ and npm
- .NET 8.0 SDK
- PostgreSQL 14+
- Docker (optional)

### Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd book-store
   ```

2. **Backend Setup**
   ```bash
   cd BookStore.API
   dotnet restore
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd BookStore-frontend
   npm install
   npm run dev
   ```

4. **Database Setup**
   - Configure PostgreSQL connection in `appsettings.json`
   - Run migrations: `dotnet ef database update`

### Production Deployment

#### Using Docker Compose
```bash
docker-compose -f docker-compose.prod.yml up -d
```

#### Manual Deployment
```bash
# Backend
cd BookStore.API
dotnet publish -c Release

# Frontend
cd BookStore-frontend
npm run build:prod
```

## 🏗️ Architecture

### Frontend (React + TypeScript)
- **Framework**: React 19 with TypeScript
- **Routing**: React Router v7
- **State Management**: Context API
- **Styling**: CSS Modules
- **Build Tool**: Vite
- **HTTP Client**: Axios

### Backend (.NET Core)
- **Framework**: .NET 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT tokens
- **API**: RESTful API with Swagger documentation

### Folder Structure
```
📁 BookStore-frontend/
├── 📁 src/
│   ├── 📁 components/     # Reusable UI components
│   ├── 📁 pages/          # Page components
│   ├── 📁 context/        # React Context providers
│   ├── 📁 services/       # API service layer
│   ├── 📁 types/          # TypeScript type definitions
│   ├── 📁 utils/          # Utility functions
│   └── 📁 hooks/          # Custom React hooks
📁 BookStore.API/
├── 📁 Controllers/        # API controllers
├── 📁 Services/           # Business logic
├── 📁 Models/             # Data models
├── 📁 DTOs/               # Data transfer objects
└── 📁 Data/               # Database context
```

## 🧪 Testing

### Run Backend Tests
```bash
cd BookStore.API.Tests
dotnet test
```

### API Testing
For development testing, use the authorization test tool:
```bash
# Open dev-tools/authorization-test.html in browser
```

## 📝 Available Scripts

### Frontend
- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run build:prod` - Build with production optimizations
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
- `npm run type-check` - Run TypeScript type checking

### Backend
- `dotnet run` - Start development server
- `dotnet test` - Run tests
- `dotnet publish` - Build for production

## 🔧 Configuration

### Environment Variables

#### Frontend (.env)
```env
VITE_API_URL=http://localhost:5000/api
VITE_APP_NAME=BookStore
```

#### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-postgresql-connection-string"
  },
  "JwtSettings": {
    "SecretKey": "your-jwt-secret",
    "Issuer": "BookStore.API",
    "Audience": "BookStore.Frontend"
  }
}
```

## 🚢 Deployment

### Production Checklist
- [ ] Update environment variables
- [ ] Configure database connection
- [ ] Set up SSL certificates
- [ ] Configure reverse proxy (nginx)
- [ ] Set up monitoring and logging
- [ ] Configure backup strategy

### Docker Deployment
The application includes Docker support for easy deployment:
- `docker-compose.yml` - Development environment
- `docker-compose.prod.yml` - Production environment

## 📄 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

For questions or issues, please open a GitHub issue or contact the development team.