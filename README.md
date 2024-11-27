# Bank Management App

Welcome to the Bank Management App Backend API repository! This backend is built with ASP.NET Core 8, Entity Framework, JWT Authentication, and uses MS SQL Server for data storage. The API exposes several endpoints to handle banking functionalities like user authentication, account management, transactions, loan management, and admin operations.

Features

1. User Authentication and Authorization

JWT Authentication: Secure API access using JWT tokens for user authentication and authorization.

Role-based Access Control: Different roles such as customer, bank employee, and administrator with access restrictions based on roles.


2. Account Management

CRUD operations for managing accounts, including:

Checking, savings, and loan accounts.

Viewing account details (balance, history, etc.).


3. Transactions and Fund Transfers

Perform operations like:

Deposit, withdrawal, and internal/external fund transfers.

Transaction limits and fees for account types.

View transaction history for each account.



4. Loan Management

Application for loans, with fields such as loan type, amount, and term.

Loan approval workflow for admin review.


5. Admin Dashboard

Admin can manage users, reset passwords, and control account status.


Tech Stack

Backend Framework: ASP.NET Core 8

Database: MS SQL Server

ORM: Entity Framework Core

Authentication: JWT (JSON Web Tokens)

Libraries/Tools:

Microsoft.AspNetCore.Authentication.JwtBearer

Microsoft.EntityFrameworkCore

Swashbuckle.AspNetCore (for API documentation)

Installation

1. Clone the Repository

git clone https://github.com/your-username/Bank_Management_App_Backend.git
cd Bank_Management_App_Backend

2. Set Up the Database

1. Install and set up MS SQL Server.


2. Create a database named BankManagementApp (or use an existing one).


3. Update the connection string in appsettings.json:



"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=BankManagementApp;User Id=your-username;Password=your-password;"
}

3. Install Dependencies

Run the following command to install the required NuGet packages:

dotnet restore

4. Migrate the Database

Run the migrations to create the necessary tables in the database:

dotnet ef database update

5. Run the Application

To start the backend API, use the following command:

dotnet run

API Documentation : Run Swagger

Authentication (JWT)

After logging in, the API will return a JWT token that must be included in the header of all subsequent requests.

Example header for requests:


Authorization: Bearer <your-jwt-token>

Running Tests

To run unit tests, use the following command:

dotnet test

Logging

The backend uses Serilog for logging.

Logs are stored in the console output and can be configured to log to files or other storage systems.


Environment Variables

Make sure to set the following environment variables in the appsettings.json or through your environment configuration:

JWT_SECRET: The secret key used to sign JWT tokens.

ConnectionStrings.DefaultConnection: The connection string to your MS SQL Server database.


For any questions or issues, feel free to open an issue or submit a pull request.

