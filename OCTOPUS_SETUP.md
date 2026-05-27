# Octopus Deploy Setup Guide

## Package Names for Octopus

### Helm Chart Package
- **Package Name:** `customer-api-chart`
- **Format:** `.tgz` (Helm chart archive)
- **Example:** `customer-api-chart-1.0.0.tgz`

### Docker Image Reference
- **Image:** `ghcr.io/kamloicc/customer-api`
- **Tags:** Semantic versions (e.g., `1.0.0`) or `latest`

## GitHub Actions Workflow

### Automatic Builds
The workflow triggers on:
- Push to `main` or `develop` branches
- Git tags matching `v*.*.*` (e.g., `v1.0.0`)
- Pull requests to `main`

### Artifacts Generated
1. **LegacyLoanProcessor:** ZIP package for traditional deployment
2. **CustomerApi:** Docker image pushed to GitHub Container Registry
3. **Helm Chart:** `customer-api-chart-*.tgz` package

### Versioning
- **Development builds:** `1.0.0-build.<run-number>`
- **Release builds:** From Git tag (e.g., `v1.2.3` → `1.2.3`)

## Creating a Release

```bash
# Tag a release
git tag v1.0.0
git push origin v1.0.0
```

This creates:
- Docker image: `ghcr.io/kamloicc/customer-api:1.0.0`
- Helm chart: `customer-api-chart-1.0.0.tgz`

## Octopus Deploy Configuration

### Step 1: Configure Package Feed
1. In Octopus, go to Library → External Feeds
2. Add GitHub Container Registry feed
3. URL: `https://ghcr.io`
4. Add GitHub token with `read:packages` permission

### Step 2: Create Project for CustomerApi

#### Deployment Process
1. **Step 1:** Deploy Helm Chart
   - Step Type: "Deploy Kubernetes Helm Chart"
   - Package: `customer-api-chart`
   - Chart Values Override:
     ```yaml
     image:
       repository: ghcr.io/kamloicc/customer-api
       tag: "#{Octopus.Release.Number}"
     env:
       environment: "#{Octopus.Environment.Name}"
       version: "#{Octopus.Release.Number}"
     ```

#### Variables
- `Octopus.Environment.Name` - Auto-populated (e.g., "Development", "Production")
- `Octopus.Release.Number` - Auto-populated (e.g., "1.0.0")

### Step 3: Create Project for LegacyLoanProcessor

#### Deployment Process
1. **Step 1:** Deploy Package
   - Step Type: "Deploy a Package"
   - Package: `LegacyLoanProcessor`
   - Deployment target: IIS or Windows Server

#### Variables
- `LoanProcessor.ConnectionString` - Custom database connection string

## Environment Configuration

### Development
```yaml
env:
  environment: "Development"
  version: "1.0.0"
```

### Production
```yaml
env:
  environment: "Production"
  version: "#{Octopus.Release.Number}"
```

## Testing Locally

### Build Docker Image
```bash
cd src/CustomerApi/CustomerApi
docker build -t ghcr.io/kamloicc/customer-api:test .
```

### Package Helm Chart
```bash
helm package helm/customer-api --version 1.0.0
```

### Install to Kubernetes
```bash
helm install customer-api-chart helm/customer-api \
  --set image.tag=test \
  --set env.environment=Local \
  --set env.version=1.0.0-test
```

## Troubleshooting

### Image Pull Errors
If Kubernetes can't pull from GHCR:
1. Ensure package visibility is public on GitHub
2. Or create image pull secret:
```bash
kubectl create secret docker-registry ghcr-secret \
  --docker-server=ghcr.io \
  --docker-username=kamloicc \
  --docker-password=$GITHUB_TOKEN
```

### Helm Chart Errors
Validate chart before deploying:
```bash
helm lint helm/customer-api
helm template customer-api-chart helm/customer-api --debug
```

## Next Steps

1. Push a Git tag to trigger a release build
2. Download artifacts from GitHub Actions
3. Upload Helm chart to Octopus Deploy
4. Configure deployment process in Octopus
5. Create and deploy a release
