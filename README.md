# UppBeat
API allowing users to perform standard CRUD operations against Track data

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