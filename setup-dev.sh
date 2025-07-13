#!/bin/bash
# Development environment setup script

echo "🚀 Setting up BookStore development environment..."

# Check prerequisites
echo "📋 Checking prerequisites..."

# Check Node.js
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed. Please install Node.js 18+ and try again."
    exit 1
fi

# Check .NET
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install .NET 8.0 SDK and try again."
    exit 1
fi

echo "✅ Prerequisites check passed"

# Setup frontend
echo "📦 Setting up frontend..."
cd BookStore-frontend

if [ ! -f ".env" ]; then
    echo "🔧 Creating frontend .env file..."
    cp .env.example .env
    echo "✅ Frontend .env created from example"
fi

echo "📥 Installing frontend dependencies..."
npm install

if [ $? -eq 0 ]; then
    echo "✅ Frontend dependencies installed"
else
    echo "❌ Failed to install frontend dependencies"
    exit 1
fi

# Setup backend
echo "📦 Setting up backend..."
cd ../BookStore.API

echo "📥 Restoring backend packages..."
dotnet restore

if [ $? -eq 0 ]; then
    echo "✅ Backend packages restored"
else
    echo "❌ Failed to restore backend packages"
    exit 1
fi

# Setup root environment
cd ..
if [ ! -f ".env" ]; then
    echo "🔧 Creating root .env file..."
    cp .env.example .env
    echo "✅ Root .env created from example"
fi

echo ""
echo "🎉 Setup complete!"
echo ""
echo "📚 Next steps:"
echo "1. Configure your database connection in BookStore.API/appsettings.json"
echo "2. Run 'dotnet ef database update' in BookStore.API directory"
echo "3. Start the backend: cd BookStore.API && dotnet run"
echo "4. Start the frontend: cd BookStore-frontend && npm run dev"
echo ""
echo "🔗 Useful links:"
echo "- Frontend: http://localhost:5173"
echo "- Backend API: http://localhost:5000"
echo "- API Documentation: http://localhost:5000/swagger"
echo "- Test Tools: Open dev-tools/authorization-test.html in browser"
echo ""
echo "💡 See README.md for detailed instructions"
