# KMD-09-02: Set Up GitHub Actions CI/CD Pipeline

**Epic:** 09 – Deployment & Operations

## Description
Create a GitHub Actions workflow that builds, tests, and deploys the application to Azure App Service on push to the main branch. The pipeline ensures code quality and automates deployment.

## Acceptance Criteria
- [ ] A GitHub Actions workflow file exists in `.github/workflows/`
- [ ] The pipeline triggers on push to the `main` branch
- [ ] The pipeline restores NuGet packages, builds the project, and runs tests
- [ ] On successful build and test, the application is deployed to Azure App Service
- [ ] Deployment uses a publish profile or Azure credentials stored in GitHub Secrets
- [ ] The pipeline status is visible in the repository (badge optional)
- [ ] Failed builds prevent deployment
- [ ] The workflow file is well-documented with comments

## Technical Notes
- Use the `azure/webapps-deploy` GitHub Action for deployment
- Store Azure publish profile in GitHub Secrets (`AZURE_WEBAPP_PUBLISH_PROFILE`)
- Standard steps: checkout → setup-dotnet → restore → build → test → publish → deploy
- Consider adding a separate workflow for pull request validation (build + test only)
