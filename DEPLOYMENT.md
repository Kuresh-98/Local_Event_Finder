# Deployment Guide

This guide covers deploying the Local Event Finder application to various platforms.

## Prerequisites

- .NET 9.0 Runtime
- SQL Server (or SQL Server Express)
- Web server (IIS, Apache, Nginx, or cloud platform)

## Local Deployment

### 1. Build the Application
```bash
dotnet restore
dotnet build --configuration Release
```

### 2. Database Setup
```bash
dotnet ef database update
```

### 3. Run the Application
```bash
dotnet run --configuration Release
```

## IIS Deployment

### 1. Install Prerequisites
- .NET 9.0 Hosting Bundle
- ASP.NET Core Runtime

### 2. Publish the Application
```bash
dotnet publish --configuration Release --output ./publish
```

### 3. Configure IIS
- Create a new application pool targeting .NET CLR Version "No Managed Code"
- Create a new website pointing to the publish folder
- Set appropriate permissions

### 4. Database Configuration
Update connection string in `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LocalEventFinder;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

## Azure Deployment

### 1. Create Azure Resources
- App Service
- SQL Database
- Application Insights (optional)

### 2. Configure Connection String
Add the SQL Database connection string in Azure App Service configuration.

### 3. Deploy via Visual Studio
- Right-click project → Publish
- Select Azure App Service
- Configure and deploy

### 4. Deploy via Azure CLI
```bash
az webapp deployment source config --name YOUR_APP_NAME --resource-group YOUR_RESOURCE_GROUP --repo-url https://github.com/yourusername/local-event-finder.git --branch main --manual-integration
```

## Docker Deployment

### 1. Create Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Local Event Finder.csproj", "."]
RUN dotnet restore
COPY . .
WORKDIR "/src"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Local Event Finder.dll"]
```

### 2. Build and Run
```bash
docker build -t local-event-finder .
docker run -p 8080:80 local-event-finder
```

## Environment Configuration

### Production Settings
Create `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_PRODUCTION_CONNECTION_STRING"
  }
}
```

### Security Considerations
- Use HTTPS in production
- Configure proper CORS policies
- Set up authentication secrets
- Use environment variables for sensitive data
- Enable SQL Server encryption

## Monitoring and Logging

### Application Insights
Add to `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();
```

## Performance Optimization

### Database
- Enable connection pooling
- Configure proper indexes
- Use async/await patterns
- Implement caching where appropriate

### Application
- Enable response compression
- Configure static file caching
- Use CDN for static assets
- Implement proper error handling

## Troubleshooting

### Common Issues
1. **Database Connection**: Verify connection string and SQL Server accessibility
2. **Permissions**: Ensure proper file and database permissions
3. **Port Conflicts**: Check for port availability
4. **Missing Dependencies**: Verify .NET runtime installation

### Logs
Check application logs for detailed error information:
- Windows Event Viewer (IIS)
- Azure App Service logs
- Docker container logs

## Backup and Recovery

### Database Backup
```sql
BACKUP DATABASE LocalEventFinder TO DISK = 'C:\Backup\LocalEventFinder.bak'
```

### Application Backup
- Source code: Git repository
- Configuration: Environment-specific settings
- Static files: wwwroot folder
- Database: Regular backup schedule

## Scaling Considerations

### Horizontal Scaling
- Use load balancer
- Configure session state (if needed)
- Database connection pooling
- Stateless application design

### Vertical Scaling
- Increase server resources
- Optimize database queries
- Implement caching strategies
- Monitor performance metrics
