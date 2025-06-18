# DesktopTool – WPF + Azure Functions + EF Core

A modern desktop application built with **WPF (.NET 8)**, **Azure Functions**, and **EF Core**, designed to demonstrate secure login and registration using cloud-first principles.

---

## ✨ Features 

- ✅ **User Authentication** via Azure Function (Login/Register)
- 🔐 **Role-based Registration** (Student/Teacher)
- ☁️ Azure SQL Database Integration using EF Core
- 💡 Clean MVVM Architecture + Dependency Injection

## To-Do / Roadmap
 Add token-based authentication

 Improve UI with animations & validation

 Add unit tests for services and functions

 Host Azure Function in production


## 🖥 Technologies Used

- WPF (.NET 8)
- Azure Functions (HTTP Trigger)
- Entity Framework Core
- Azure SQL Database
- MVVM Pattern
- Dependency Injection (DI)
- Git + GitHub

🔒 Authentication Flow
User selects Login or Register

For Register, chooses role: Student or Teacher

Credentials sent via HttpClient to Azure Function

Function calls EF Core to query/insert into Azure SQL

Returns success/failure message to WPF UI



