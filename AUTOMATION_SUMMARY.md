# Automated CI/CD Pipeline Summary

## Overview
This repository implements a fully automated CI/CD pipeline from GitHub to Octopus Deploy to Development environment.

## Simplified Versioning
All components use the **GitHub Actions run number** as the version:
- Docker Image: `ghcr.io/kamloicc/customer-api:42`
- Helm Chart: `customer-api-chart-42.tgz`
- ZIP Package: `LegacyLoanProcessor.42.zip`
- Octopus Release: `42`

## Automation Flow

### 1. Developer Workflow
```bash
git add .
git commit -m "Feature update"
git push origin main
```

### 2. GitHub Actions (Automatic)
- **Builds** both applications
- **Creates** Docker image with run number tag
- **Pushes** Docker image to GHCR
- **Packages** Helm chart with run number version
- **Packages** ZIP file with run number
- **Pushes** all packages to Octopus built-in feed
- **Creates** Octopus releases for both projects
- **Deploys** both releases to Development

### 3. Result
Within minutes, your changes are:
- Built and packaged
- Published to registries
- Deployed to Development environment

## Key Features

### Synchronized Versioning
```yaml
Run #42:
  Docker: ghcr.io/kamloicc/customer-api:42
  Helm:   customer-api-chart-42.tgz
  ZIP:    LegacyLoanProcessor.42.zip
  Release: 42
```

### Octopus Variable Substitution
Helm values are automatically configured:
```yaml
env:
  environment: "#{Octopus.Environment.Name}"
  version: "#{Octopus.Release.Number}"
```

### Auto-Deployment
- Development environment receives updates automatically
- Production deployments remain manual (promote from Octopus UI)

## Required Octopus Projects

### Project: meridian-legacy
- Package: `LegacyLoanProcessor`
- Deployment: IIS/Windows Server
- Variables: `LoanProcessor.ConnectionString`

### Project: meridian-customer-api
- Package: `customer-api-chart`
- Docker Image: `ghcr.io/kamloicc/customer-api`
- Deployment: Kubernetes via Helm
- Variables: Environment-specific overrides

## Environments

### Development (Automated)
- Receives all main branch commits automatically
- Used for integration testing
- Variables: Development settings

### Production (Manual)
- Promoted from Octopus UI
- Requires approval
- Variables: Production settings

## Monitoring

Check deployment status:
- **GitHub Actions:** https://github.com/kamloicc/meridian-octopus-demo/actions
- **Octopus Dashboard:** Your Octopus Deploy instance
- **GHCR Packages:** https://github.com/kamloicc?tab=packages

## Troubleshooting

### Build Failures
Check GitHub Actions logs for errors

### Deployment Failures
Check Octopus Deploy task logs

### Package Not Found
Verify packages were pushed to Octopus feed successfully

## Next Steps

1. Configure GitHub Secrets:
   - `OCTOPUS_SERVER_URL`
   - `OCTOPUS_API_KEY`

2. Create Octopus Projects:
   - `meridian-legacy`
   - `meridian-customer-api`

3. Configure Development environment in Octopus

4. Push to main branch to trigger automation
