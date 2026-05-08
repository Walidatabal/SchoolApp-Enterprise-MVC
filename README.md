# SchoolApp

SchoolApp is a production-style ASP.NET Core MVC + Web API application designed for managing students, teachers, courses, enrollments, departments, parents, classrooms, and user roles using enterprise-level architecture and best practices.

The project demonstrates real-world backend development concepts including layered architecture, authentication & authorization, service/repository patterns, structured logging, validation, and secure API development.

---

# Features

## Core Features

- ASP.NET Core MVC + RESTful Web API
- Entity Framework Core with SQL Server
- ASP.NET Identity Authentication & Authorization
- JWT Authentication
- Role-Based Authorization
- Admin Dashboard
- Teacher Approval Workflow
- Global Exception Handling
- Structured Logging with Serilog
- FluentValidation
- Swagger/OpenAPI Documentation
- Repository + Unit of Work Pattern
- DTOs & ViewModels Separation
- Layered Architecture

---

# Authentication & Authorization

- Login / Logout
- JWT Token Authentication
- ASP.NET Identity
- Role-Based Access Control

### Roles

- Admin
- Teacher
- PendingTeacher
- Student
- Parent
- User

---

# Teacher Approval Workflow

The project includes a full teacher approval workflow:

- Teacher registration
- Pending approval state
- Admin approval/rejection
- Pending Teachers page
- Approved Teachers page
- Status validation
- Logging for approval actions

### Teacher Status

- Pending
- Approved
- Rejected

---

# Main Modules

## Students Module

- Create student
- Edit student
- Delete student
- Student details
- Parent relationship support

## Teachers Module

- Create teacher
- Teacher approval flow
- Approved teachers page
- Pending teachers page

## Courses Module

- Create courses
- Assign teachers to courses
- Teacher-course relationships

## Enrollment Module

- Student enrollment
- Many-to-many relationship handling

## Parent Module

- Parent management
- Parent-student relationship

## Dashboard Module

- Statistics
- Admin overview
- Entity counters

---

# Architecture

The project follows layered architecture principles.

```text
Presentation Layer
    ↓
Controllers (MVC + API)
    ↓
Service Layer
    ↓
Repository Layer
    ↓
Entity Framework Core
    ↓
SQL Server