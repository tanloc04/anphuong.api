# AnPhuong Project - Backend API

**Status: In Progress (Phase 1 Completed - 60%)**

This repository contains the backend core and RESTful API services for the AnPhuong project. It handles business logic, data persistence, and serves data to the frontend application.

## Technology Stack

* Framework: ASP.NET Core
* Language: C#
* Database: SQL Server
* Architecture: N-Tier / Layered Architecture

## System Architecture

The solution is structured into distinct layers to promote separation of concerns, maintainability, and scalability:

* **anphuong.api:** The presentation layer exposing RESTful endpoints and handling HTTP requests/responses.
* **anphuong.Service:** Contains the core business logic and use cases of the application.
* **anphuong.Repository:** Manages data access and abstractions (Repository Pattern) for database interactions.
* **anphuong.Core:** Defines domain entities, interfaces, and essential DTOs (Data Transfer Objects).
* **anphuong.Utility:** Provides shared helper functions and extensions.
* **anphuong.Test:** Contains unit tests and integration tests for the system.

## Current Progress (Phase 1)

* Designed and implemented the SQL Server database schema.
* Set up the layered architecture structure.
* Developed core RESTful API endpoints for Authentication, Product Management, Category Management, User Management, Order Management, Customer Management.
* Documented system design and database ERD.

## Upcoming Features (Phase 2 Roadmap)

* Implement advanced data filtering, pagination, and sorting queries.
* Optimize database queries and setup caching mechanisms.
* Integrate third-party services (e.g., Payment Gateway, Email Provider).
* Expand unit test coverage.

## Related Resources

* **Frontend Repository:** [tanloc04/anphuong.web](https://github.com/tanloc04/anphuong.web)
* **System Documentation:** The detailed system architecture, activity diagrams, and ERD (PDF) can be found in the `docs` folder of this repository: [AnPhuongDocument.pdf](https://github.com/tanloc04/anphuong.api/blob/main/docs/AnPhuongDocument.pdf)

## Local Setup Instructions

To run this project locally, ensure you have the .NET SDK installed and a running instance of SQL Server.

1. Clone this repository:
   ```bash
   git clone [https://github.com/tanloc04/anphuong.api.git](https://github.com/tanloc04/anphuong.api.git)
   ```
2. Navigate to the API project directory:
  ```bash
  cd anphuong.api
  ```
3. Create a .env file that follow this template: 
  ```bash
  # Database Settings
  ConnectionStrings__AnPhuongFurnitureDb=Server=localhost;Database=AnPhuongFurniture;User Id=sa;Password=your_sql_password;TrustServerCertificate=True
  DB_CONNECTION=Server=localhost;Database=AnPhuongFurniture;User Id=sa;Password=your_sql_password;TrustServerCertificate=True
  
  # JWT Configuration
  Jwt__Key=your_jwt_secret_key_here
  Jwt__Issuer=http://localhost:7230
  Jwt__Audience=http://localhost:7230
  
  # OAuth & Third-party Integrations
  GOOGLE_CLIENT_ID=your_google_client_id
  ALLOWED_EMAILS=your_admin_email
  AN_PHUONG_ICON=https://your-domain.com/path-to-your-logo.jpg
  
  # SMTP Email Settings
  SMTP_SERVER=smtp.gmail.com
  SMTP_PORT=587
  SMTP_USER=your_email@gmail.com
  SMTP_PASSWORD=your_app_password
  
  # API Endpoints
  CONFIRM_ACCOUNT_ENDPOINT=http://localhost:5273/api/Customer/account-confirmation/
  
  # Cloudinary Settings
  CLOUDINARY_APIKEY=your_cloudinary_api_key
  CLOUDINARY_APISECRET=your_cloudinary_api_secret
  CLOUDINARY_CLOUDNAME=your_cloudinary_cloud_name
  ```
4. Restore dependencies and run the application:
  ```bash
  dotnet restore
  dotnet run
  ```
