# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY CustomerApi.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Set environment variables (will be overridden by Kubernetes/Octopus)
ENV MERIDIAN_ENVIRONMENT="Development"
ENV MERIDIAN_VERSION="1.0.0"
ENV ASPNETCORE_URLS="http://+:8080"

# Expose port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "CustomerApi.dll"]
