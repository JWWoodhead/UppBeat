# UppBeat
API allowing users to perform standard CRUD operations against Track data

### Features

- Token-based authentication and authorization
- Begins of Role-based access control (Anonymous and Artist currently)
- Track management with proper artist ownership
- Pagination and filtering support for track retrieval
- Swagger documentation
- Docker containerization

### Tech stack

- .NET 8.0
- Entity Framework Core
- SQL Server
- Docker

### API specification

The full API specification can be accesed by launching API (see "Launch via Docker" section below) and browsing to https://localhost:8081/swagger/index.html

### Database setup

The UppBeat API uses EntityFrameworkCore migration scripts to maintain schema. This should ideally moved to a better implementation out of the main runtime process when we push to Production

Pre-requisite - install dotnet EF tools
```bash
dotnet tool install --global dotnet-ef
```

Add a new generated migration script (requires tool above):
```bash
dotnet ef migrations add NameOfNewMigrationScript
```

Update database to latest version of migration scripts
```bash
dotnet ef database update
```

### Launch via Docker

A pre-existing docker-compose file already exists for the solution. The API and a SQL server container will be be created as part of this

```bash
docker-compose up
```

### Future functionality

- Allow tracks to be downloaded from filename specified against track. This will likely be stored in S3 if AWS is used and retrieved via a pre-signed URL
- Add custom policy for track downloading which should apply to both Standard users and Artists (potentially using custom user roles)
- Add k6 for basic load testing once a persisted DB has been created and associated rate limiting tests
- Create terraform implementation for pushing this to whichever Cloud provider is chosen
