# Meridian Octopus Deploy Demo

Demonstration of Octopus Deploy integration with .NET applications showing traditional ZIP deployment and containerized Kubernetes deployment patterns.

## Structure

```
src/
  LegacyLoanProcessor/  - ASP.NET Core web app (ZIP deployment)
  CustomerApi/          - ASP.NET Core API (Docker/Kubernetes)
helm/
  customer-api/         - Kubernetes Helm chart
.github/workflows/      - CI/CD automation
```

## LegacyLoanProcessor

Traditional ASP.NET Core web application demonstrating Octopus Deploy variable substitution.

The landing page displays:
- `#{Octopus.Environment.Name}` - Current environment
- `#{Octopus.Release.Number}` - Release version
- `#{LoanProcessor.ConnectionString}` - Connection string variable

Build and package:
```bash
cd src/LegacyLoanProcessor/LegacyLoanProcessor
dotnet publish -c Release -o ./publish
cd publish && zip -r LegacyLoanProcessor.zip .
```

Run locally:
```bash
dotnet run
```

## CustomerApi

ASP.NET Core Web API with health and version endpoints. Deployed as Docker container to Kubernetes.

Endpoints:
- `GET /health` - Returns health status
- `GET /version` - Returns service name, environment, and version

Environment variables:
- `MERIDIAN_ENVIRONMENT` - Set by Octopus Deploy
- `MERIDIAN_VERSION` - Set by Octopus Deploy

Run locally:
```bash
cd src/CustomerApi/CustomerApi
dotnet run
```

Docker:
```bash
docker build -t meridian/customer-api .
docker run -p 8080:8080 -e MERIDIAN_ENVIRONMENT=Local meridian/customer-api
```

## Helm Chart

Kubernetes deployment chart in `helm/customer-api/` includes:
- Deployment with environment variable injection
- Service (ClusterIP)
- ConfigMap for application configuration

Variable substitution in `values.yaml`:
```yaml
image:
  tag: "#{Octopus.Release.Number}"
environment:
  MERIDIAN_ENVIRONMENT: "#{Octopus.Environment.Name}"
```

Test chart:
```bash
helm lint helm/customer-api
helm install customer-api helm/customer-api --dry-run --debug
```

## Octopus Deploy Configuration

### LegacyLoanProcessor
1. Package as ZIP
2. Deploy to IIS or Windows Server
3. Configure variable: `LoanProcessor.ConnectionString`

### CustomerApi
1. Build and push Docker image
2. Package Helm chart
3. Deploy using Helm with variable substitution enabled

## CI/CD Pipeline

GitHub Actions workflow builds both applications:
- Compiles and packages LegacyLoanProcessor as ZIP
- Builds CustomerApi Docker image
- Packages Helm chart

Required secrets:
- `OCTOPUS_SERVER_URL`
- `OCTOPUS_API_KEY`
- `DOCKER_USERNAME`
- `DOCKER_PASSWORD`

## Requirements

- .NET 10 SDK
- Docker
- Kubernetes cluster
- Helm 3.x
- Octopus Deploy instance
