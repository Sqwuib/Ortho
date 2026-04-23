Ortho is a enterprise-style task managment system.

Currently in very early development by Michael Pettigrew.
Updated whenever I get time.

To Run:

Backend:

cd backend/Ortho.Api

dotnet run

Frontend:

cd frontend

npm install

npm run dev

## The Plan

```
                +----------------------+
                |      React UI        |
                |  (Vite + React TS)   |
                +----------+-----------+
                           |
                           | HTTP / REST
                           v
                +----------------------+
                |   ASP.NET Core API   |
                |      (Ortho.Api)     |
                +----------+-----------+
                           |
        +------------------+------------------+
        |                                     |
        v                                     v
+---------------------+           +---------------------+
|   Application Layer |           |   Auth / Security   |
|                     |           |                     |
|  Project Service    |           | JWT Authentication  |
|  Task Service       |           | Password Hashing    |
|  User Service       |           | Authorization       |
+----------+----------+           +----------+----------+
           |                                   |
           v                                   v
+-------------------------------------------------------+
|                Infrastructure Layer                   |
|                                                       |
|  Entity Framework Core                                |
|  Repository Pattern                                   |
|  Database Context                                     |
+-------------------------+-----------------------------+
                          |
                          v
                 +-------------------+
                 |      Database     |
                 |     (SQLite)      |
                 +-------------------+

                          |
                          v
             +-------------------------------+
             | Notification Microservice     |
             |                               |
             |  Email Notifications          |
             |  Event Logging                |
             |  Audit Events                 |
             +-------------------------------+

                          |
                          v
                 +-------------------+
                 |   Message Queue   |
                 |   (Optional)      |
                 |   RabbitMQ / etc  |
                 +-------------------+
```
