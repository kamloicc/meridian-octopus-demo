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

## CI/CD Pipeline - Fully Automated & Idempotent

GitHub Actions workflow (`.github/workflows/build-and-package.yml`) provides complete automation from commit to deployment with full idempotency support.

### Automated Workflow
When you push to the `main` branch:

1. **Build Applications**
   - Compiles LegacyLoanProcessor
   - Builds multi-architecture CustomerApi Docker image (AMD64 + ARM64)
   
2. **Package Artifacts**
   - Creates `LegacyLoanProcessor.20.0.1.zip`
   - Pushes Docker image to GHCR as `ghcr.io/kamloicc/customer-api:20.0.1`
   - Tags Docker image as `latest`
   - Packages Helm chart as `customer-api-chart-20.0.1.tgz`
   
3. **Push to Octopus Deploy** (Idempotent)
   - Uploads ZIP package to Octopus built-in feed (overwrites if exists)
   - Uploads Helm chart to Octopus built-in feed (overwrites if exists)
   - Uses `overwrite_mode: OverwriteExisting` for safe reruns
   
4. **Create Octopus Releases** (Idempotent)
   - Creates release `20.0.1` for `meridian-legacy` project
   - Creates release `20.0.1` for `meridian-customer-api` project
   - Uses `ignore_existing: true` to skip if already exists
   
5. **Auto-Deploy to Development**
   - Deploys both releases to Development environment automatically
   - Redeployable without errors

### Semantic Versioning
Current version: **20.0.1**

All version numbers are synchronized using semantic versioning:
- Docker image tag: `20.0.1` (and `latest`)
- Helm chart version: `20.0.1`
- Octopus release number: `20.0.1`
- ZIP package version: `20.0.1`

Example artifacts:
- `ghcr.io/kamloicc/customer-api:20.0.1`
- `customer-api-chart-20.0.1.tgz`
- `LegacyLoanProcessor.20.0.1.zip`
- Octopus Releases: `20.0.1`

### Idempotent Pipeline Benefits
The workflow is fully rerun-safe:
- **Package uploads:** Overwrites existing packages automatically
- **Release creation:** Ignores if release already exists
- **Deployments:** Can be redeployed safely
- **No manual cleanup:** Just push and run again

### Simple Deployment Flow
```bash
git add .
git commit -m "Your changes"
git push origin main
```

This single push automatically:
- Builds everything
- Publishes to GHCR
- Pushes packages to Octopus
- Creates releases
- Deploys to Development

### Required Secrets
Configure these in GitHub Settings → Secrets:
- `OCTOPUS_SERVER_URL` - Your Octopus Deploy server URL
- `OCTOPUS_API_KEY` - Octopus API key with permission to push packages and create releases
- `GITHUB_TOKEN` - Automatically provided by GitHub Actions

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

All artifacts use **semantic versioning** (MAJOR.MINOR.PATCH) controlled by the VERSION environment variable in the workflow.

### Current Version: 20.0.1

All artifacts are synchronized to this version:
- Docker image: `ghcr.io/kamloicc/customer-api:20.0.1`
- Helm chart: `customer-api-chart-20.0.1.tgz`
- ZIP package: `LegacyLoanProcessor.20.0.1.zip`
- Octopus releases: `20.0.1`

### Updating Version

To update to a new version, edit the VERSION in `.github/workflows/build-and-package.yml`:

```yaml
env:
  VERSION: 20.0.2  # Increment as needed
```

**Semantic Versioning Guide:**
- **PATCH** (20.0.1 → 20.0.2): Bug fixes, hotfixes
- **MINOR** (20.0.2 → 20.1.0): New features, backward compatible
- **MAJOR** (20.1.0 → 21.0.0): Breaking changes

The workflow ensures all artifacts use the same version number, preventing version drift across the deployment pipeline.
