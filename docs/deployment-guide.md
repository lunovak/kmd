# KMD Deployment Guide – Azure App Service & Azure SQL

## Prerequisites

- Azure subscription
- Azure CLI installed (`az` command)
- .NET 10 SDK installed locally

## 1. Create Azure Resources

### Resource Group

```bash
az group create --name rg-kmd --location westeurope
```

### Azure SQL Database (Free Tier)

```bash
az sql server create \
  --name kmd-sql-server \
  --resource-group rg-kmd \
  --location westeurope \
  --admin-user kmdadmin \
  --admin-password '<YourStrongPassword>'

az sql db create \
  --resource-group rg-kmd \
  --server kmd-sql-server \
  --name KmdDb \
  --edition Free

az sql server firewall-rule create \
  --resource-group rg-kmd \
  --server kmd-sql-server \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

> **Free tier limitations:** 32 GB max, 5 DTU, automatic pause after 1 hour of inactivity, limited to one free DB per subscription.

### Azure App Service

```bash
az appservice plan create \
  --name kmd-plan \
  --resource-group rg-kmd \
  --sku F1 \
  --is-linux

az webapp create \
  --resource-group rg-kmd \
  --plan kmd-plan \
  --name kmd-web-app \
  --runtime "DOTNETCORE:10.0"
```

## 2. Configure App Settings

```bash
az webapp config connection-string set \
  --resource-group rg-kmd \
  --name kmd-web-app \
  --settings DefaultConnection="Server=tcp:kmd-sql-server.database.windows.net,1433;Database=KmdDb;User ID=kmdadmin;Password=<YourStrongPassword>;Encrypt=True;TrustServerCertificate=False;" \
  --connection-string-type SQLAzure

az webapp config appsettings set \
  --resource-group rg-kmd \
  --name kmd-web-app \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    AdminSeed__Email=admin@kmd.local \
    AdminSeed__Password='<YourAdminPassword>' \
    Email__AzureCommunicationConnectionString='<ACS-ConnectionString>' \
    Email__SenderAddress='DoNotReply@yourdomain.com'
```

## 3. Deploy the Application

### Manual Deployment

```bash
cd src/Kmd.Web
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip .
az webapp deploy --resource-group rg-kmd --name kmd-web-app --src-path ../deploy.zip --type zip
```

### Via GitHub Actions

See `.github/workflows/deploy.yml` – push to `main` triggers automatic deployment.

## 4. Database Migrations

Migrations are applied automatically on application startup via `Database.MigrateAsync()` in `Program.cs`. No separate migration step is needed.

## 5. Environment-Specific Configuration

| Setting | Development | Production |
|---------|------------|------------|
| Connection String | LocalDB (`appsettings.json`) | Azure SQL (App Service config) |
| Email Service | `LoggingEmailService` (logs only) | `AzureCommunicationEmailService` |
| HTTPS | Enforced via HSTS | Enforced via HSTS + App Service |
| Logging | Detailed errors | Warning level + App Insights |
| Error Pages | Developer exception page | `/Error` page |

## 6. Verify Deployment

1. Browse to `https://kmd-web-app.azurewebsites.net`
2. Log in with the admin credentials configured in app settings
3. Navigate to Admin > Email Test to verify email configuration
4. Check Azure Portal > App Service > Log stream for application logs

## Troubleshooting

- **502 errors**: Check App Service logs via `az webapp log tail --name kmd-web-app --resource-group rg-kmd`
- **Database connection failures**: Verify firewall rules allow Azure services, check connection string
- **Missing migrations**: The app runs `MigrateAsync()` on startup – check logs for EF Core errors
