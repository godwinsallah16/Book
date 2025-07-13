# Books Feature

This folder contains all components, pages, and logic related to book management and display in the BookStore frontend.

## Main Components
- **Favorites**: Allows users to view and manage their favorite books.
- **BookFormPage**: Page for adding or editing book details.
- **BookList**: Displays a list of available books.
- **BookDetails**: Shows detailed information about a selected book.

## Key Functionality
- Fetching and displaying books from the backend API.
- Managing user favorites and book interactions.
- Supporting add/edit operations for books (admin functionality).

## Related Services & Types
- Uses `favoritesService` for favorite management.
- Relies on shared types from the central `types` folder.

## Conventions
- Each feature folder should include a README.md describing its purpose and structure.
- Components should be organized by feature for maintainability.
- Use TypeScript for type safety and consistency.

## Usage
Import components and hooks from this folder to build book-related pages and functionality.

---
For more details, see the code in each component and the main README in the frontend root.

_This README is provided for consistency across feature folders._
