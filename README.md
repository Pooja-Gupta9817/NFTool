# DesktopTool – WPF + Azure Functions + EF Core

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![WPF](https://img.shields.io/badge/UI-WPF-blue)
![Azure](https://img.shields.io/badge/Backend-Azure_Functions-blue)
![EF Core](https://img.shields.io/badge/ORM-EF_Core-brightgreen)
![Unit Tests](https://img.shields.io/badge/Tests-xUnit%20%2B%20Moq-yellow)
![Theme](https://img.shields.io/badge/Theme-Light%20%26%20Dark-black)
![Status](https://img.shields.io/badge/Build-Stable-brightgreen)

---

📌 [View Feature Tracker & Roadmap](https://github.com/users/Pooja-Gupta9817/projects/2/views/1)  


## ✨ Features 

- ✅ **User Authentication** via Azure Functions (Login/Register)
- 👥 **Role-based Access** for Students and Teachers
- ☁️ **Azure SQL** integration with **EF Core**
- 📤 Upload PDFs to **Azure Blob Storage**
- 🌓 **Theme Toggle** – Light & Dark mode support
- 💉 Built-in **Dependency Injection** support
- 🧪 **Unit Testing** using xUnit + Moq
- 🤖 Developed in Visual Studio, with GitHub Copilot used to accelerate repetitive coding tasks.

---

## 🔄 Authentication Flow

1. User logs in or registers via the WPF app
2. Request hits **Azure Function** (Login/Register)
3. Data persisted in **SQL Server** via EF Core
4. JWT issued and passed back to WPF for authorized requests

---

## 📂 File Upload Flow

- WPF app uploads a file via `HttpClient`
- Azure Function saves to **Azure Blob Storage**
- Metadata logged to **SQL Database**
- JWT-based authentication ensures only authorized users can upload

---

## 📊 Planned Enhancements

| 📊 WPF Dashboard with Charts | Adds a visual analytics layer using LiveCharts or Syncfusion |
| 🔐 Secure Refresh Token Flow | Implements token renewal and session management for production-readiness |
| ⏱️ Background Jobs via Azure Queue Trigger | Offloads heavy/long-running operations asynchronously |
| ⚡ Durable Function Orchestration | Demonstrates fan-out/fan-in patterns and resilient workflow design |
| 🧾 Optional Cosmos DB Logging | Explore structured logging using NoSQL (local emulator or cloud) |

### 🚀 Real-Time Notifications with SignalR (Simulated)

**Flow Summary** (Implemented via `FakeSignalRNotifier`):

1. 📤 User uploads a file to Azure Blob Storage  
2. ⚡ Azure Function (BlobTrigger) processes the event  
3. ✏️ Function logs notification to a local file (`signalr.log`)  
4. 🖥️ WPF client watches log file and shows real-time toast notification  

---

## 🧪 Unit Testing Setup

- Test framework: **xUnit**
- Mocking: **Moq**
- Tested services:
  - `AuthService` (login/register)
  - PDF upload scenarios
- HttpClient mocked with `HttpMessageHandler`

> ✅ Test project is **UI-independent** and focuses only on business logic

---

## 🛠 Technologies Used

| Area              | Tech                                                                 |
|-------------------|----------------------------------------------------------------------|
| UI                | WPF (.NET 8)                                                         |
| Serverless APIs   | Azure Functions (HTTP triggers)                                      |
| Auth              | JWT via custom token generation                                      |
| Database          | Azure SQL with Entity Framework Core                                 |
| Storage           | Azure Blob Storage (locally via Azurite or Emulator)                 |
| Realtime          | Azure SignalR Service (planned)                                      |
| Testing           | xUnit, Moq                                                           |
| Development Tools | Git, GitHub, GitHub Copilot (Visual Studio), Azure Storage Emulator |

---

## 🧰 Local Setup Notes

- Run Azure Function using `func start`
- Use **Azurite** or Azure Storage Emulator for local blob testing
- Add `SqlConnectionString` and `AzureSignalRConnectionString` in `local.settings.json`
- Start the WPF app and test upload/login flows


### 📱 Future Roadmap

🔄 **Planned Migration to .NET MAUI**  
After all functionality is implemented and tested in WPF, the solution will be **migrated to a .NET MAUI app** for cross-platform deployment (Windows + Android). This will:
- Showcase mobile + desktop dev skills
- Allow native app notifications
- Reuse backend logic via shared service layer
---

