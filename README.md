# .NET Core 6 Authentication API
 


This .NET Core Authentication API was created to be consumed by a Vue.js and Tailwindcss frontend. 

~
~
~
~
~

# Description
This project is a .NET Core Web API designed to handle user authentication and management. It demonstrates the implementation of JWT (JSON Web Tokens) for secure API access, alongside robust user registration and login functionalities. The API also showcases best practices in handling password security and refresh tokens.

# Features

- User Registration
- User Login
- JWT Authentication
- Refresh Token Mechanism
- Role-based Authorization (commented, for future implementation)
- Secure Password Hashing

# Getting Started

Prerequisites:

- .NET Core 3.1 SDK (or later)
- Entity Framework Core (for database operations)
- Any SQL database (the project is configured for SQL Server by default)

# Installation

Clone the repository:
- git clone https://github.com/[your-username]/dotnet-core-auth-api.git


Navigate to the project directory:
- cd dotnet-core-auth-api

Restore the necessary packages:
- dotnet restore

- Update the database connection string in appsettings.json to point to your SQL database.

Run database migrations (Ensure your database server is running):
- dotnet ef database update

Start the API:
- dotnet run

# Usage

Once the API is running, you can perform the following actions:

- Register a New User: Send a POST request to /api/Auth/Register with user credentials.
  
- Login: Send a POST request to /api/Auth/Login to receive a JWT and a refresh token.

- Access Protected Resources: Use the JWT in the Authorization header as a Bearer token.

- Refresh JWT: Send a POST request to /api/Auth/refresh-token with the refresh token.

# API Endpoints

List of available endpoints:

- POST /api/Auth/Register: Register a new user.
- POST /api/Auth/Login: Authenticate a user and receive a token.
- POST /api/Auth/refresh-token: Refresh an existing JWT.
- GET /api/Auth: Fetch all registered users (requires authorization).

