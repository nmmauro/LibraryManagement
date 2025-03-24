# LibraryManagement
A full-stack library management application built with Angular and .NET 8. This application allows librarians to manage books and customers to browse, check out books, and leave reviews.

## Features
- User authentication with role-based access (Librarian and Customer)
- Book management (add, edit, delete) for librarians
- Book browsing with filtering and sorting
- Book search functionality
- Book checkout and return system
- Customer review system
- Featured books display

## Technologies Used
- Frontend: Angular 19, TypeScript, Bootstrap 5
- Backend: .NET 8 Web API, ASP.NET Identity, Entity Framework Core
- Database: SQL Server Express

## Prerequisites
- .NET 8 SDK
- Node.js and npm
- Angular CLI
- SQL Server Express
- Visual Studio 2022

## Setup Instructions
### Database Setup
The application is configured to create and seed the database automatically. You only need to ensure SQL Server Express is running on your machine.

### Backend API Setup
1. Clone this repository
2. Open the solution file LibraryManagement.sln in Visual Studio
3. Update the connection string in appsettings.json if needed:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LibraryManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```
- Modify the Server parameter if your SQL Server instance has a different name.
4. Build and run the API project (LibraryManagementAPI)
5. The API will create the database, apply migrations, and seed it with sample data
6. The API will be available at https://localhost:7051 (or similar port shown in your console)
7. You can access Swagger documentation at https://localhost:7051/swagger

### Frontend Setup
1. Navigate to the Angular project directory:
```
cd library-management-ui
```
2. Install dependencies:
```
npm install
```
3. Update the API URL if needed:
- Open src/app/core/environment.ts
- Update the apiUrl to match your API's URL:
```
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7051/api'
};
```
4. Start the Angular development server:
```
ng serve
```
5. The frontend will be available at http://localhost:4200

## Demo Accounts
The database is seeded with two test accounts:
### Librarian:
- Email: librarian@example.com
- Password: Password123!

### Customer:
- Email: customer@example.com
- Password: Password123!

## Application Structure

### /LibraryManagementAPI - .NET 8 Web API backend
- /Controllers - API endpoints
- /Data - EF Core context and migrations
- /Models - Domain entities
- /DTOs - Data transfer objects

### /library-management-ui - Angular frontend
- /src/app/components - Angular components
- /src/app/services - API services
- /src/app/models - TypeScript interfaces
- /src/app/guards - Route guards

## Notes
- Authentication uses JWT tokens with a 60-minute expiration
- The database is automatically seeded with sample books using Bogus
- Role-based authorization restricts access to certain features based on user role

Feel free to reach out if you have any questions or issues running the application!
