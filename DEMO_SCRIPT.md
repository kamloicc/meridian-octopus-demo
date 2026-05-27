# Meridian Octopus Deploy Cloud - Live Demo Script

## Demo Overview
**Duration:** 15-20 minutes  
**Audience:** Technical stakeholders, DevOps teams, Enterprise architects  
**Objective:** Showcase enterprise release orchestration across hybrid infrastructure

## Demo Setup (Before Presentation)

### Pre-Demo Checklist
- [ ] Octopus Deploy Cloud accessible: https://kamloicem.octopus.app/
- [ ] GitHub repository open: https://github.com/kamloicc/meridian-octopus-demo
- [ ] Windows Tentacle running and connected
- [ ] Kubernetes Agent connected to minikube
- [ ] Terminal/PowerShell ready
- [ ] Browser tabs prepared
- [ ] Namespace `meridian-dev` exists in minikube

## Demo Flow

### Part 1: Introduction (2 minutes)

**Script:**
> "Today I'll demonstrate Meridian's automated deployment pipeline using Octopus Deploy Cloud. Meridian is a fintech company operating hybrid infrastructure - traditional IIS applications alongside modern Kubernetes workloads."

**Key Points:**
- Legacy Loan Processor: Windows/IIS application
- Customer API: Modern containerized microservice
- Single GitHub push triggers everything
- Full automation to Development, manual gates for Production

**Show Slide/Diagram:**
```
GitHub → Actions → Octopus Cloud → [IIS Windows | Kubernetes]
```

---

### Part 2: Architecture Overview (3 minutes)

**Navigate to:** Repository README.md

**Script:**
> "Let's look at the repository structure. We have two applications with very different deployment patterns."

**Highlight:**
1. **src/LegacyLoanProcessor** - Traditional .NET web app
2. **src/CustomerApi** - Modern Web API
3. **helm/customer-api** - Kubernetes Helm chart
4. **.github/workflows** - Automated CI/CD

**Show Octopus UI:**
Navigate to: https://kamloicem.octopus.app/app#/Spaces-1/projects

**Point out:**
- Project: "meridian - legacy"
- Project: "meridian - customer api"
- Environments: Development → Test → Staging → Production

---

### Part 3: Trigger Automated Deployment (5 minutes)

**Script:**
> "I'll make a simple change and push to main. Watch as the entire deployment pipeline executes automatically."

**Terminal Commands:**
```bash
cd meridian-octopus-demo

# Make a visible change
echo "<!-- Demo timestamp: $(date) -->" >> README.md

git add .
git commit -m "Demo: Automated deployment $(date +%H:%M)"
git push origin main
```

**Navigate to:** GitHub Actions
https://github.com/kamloicc/meridian-octopus-demo/actions

**Script:**
> "GitHub Actions is now executing. Let me explain what's happening in parallel..."

**Explain Each Job:**

1. **Build LegacyLoanProcessor**
   - Compiles .NET application
   - Creates ZIP package
   - Version: GitHub run number

2. **Build CustomerApi Docker Image**
   - Builds container image
   - Pushes to GitHub Container Registry
   - Tags: run number + latest

3. **Package Helm Chart**
   - Injects Octopus variables
   - Creates versioned chart
   - All versions synchronized

4. **Push to Octopus Deploy**
   - Uploads both packages to Octopus feed
   - Packages ready for deployment

5. **Create and Deploy Releases**
   - Creates Octopus releases
   - Auto-deploys to Development

**Key Message:**
> "Notice all version numbers match - Docker tag, Helm version, Octopus release. This is release 
<run_number>."

---

### Part 4: Octopus Deployment Orchestration (4 minutes)

**Navigate to:** Octopus Dashboard

**Script:**
> "Let's watch Octopus orchestrate these deployments across our hybrid infrastructure."

**Show Octopus Dashboard:**

1. **Navigate to Releases**
   - Show newly created releases
   - Point out synchronized version numbers

2. **Open "meridian - legacy" Deployment**
   - Show deployment progress to Development
   - Expand task log
   - Highlight variable substitution

**Key Points to Mention:**
```
Variables being substituted:
- #{Octopus.Environment.Name} → "Development"
- #{Octopus.Release.Number} → "<run_number>"
- #{LoanProcessor.ConnectionString} → "<connection_string>"
```

3. **Open "meridian - customer api" Deployment**
   - Show Helm deployment steps
   - Highlight Kubernetes namespace: meridian-dev
   - Show container image reference

---

### Part 5: Validation - IIS Application (3 minutes)

**Script:**
> "Let's validate the IIS deployment on Windows."

**PowerShell Commands:**
```powershell
# Check IIS site
Get-Website | Where-Object {$_.Name -like "*meridian*"}

# Open browser to application
Start-Process "http://localhost:8080"
```

**In Browser:**
Point out displayed information:
- 🏦 Meridian Legacy Loan Processor
- Environment: **Development**
- Version: **<run_number>**
- Connection String: **<substituted_value>**

