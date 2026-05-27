# 🏦 Meridian Octopus Deploy Demo

A comprehensive demonstration repository showcasing Octopus Deploy integration patterns for both traditional and containerized .NET applications.

## 📁 Repository Structure

```
meridian-octopus-demo/
├── src/
│   ├── LegacyLoanProcessor/     # Traditional ASP.NET Core web app (ZIP deployment)
│   └── CustomerApi/              # Modern ASP.NET Core API (Container deployment)
├── helm/
│   └── customer-api/             # Kubernetes Helm chart with Octopus variables
├── .github/workflows/            # CI/CD pipelines
└── README.md
```

## 🚀 Applications

### 1. LegacyLoanProcessor (Traditional Deployment)

**Technology:** ASP.NET Core Razor Pages Web Application

**Deployment Method:** ZIP package to traditional servers via Octopus Deploy

**Features:**
- Displays Octopus Deploy environment information on the landing page
- Demonstrates variable substitution patterns:
  - `#{Octopus.Environment.Name}` - Current deployment environment
  - `#{Octopus.Release.Number}` - Release version number
  - `#{LoanProcessor.ConnectionString}` - Custom connection string variable

**Build & Package:**
```bash
cd src/LegacyLoanProcessor/LegacyLoanProcessor
dotnet build -c Release
dotnet publish -c Release -o ./publish
cd publish
zip -r LegacyLoanProcessor.zip .
```

**Local Development:**
```bash
cd src/LegacyLoanProcessor/LegacyLoanProcessor
dotnet run
# Navigate to http://localhost:5000
```

### 2. CustomerApi (Container Deployment)

**Technology:** ASP.NET Core Web API (.NET 10)

**Deployment Method:** Docker container to Kubernetes via Helm

**Endpoints:**
- `GET /health` - Health check endpoint
  ```json
  {
    "status": "healthy",
    "timestamp": "2026-05-27T07:00:00Z"
  }
  ```

- `GET /version` - Service version information
  ```json
  {
    "service": "Meridian Customer API",
    "environment": "Production",
    "version": "1.2.3",
    "timestamp": "2026-05-27T07:00:00Z"
  }
  ```

**Environment Variables:**
- `MERIDIAN_ENVIRONMENT` - Deployment environment (injected by Octopus)
- `MERIDIAN_VERSION` - Release version (injected by Octopus)

**Build & Run Locally:**
```bash
cd src/CustomerApi/CustomerApi
dotnet run
# Navigate to http://localhost:5000/health
# Navigate to http://localhost:5000/version
```

**Build Docker Image:**
```bash
cd src/CustomerApi/CustomerApi
docker build -t meridian/customer-api:latest .
docker run -p 8080:8080 \
  -e MERIDIAN_ENVIRONMENT="Development" \
  -e MERIDIAN_VERSION="1.0.0" \
  meridian/customer-api:latest
```

## ☸️ Helm Chart

Located in `helm/customer-api/`, this chart demonstrates Kubernetes deployment with Octopus variable substitution.

**Key Resources:**
- **Deployment:** Manages pod replicas with environment variable injection
- **Service:** ClusterIP service exposing the API
- **ConfigMap:** Application configuration data

**Octopus Variable Substitution:**
```yaml
# values.yaml
image:
  tag: "#{Octopus.Release.Number}"

environment:
  MERIDIAN_ENVIRONMENT: "#{Octopus.Environment.Name}"
  MERIDIAN_VERSION: "#{Octopus.Release.Number}"
```

**Test Helm Chart Locally:**
```bash
# Lint the chart
helm lint helm/customer-api

# Dry run to see generated manifests
helm install customer-api helm/customer-api --dry-run --debug

# Install to local Kubernetes cluster
helm install customer-api helm/customer-api \
  --set image.tag=latest \
  --set environment.MERIDIAN_ENVIRONMENT=Local \
  --set environment.MERIDIAN_VERSION=1.0.0
```

## 🔧 Octopus Deploy Configuration

### LegacyLoanProcessor Deployment Process

1. **Upload Package:** Use the packaged ZIP file
2. **Deploy to IIS/Windows Server:**
   - Extract package to deployment directory
   - Configure IIS site
   - Substitute variables in configuration files
3. **Variables to Configure:**
   - `LoanProcessor.ConnectionString` - Database connection string
   - Built-in: `Octopus.Environment.Name`, `Octopus.Release.Number`

### CustomerApi Deployment Process

1. **Push Docker Image:** Build and push to container registry
2. **Package Helm Chart:** Create `.tgz` chart package
3. **Deploy with Helm:**
   - Use "Deploy Kubernetes Helm Chart" step
   - Enable "Substitute variables in values.yaml"
   - Configure image tag: `#{Octopus.Release.Number}`
4. **Variables to Configure:**
   - Built-in: `Octopus.Environment.Name`, `Octopus.Release.Number`

### Variable Substitution Syntax

Octopus Deploy uses the `#{VariableName}` syntax for variable substitution:

```yaml
# Before substitution
version: "#{Octopus.Release.Number}"
environment: "#{Octopus.Environment.Name}"

# After substitution (Production environment, release 1.2.3)
version: "1.2.3"
environment: "Production"
```

## 🔄 CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/build-and-package.yml`) includes:

1. **Build LegacyLoanProcessor:**
   - Compile .NET application
   - Publish to output directory
   - Package as ZIP for Octopus

2. **Build CustomerApi:**
   - Build Docker image
   - Push to container registry
   - Tag with build number

3. **Package Helm Chart:**
   - Lint and package Helm chart
   - Version with build number
   - Upload as artifact

**Required Secrets:**
- `OCTOPUS_SERVER_URL` - Octopus Deploy server URL
- `OCTOPUS_API_KEY` - Octopus API key for authentication
- `DOCKER_USERNAME` - Docker Hub username
- `DOCKER_PASSWORD` - Docker Hub password/token

## 📋 Prerequisites

- .NET 10 SDK
- Docker Desktop
- Kubernetes cluster (for Helm deployment)
- Helm 3.x
- Octopus Deploy instance

## 🎯 Key Learning Points

1. **Two Deployment Patterns:**
   - Traditional: ZIP packages to VMs/servers
   - Modern: Containers to Kubernetes

2. **Variable Substitution:**
   - Environment-specific configuration
   - Release version tracking
   - Custom application settings

3. **Health & Observability:**
   - Health check endpoints
   - Version information endpoints
   - Kubernetes readiness/liveness probes

4. **GitOps-Ready:**
   - Declarative Kubernetes manifests
   - Version-controlled Helm charts
   - Automated CI/CD pipeline

## 📝 Next Steps

1. **Customize for Your Needs:**
   - Update connection strings and variables
   - Add authentication/authorization
   - Implement additional endpoints

2. **Configure Octopus Deploy:**
   - Create projects for each application
   - Set up environments (Dev, Test, Prod)
   - Configure deployment processes

3. **Enhance Observability:**
   - Add Application Insights
   - Implement structured logging
   - Add metrics endpoints

## 📚 Additional Resources

- [Octopus Deploy Documentation](https://octopus.com/docs)
- [Helm Charts Guide](https://helm.sh/docs/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Kubernetes Documentation](https://kubernetes.io/docs/)

## 📄 License

MIT License - Feel free to use this demo for learning and development purposes.

---

**Built with ❤️ for demonstrating Octopus Deploy integration patterns**
