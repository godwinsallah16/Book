# Contributing to BookStore

Thank you for your interest in contributing to BookStore! This document provides guidelines and information for contributors.

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 8.0 SDK
- PostgreSQL 14+
- Git

### Development Setup

1. **Fork and clone the repository**
   ```bash
   git clone https://github.com/your-username/bookstore.git
   cd bookstore
   ```

2. **Install dependencies**
   ```bash
   # Frontend
   cd BookStore-frontend
   npm install

   # Backend
   cd ../BookStore.API
   dotnet restore
   ```

3. **Set up environment variables**
   ```bash
   # Copy example files and update with your values
   cp BookStore-frontend/.env.example BookStore-frontend/.env
   cp .env.example .env
   ```

4. **Start development servers**
   ```bash
   # Terminal 1: Backend
   cd BookStore.API
   dotnet run

   # Terminal 2: Frontend
   cd BookStore-frontend
   npm run dev
   ```

## 📝 Development Guidelines

### Code Style

#### Frontend (React/TypeScript)
- Use TypeScript for all new code
- Follow the existing ESLint and Prettier configurations
- Use functional components with hooks
- Implement proper error handling
- Write descriptive component and function names

#### Backend (.NET Core)
- Follow C# naming conventions
- Use async/await for asynchronous operations
- Implement proper exception handling
- Add XML documentation for public APIs
- Follow SOLID principles

### Commit Message Format

Use conventional commits:

```
type(scope): description

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(auth): add password reset functionality
fix(cart): resolve quantity update bug
docs(readme): update installation instructions
```

### Branch Naming

Use descriptive branch names:
- `feature/add-user-profile`
- `fix/cart-quantity-bug`
- `docs/api-documentation`
- `refactor/auth-service`

## 🧪 Testing

### Frontend Testing

```bash
cd BookStore-frontend
npm run lint
npm run type-check
npm run format:check
```

### Backend Testing

```bash
cd BookStore.API.Tests
dotnet test
```

### Integration Testing

Use the development tools:
```bash
# Open dev-tools/authorization-test.html in browser
```

## 📋 Pull Request Process

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Write clean, documented code
   - Add tests if applicable
   - Update documentation

3. **Test your changes**
   ```bash
   # Frontend
   npm run lint
   npm run type-check
   npm run build

   # Backend
   dotnet test
   dotnet build
   ```

4. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat: add your feature description"
   ```

5. **Push and create PR**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create Pull Request**
   - Use the PR template
   - Provide clear description
   - Link related issues
   - Add screenshots if applicable

### PR Checklist

- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Tests added/updated if applicable
- [ ] Documentation updated
- [ ] No breaking changes (or clearly documented)
- [ ] All checks passing

## 🐛 Bug Reports

### Before Submitting

1. Check existing issues
2. Test with latest version
3. Gather reproduction steps

### Bug Report Template

```markdown
**Bug Description**
Clear description of the bug

**Steps to Reproduce**
1. Step one
2. Step two
3. Step three

**Expected Behavior**
What should happen

**Actual Behavior**
What actually happens

**Environment**
- OS: [e.g., Windows 11]
- Browser: [e.g., Chrome 120]
- Node.js: [e.g., 18.17.0]
- .NET: [e.g., 8.0]

**Additional Context**
Screenshots, logs, etc.
```

## 💡 Feature Requests

### Feature Request Template

```markdown
**Feature Description**
Clear description of the feature

**Problem Statement**
What problem does this solve?

**Proposed Solution**
How should it work?

**Alternatives Considered**
Other solutions you've considered

**Additional Context**
Mockups, examples, etc.
```

## 📚 Resources

### Documentation
- [React Documentation](https://react.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Vite Documentation](https://vitejs.dev/)

### Project Structure
- `/BookStore-frontend` - React frontend application
- `/BookStore.API` - .NET Core backend API
- `/BookStore.API.Tests` - Backend unit tests
- `/dev-tools` - Development utilities

## 🤝 Community

### Code of Conduct

- Be respectful and inclusive
- Welcome newcomers
- Focus on constructive feedback
- Help others learn and grow

### Getting Help

- Check documentation first
- Search existing issues
- Ask questions in discussions
- Join our community channels

## 📄 License

By contributing, you agree that your contributions will be licensed under the same license as the project.

## 🙏 Recognition

Contributors will be acknowledged in:
- README.md contributors section
- Release notes
- Project documentation

Thank you for contributing to BookStore! 🚀
