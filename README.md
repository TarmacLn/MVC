# University App

ASP.NET Core MVC application for managing university courses, professors, and students.
We have included a UniversityDB.dump file to use to create a database.

## Prerequisites

- .NET 10+
- PostgreSQL

## Setup

1. Configure your PostgreSQL connection in `appsettings.json`
2. Run the application:
   ```bash
   cd MVC
   dotnet run
   ```

## Features

### Secretary
- Manage users (professors and students)
- Assign courses to professors
- Enroll students in courses
- View course and student lists

### Professor
- View assigned courses with enrolled students
- Add grades (partial and final)
- View student grades per course

### Student
- View grades per semester
- View grades per course
- View total average

## Tech Stack

- ASP.NET Core MVC (.NET 10.0)
- Entity Framework Core
- PostgreSQL
- Bootstrap 5

## Made by

- Danai Charzaka P22194
- Dimitris Katsanos - P22225
- Ioanna Andrianou - P22010