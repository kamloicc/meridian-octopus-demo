# Octopus Deploy Setup Guide

## Fully Automated CI/CD

The repository is configured for complete automation from GitHub to Octopus Deploy to Development environment.

## Package Names

### Helm Chart Package
- **Package Name:** `customer-api-chart`
- **Format:** `.tgz` (Helm chart archive)
- **Example:** `customer-api-chart-42.tgz`

### Docker Image Reference
- **Image:** `ghcr.io/kamloicc/customer-api`
- **Tags:** GitHub Actions run number (e.g., `42`)

### ZIP Package
- **Package Name:** `LegacyLoanProcessor`
- **Example:** `LegacyLoanProcessor.42.zip`

## Automated Workflow

### Trigger
Push to `main` branch triggers full automation:

```bash
git push origin main
```

### What Happens Automatically

1. **Build Phase**
   - Compiles both applications
   - Runs tests (if configured)
   
2. **Package Phase**
   - Creates `LegacyLoanProcessor.<run>.zip`
   - Builds and pushes `ghcr.io/kamloicc/customer-api:<run>`
   - Tags Docker image as `latest`
   - Packages `customer-api-chart-<run>.tgz`
   
3. **Push to Octopus**
   - Uploads ZIP package to Octopus built-in feed
   - Uploads Helm chart to Octopus built-in feed
   
4. **Release Creation**
   - Creates release for `meridian-legacy` project
   - Creates release for `meridian-customer-api` project
   - Both releases use run number as version
   
5. **Auto-Deploy**
   - Deploys both releases to Development environment
   - No manual intervention required

## Versioning Strategy

All versions synchronized to GitHub Actions run number:
- Docker tag: `42`
- Helm version: `42`
- Octopus release: `42`

Example for run #42:
```
Docker Image: ghcr.io/kamloicc/customer-api:42
Helm Chart: customer-api-chart-42.tgz
ZIP Package: LegacyLoanProcessor.42.zip
Octopus Release: 42
```

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
