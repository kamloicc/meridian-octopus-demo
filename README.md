# Meridian Octopus Deploy Demo

Demonstration of Octopus Deploy integration with .NET applications showing traditional ZIP deployment and containerized Kubernetes deployment patterns.

## Structure

```
src/
  LegacyLoanProcessor/  - ASP.NET Core web app (ZIP deployment)
  CustomerApi/          - ASP.NET Core API (Docker/Kubernetes)
helm/
  customer-api/         - Kubernetes Helm chart (customer-api-chart)
.github/workflows/      - CI/CD automation with GHCR
```

## Quick Start

### Prerequisites
- .NET 10 SDK
- Docker
- Kubernetes cluster
- Helm 3.x
- Octopus Deploy instance

### Local Development

**CustomerApi:**
```bash
cd src/CustomerApi/CustomerApi
dotnet run
# Test: curl http://localhost:5000/health
```

**LegacyLoanProcessor:**
```bash
cd src/LegacyLoanProcessor/LegacyLoanProcessor
dotnet run
# Open: http://localhost:5000
```

## CustomerApi

ASP.NET Core Web API with health and version endpoints. Containerized and deployed to Kubernetes.

### Endpoints
- `GET /health` - Health check
- `GET /version` - Service information with environment and version

### Environment Variables
- `MERIDIAN_ENVIRONMENT` - Deployment environment
- `MERIDIAN_VERSION` - Application version

### Build Docker Image

Local build:
```bash
cd src/CustomerApi/CustomerApi
docker build -t ghcr.io/kamloicc/customer-api:latest .
docker run -p 8080:8080 \
  -e MERIDIAN_ENVIRONMENT=Development \
  -e MERIDIAN_VERSION=1.0.0 \
  ghcr.io/kamloicc/customer-api:latest
```

Test endpoints:
```bash
curl http://localhost:8080/health
curl http://localhost:8080/version
```

### Push to GitHub Container Registry

Authenticate:
```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
```

Build and push:
```bash
docker build -t ghcr.io/kamloicc/customer-api:1.0.0 .
docker push ghcr.io/kamloicc/customer-api:1.0.0
docker tag ghcr.io/kamloicc/customer-api:1.0.0 ghcr.io/kamloicc/customer-api:latest
docker push ghcr.io/kamloicc/customer-api:latest
```

## Helm Chart

Chart name: `customer-api-chart`

Location: `helm/customer-api/`

### Chart Structure
- **Deployment:** Pod management with environment variables
- **Service:** ClusterIP service exposing port 80
- **ConfigMap:** Application configuration

### Values Configuration

Default `values.yaml`:
```yaml
image:
  repository: ghcr.io/kamloicc/customer-api
  tag: "latest"

env:
  environment: "Development"
  version: "1.0.0"
```

### Deploy with Helm

Lint and validate:
```bash
helm lint helm/customer-api
```

Dry run:
```bash
helm install customer-api-chart helm/customer-api --dry-run --debug
```

Install to cluster:
```bash
helm install customer-api-chart helm/customer-api \
  --set image.tag=1.0.0 \
  --set env.environment=Production \
  --set env.version=1.0.0
```

Upgrade deployment:
```bash
helm upgrade customer-api-chart helm/customer-api \
  --set image.tag=1.0.1 \
  --set env.version=1.0.1
```

Uninstall:
```bash
helm uninstall customer-api-chart
```

### Package Helm Chart

```bash
helm package helm/customer-api --version 1.0.0 --app-version 1.0.0
# Creates: customer-api-chart-1.0.0.tgz
```

## LegacyLoanProcessor

Traditional ASP.NET Core web application with Octopus Deploy variable substitution demonstration.

### Features
Landing page displays:
- `#{Octopus.Environment.Name}` - Current environment
- `#{Octopus.Release.Number}` - Release version
- `#{LoanProcessor.ConnectionString}` - Connection string

### Build and Package

```bash
cd src/LegacyLoanProcessor/LegacyLoanProcessor
dotnet publish -c Release -o ./publish
cd publish
zip -r LegacyLoanProcessor.zip .
```

## CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/build-and-package.yml`) provides automated builds.

### Workflow Features
- Builds LegacyLoanProcessor ZIP package
- Builds and pushes Docker image to GHCR
- Packages Helm chart with synchronized versioning
- Supports semantic versioning via Git tags

### Image Tags
- `main` branch → `ghcr.io/kamloicc/customer-api:latest`
- `develop` branch → `ghcr.io/kamloicc/customer-api:develop-<sha>`
- Git tag `v1.2.3` → `ghcr.io/kamloicc/customer-api:1.2.3`

### Artifacts
- **legacy-loan-processor:** ZIP package
- **customer-api-chart:** Helm chart (.tgz)

### Release Process

Create a semantic version tag:
```bash
git tag v1.0.0
git push origin v1.0.0
```

This triggers the workflow to:
1. Build Docker image with version tag
2. Package Helm chart with matching version
3. Upload artifacts

### Required Secrets
- `OCTOPUS_SERVER_URL` - Octopus Deploy server URL
- `OCTOPUS_API_KEY` - Octopus API key
- `GITHUB_TOKEN` - Automatically provided

## Octopus Deploy Integration

### Package Names
- Helm chart: `customer-api-chart`
- Docker image: `ghcr.io/kamloicc/customer-api`

### LegacyLoanProcessor Deployment
1. Upload ZIP package to Octopus
2. Deploy to IIS or Windows Server
3. Configure variable: `LoanProcessor.ConnectionString`

### CustomerApi Deployment
1. Reference Docker image: `ghcr.io/kamloicc/customer-api:1.0.0`
2. Upload Helm chart package: `customer-api-chart-1.0.0.tgz`
3. Use "Deploy Helm Chart" step in Octopus
4. Override values:
   ```yaml
   env:
     environment: "#{Octopus.Environment.Name}"
     version: "#{Octopus.Release.Number}"
   ```

### Octopus Variables
- `Octopus.Environment.Name` - Auto-populated environment name
- `Octopus.Release.Number` - Auto-populated release version
- `LoanProcessor.ConnectionString` - Custom connection string

## Versioning Strategy

The Helm chart version and Docker image tag stay synchronized:

- **Development builds:** `1.0.0-build.<run-number>`
- **Tagged releases:** Semantic version from Git tag (e.g., `v1.2.3` → `1.2.3`)

Example:
```bash
git tag v1.2.3
git push origin v1.2.3
# Results in:
#   Docker: ghcr.io/kamloicc/customer-api:1.2.3
#   Helm: customer-api-chart-1.2.3.tgz
```
