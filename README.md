FRUIT SOUTH AFRICA DEVELOPER TEST
Overview
This project is a scalable and secure web application to manage products and categories for users. The application allows users to register, log in, and perform CRUD (Create, Read, Update, Delete) operations on products and categories.

Technologies Used
Language: C#
Framework: ASP.NET Core MVC
Frontend: JavaScript, jQuery, Bootstrap
Backend: ASP.NET Core 
ORM: Entity Framework Core
Database: SQL Server or MySQL
Features
User Authentication:

User registration with email and password
User login with email and password
Category Management:

View, add, edit, delete categories
Unique category code format validation (e.g., ABC123)
Product Management:

View, add, edit, delete products with pagination (page size of 10)
Unique product code format (yyyyMM-###)
Image upload for products
Import/export products via Excel spreadsheet
Installation
Prerequisites
.NET Core SDK
SQL Server

Setup Instructions
Clone the repository:

git clone https://github.com/yourusername/ProductManagementAPP.git
cd ProductManagementAPP

Configure the database:

Update the appsettings.json file with your database connection string.

"ConnectionStrings": {
  "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
}
Apply database migrations:

dotnet ef database update
Run the application:

dotnet run
Running Tests
To run the unit tests, use the following command:

sh
Copy code
dotnet test
Security
Authentication and Authorization: Implemented using ASP.NET Core Identity
Data Validation: Data annotations and custom validation attributes
Exception Handling: Global exception handling for managing unexpected errors
Architecture
The application follows an N-Tier architecture:

Presentation Layer: ASP.NET Core MVC
Data Access Layer: Entity Framework Core
Database: SQL Server or MySQL
Contributing
Contributions are welcome! Please fork the repository and create a pull request with your changes.
