# Task Manager - Full Stack Application

## Task Overview

### 1. Backend (.NET Core API)

Build a REST API with the following functionality:

**User registration and authentication**
- Use simple JWT authentication.

**Task CRUD operations (Task entity):**
- Fields: Title, Description, Status (New, InProgress, Done), DueDate.

**Filtering:**
- Filter tasks by status and date.

**Database:**
- Use EF Core with SQLite, MSSQL, or PostgreSQL (any is fine — persistence required).

### 2. Frontend (Angular 15+)

Build a frontend with:

**Login form (JWT-based auth).**

**Task list with filters:**
- Filter by status and date.

**Add / Edit / Delete task features.**

**Simple, clean UI (no need for fancy design).**

**Pagination for the task list.**

### 3. Simple unit tests (either backend or frontend)

## How to Run

### Database Setup (First Time Only)
cd Api <br>
dotnet ef migrations add InitialCreate <br>
dotnet ef database update

### Trust Development Certificate (First Time Only)
dotnet dev-certs https --trust

### Backend (API)
cd Api <br>
dotnet restore <br>
dotnet build <br>
dotnet run

The API will start on:
- HTTPS: https://localhost:7000/
- HTTP: http://localhost:5000/

### Frontend (Angular)
cd Frontend <br>
npm install <br>
npm run start

The frontend will start on http://localhost:4200/

### Tests
cd Tests <br>
dotnet restore <br>
dotnet test