**Script:**
> "Notice how Octopus substituted our variables. The exact same package deployed to Production would show 'Production' and use different connection strings."

---

### Part 6: Validation - Kubernetes Application (3 minutes)

**Script:**
> "Now let's check the Kubernetes deployment."

**Terminal Commands:**
```bash
# Check namespace
kubectl get namespace meridian-dev

# Check deployment
kubectl get deployments -n meridian-dev

# Check pods
kubectl get pods -n meridian-dev

# Port forward
kubectl port-forward -n meridian-dev svc/customer-api-chart 8081:80 &

# Test health endpoint
curl http://localhost:8081/health

# Test version endpoint
curl http://localhost:8081/version
```

**Expected JSON Response:**
```json
{
  "service": "Meridian Customer API",
  "environment": "Development",
  "version": "<run_number>",
  "timestamp": "2026-05-27T08:00:00Z"
}
```

**Script:**
> "The containerized application is running in Kubernetes, pulling the version from environment variables injected by Octopus."

---

### Part 7: Promotion to Production (2 minutes)

**Navigate to:** Octopus Project → Releases

**Script:**
> "Now let's promote this release to Production. Notice the manual approval gate."

**Click:** "Deploy to Production"

**Show:** Manual Intervention Step
- Message: "Validate Meridian production deployment before approval."
- Requires explicit approval

**Script:**
> "In Production, we enforce manual intervention. A release manager must review and approve before deployment proceeds. This gives us automation speed for Development while maintaining production controls."

**Key Points:**
- Development: Fully automated
- Test/Staging: One-click promotion
- Production: Manual approval gate
- Full audit trail in Octopus

---

### Part 8: Rollback Demonstration (2 minutes)

**Navigate to:** Octopus Deployment History

**Script:**
> "Let me show you how easy rollback is. Octopus keeps all previous releases."

**Steps:**
1. Navigate to previous release (e.g., release N-1)
2. Click "Redeploy to Development"
3. Show deployment executing
4. Validate version rolled back

**Script:**
> "With one click, we've rolled back both applications to the previous version. All deployment history is preserved in Octopus."

---

### Part 9: Monitoring and Observability (1 minute)

**Show:**

1. **Octopus Dashboard:**
   - Deployment history
   - Audit logs
   - Task logs

2. **GitHub Container Registry:**
   - Show published images
   - Point out version tags

3. **Release Traceability:**
   - Git commit → GitHub run → Octopus release
   - Complete lineage

---

### Part 10: Q&A and Wrap-up (2 minutes)

**Key Takeaways:**
1. ✅ Single push triggers hybrid deployment
2. ✅ Synchronized versioning across all artifacts
3. ✅ Automated to Development, manual gates for Production
4. ✅ One-click rollback
5. ✅ Full audit trail and traceability
6. ✅ Works across Windows IIS and Kubernetes

**Typical Questions:**

**Q: "How long does deployment take?"**
> "About 5-7 minutes from push to deployed in Development."

**Q: "Can we deploy to Production automatically?"**
> "Yes, but we enforce manual intervention for compliance and control."

**Q: "What about rollbacks?"**
> "One-click rollback to any previous release. All packages preserved in Octopus."

**Q: "How do you handle secrets?"**
> "Octopus variables are scoped per environment. Sensitive values are encrypted."

**Q: "Can this work with other platforms?"**
> "Absolutely. Octopus supports AWS, Azure, on-prem, and more."

---

## Demo Variations

### Short Version (10 minutes)
- Skip rollback demo
- Focus on one application only
- Show deployment, skip deep dive into validation

### Deep Dive Version (30 minutes)
- Show Octopus project configuration
- Explain step templates
- Deep dive into variable scoping
- Show deployment process customization
- Demonstrate deployment targeting

### Executive Version (5 minutes)
- High-level architecture
- Trigger deployment
- Show Octopus dashboard
- Emphasize business value (speed, safety, audit)

---

## Troubleshooting During Demo

### If GitHub Actions fails:
> "Let me check the logs... [diagnose]. This is actually great - you can see how detailed the logs are for troubleshooting."

### If Deployment hangs:
> "While that completes, let me show you [alternative section]. Octopus provides real-time logs for debugging."

### If Application doesn't respond:
> "Let me check the pod status... This demonstrates Octopus's health monitoring capabilities."

---

## Post-Demo Resources

Hand out:
- Repository URL: https://github.com/kamloicc/meridian-octopus-demo
- Documentation: README.md, DEPLOYMENT_VALIDATION.md
- Demo recording (if recorded)
- Contact information for follow-up

---

## Demo Success Checklist

- [ ] Showed automated end-to-end deployment
- [ ] Demonstrated hybrid infrastructure (IIS + K8s)
- [ ] Highlighted synchronized versioning
- [ ] Showed variable substitution
- [ ] Demonstrated manual approval gate
- [ ] Showed rollback capability
- [ ] Emphasized audit trail and traceability
- [ ] Fielded questions confidently

---

**Remember:** The goal is to demonstrate enterprise-grade release orchestration that combines automation speed with production safety controls.
