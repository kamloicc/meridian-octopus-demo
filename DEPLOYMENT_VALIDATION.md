# Deployment Validation Guide

## Octopus Deploy Cloud Configuration

**Octopus URL:** https://kamloicem.octopus.app/  
**Space:** Default

## Project Configuration

### Project 1: meridian - legacy
- **Package:** LegacyLoanProcessor
- **Deployment Target:** Windows Tentacle (IIS)
- **Port:** localhost:8080
- **Package Selection:** LegacyLoanProcessor (version: #{Octopus.Release.Number})

### Project 2: meridian - customer api
- **Package:** customer-api-chart
- **Docker Image:** ghcr.io/kamloicc/customer-api
- **Deployment Target:** Kubernetes Agent (minikube)
- **Namespace:** meridian-dev
- **Endpoints:** /health, /version

## Environment Configuration

### Development (Automated)
- Auto-deployment enabled
- Receives all releases from GitHub Actions
- No approval required

### Test
- Manual deployment
- Promoted from Development

### Staging
- Manual deployment
- Promoted from Test

### Production
- **Manual Intervention required**
- Approval message: "Validate Meridian production deployment before approval."
- Promoted from Staging

## Versioning Validation

All versions synchronized to GitHub Actions run number:

```yaml
Example Run #42:
  - Docker Image: ghcr.io/kamloicc/customer-api:42
  - Helm Chart: customer-api-chart-42.tgz
  - ZIP Package: LegacyLoanProcessor.42.zip
  - Octopus Release: 42
```

## Pre-Deployment Checklist

### GitHub Secrets Configuration
- [ ] `OCTOPUS_SERVER_URL` = https://kamloicem.octopus.app/
- [ ] `OCTOPUS_API_KEY` = <your_octopus_api_key>
- [ ] `GITHUB_TOKEN` (automatically provided)

### Octopus Projects
- [ ] Project "meridian - legacy" created
- [ ] Project "meridian - customer api" created
- [ ] Both projects configured with correct packages

### Deployment Targets
- [ ] Windows Tentacle connected and healthy
- [ ] Kubernetes Agent connected to minikube
- [ ] Namespace "meridian-dev" exists

## Deployment Flow Validation

### 1. Trigger Deployment
```bash
cd meridian-octopus-demo
git add .
git commit -m "Test deployment"
git push origin main
```

### 2. GitHub Actions Validation
Monitor: https://github.com/kamloicc/meridian-octopus-demo/actions

**Expected Steps:**
- ✅ Build LegacyLoanProcessor
- ✅ Build CustomerApi Docker Image  
- ✅ Package Helm Chart
- ✅ Push Packages to Octopus Deploy
- ✅ Create and Deploy Octopus Releases

**Artifacts Created:**
- `LegacyLoanProcessor.<run>.zip`
- `ghcr.io/kamloicc/customer-api:<run>`
- `customer-api-chart-<run>.tgz`

### 3. Octopus Release Creation
Monitor: https://kamloicem.octopus.app/

**Expected:**
- Release created for "meridian - legacy" (version = run number)
- Release created for "meridian - customer api" (version = run number)
- Both releases automatically deployed to Development

### 4. IIS Deployment Validation

**Check Windows Tentacle:**
```powershell
# Verify IIS site
Get-Website | Where-Object {$_.Name -like "*meridian*"}

# Verify application running
Invoke-WebRequest -Uri "http://localhost:8080" -UseBasicParsing
```

**Expected Response:**
- Page displays "Meridian Legacy Loan Processor"
- Environment: Development
- Version: <run_number>
- Connection String: (variable substituted)

### 5. Kubernetes Deployment Validation

**Check minikube:**
```bash
# Verify namespace
kubectl get namespace meridian-dev

# Verify deployment
kubectl get deployments -n meridian-dev

# Verify pods
kubectl get pods -n meridian-dev

# Verify service
kubectl get svc -n meridian-dev

# Port forward for testing
kubectl port-forward -n meridian-dev svc/customer-api-chart 8081:80

# Test health endpoint
curl http://localhost:8081/health

# Test version endpoint
curl http://localhost:8081/version
```

**Expected Response from /version:**
```json
{
  "service": "Meridian Customer API",
  "environment": "Development",
  "version": "<run_number>",
  "timestamp": "2026-05-27T08:00:00Z"
}
```

### 6. Variable Substitution Validation

**IIS Application:**
- Environment: "Development" (from #{Octopus.Environment.Name})
- Version: <run_number> (from #{Octopus.Release.Number})
- Connection: <value> (from #{LoanProcessor.ConnectionString})

**Kubernetes Application:**
- MERIDIAN_ENVIRONMENT: "Development"
- MERIDIAN_VERSION: <run_number>

## Rollback Validation

### Test Rollback Scenario
1. Deploy release 42 to Development
2. Deploy release 43 to Development
3. Redeploy release 42 from Octopus UI

**Validation:**
- Both applications rollback to version 42
- IIS shows version 42
- Kubernetes pods show version 42
- No downtime during rollback

### Release Traceability
Check Octopus UI:
- Deployment history shows both deployments
- Audit logs capture all actions
- Task logs show detailed deployment steps

## Common Issues and Solutions

### Issue: GitHub Actions fails to push packages
**Solution:**
- Verify `OCTOPUS_API_KEY` is correct
- Check Octopus built-in feed is accessible
- Verify package names match exactly

### Issue: Release creation fails
**Solution:**
- Verify projects "meridian - legacy" and "meridian - customer api" exist
- Check project names include spaces exactly as configured
- Verify packages were pushed successfully

### Issue: IIS deployment fails
**Solution:**
- Check Windows Tentacle is online
- Verify IIS is installed and running
- Check deployment target role matches

### Issue: Kubernetes deployment fails
**Solution:**
- Verify Kubernetes Agent is healthy
- Check namespace "meridian-dev" exists
- Verify GHCR image is accessible
- Check image pull secrets if needed

### Issue: Variable substitution not working
**Solution:**
- Verify variables are defined in Octopus
- Check variable scoping (project/environment)
- Ensure structured variables are enabled for Helm

## Success Criteria

### Complete Success
- [x] GitHub Actions completes successfully
- [x] Packages pushed to Octopus feed
- [x] Releases created in Octopus
- [x] Both applications deployed to Development
- [x] IIS application accessible on localhost:8080
- [x] Kubernetes application endpoints responding
- [x] Variables properly substituted
- [x] Deployment history recorded
- [x] Rollback tested successfully

## Monitoring and Observability

### GitHub Actions
- https://github.com/kamloicc/meridian-octopus-demo/actions

### Octopus Dashboard
- https://kamloicem.octopus.app/app#/Spaces-1/overview

### GHCR Packages
- https://github.com/kamloicc?tab=packages

### Application Endpoints
- IIS: http://localhost:8080
- K8s: http://localhost:8081 (via port-forward)
