# 📄 Document Management API

A secure, scalable RESTful API built with **.NET 8** for managing documents. This project implements clean architecture principles, including JWT authentication, Role-Based Access Control (RBAC), automated Audit Trails, and comprehensive Unit Tests.

## 🚀 Features

* **🔐 Secure Authentication:** JWT-based Login & Registration system.
* **👤 Role-Based Access:** Granular permissions for `Admin` and `User` roles.
* **📂 Document Management:** Secure Upload, Download, Listing, and Soft Delete functionality.
* **🔍 Advanced Search:** Filter documents by title, uploader, and date with Pagination support.
* **📝 Audit Trail:** Automated database-level auditing that tracks every Create, Update, and Delete action (Who, When, What changed).
* **🧪 Unit Testing:** Comprehensive tests for Services and Controllers using **xUnit**, **Moq**, and **InMemory Database**.

## 🛠 Tech Stack

* **Framework:** .NET 8 (C#)
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core
* **Testing:** xUnit, Moq
* **Containerization:** Docker (for Database)

---

## ⚙️ Getting Started

Follow these steps to set up and run the project locally.

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 1. Clone the Repository
```bash
git clone [https://github.com/wuttachaichumyen/DocumentAPI.git](https://github.com/wuttachaichumyen/DocumentAPI.git)
cd DocumentAPI
