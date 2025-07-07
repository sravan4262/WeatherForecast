# WeatherForecast Project - Personal Learning & Experimentation

> **Disclaimer:**  
> This project is developed purely for personal learning, experimentation, and demonstrating cloud-native concepts on Azure. It is **not recommended** for production usage reference or deployment.

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Components](#components)
    - [.NET Web API](#net-web-api)
    - [Azure Function](#azure-function)
    - [SQL Database Project](#sql-database-project)
    - [Console App (OAuth Tester)](#console-app-oauth-tester)
    - [Unit Tests](#unit-tests)
4. [Azure Resources](#azure-resources)
5. [Authentication & Authorization](#authentication--authorization)
6. [CI/CD Pipeline](#cicd-pipeline)
7. [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Local Development](#local-development)
    - [Deployment (CI/CD)](#deployment-cicd)
8. [Troubleshooting](#troubleshooting)
9. [Future Enhancements](#future-enhancements)
10. [Contributing](#contributing)
11. [License](#license)

---

## Overview

The **WeatherForecast** project is a microservices-based application showing a complete end-to-end development and deployment workflow on Azure. It demonstrates best practices for containerization, API management, authentication, secret management, and automated CI/CD using GitHub Actions.

---

## Architecture

### The application's architecture includes:

- **API Gateway:** Azure API Management provides a unified, secure entry point for the Web API.  
  [weatherforecastapims.azure-api.net](https://weatherforecastapims.azure-api.net)
- **Web API:** .NET 8 Web API deployed to Azure Kubernetes Service (AKS) for scalable weather data and logic.  
  AKS Public IP: `weatherforecast.canadacentral.cloudapp.azure.com/swagger`
- **Azure Function:** HTTP-triggered Azure Function for POST calls, writing into Azure Cosmos DB.
- **Databases:**
    - **Azure SQL Database:** Relational data storage.
    - **Azure Cosmos DB:** NoSQL storage (event logs, historical data).
- **Container Registry:** Azure Container Registry (ACR) stores Docker images.
- **Authentication & Authorization:** Azure Active Directory (Azure AD) for OAuth 2.0 / OIDC authentication.
- **Secret Management:** Azure Key Vault stores sensitive info (e.g., connection strings).
- **CI/CD:** GitHub Actions automates build, test, and deployment.

---

## Components

### .NET Web API

- **Location:** `WeatherForecast` directory.
- **Purpose:** Core weather forecasting API endpoints.
- **Tech:** .NET 8, ASP.NET Core Web API.
- **Deployment:** Docker container, deployed to AKS.
- **Exposure:** Externally via Azure API Management (`https://weatherforecastapims.azure-api.net/locations`).
- **Authentication:** OAuth 2.0/OIDC (Azure AD). Requires scopes (e.g., `location.locations`).
- **Database:** Azure SQL Database. Connection string from Key Vault.
- **Dockerfile:** `WeatherForecast/Dockerfile`.

### Azure Function

- **Location:** `WeatherForecast.Function` directory.
- **Purpose:** HTTP-triggered function, writes data to Cosmos DB.
- **Tech:** .NET, Azure Functions runtime.
- **Deployment:** Docker container, deployed to Azure Function App.
- **Database:** Writes to Azure Cosmos DB.
- **Dockerfile:** `WeatherForecast.Function/Dockerfile`.

### SQL Database Project

- **Location:** `WeatherForecast_DB` directory (implied).
- **Purpose:** Database schema, migration scripts for Azure SQL Database.
- **Deployment:** Via CI/CD pipeline.
- **Connection:** From Key Vault.

### Console App (OAuth Tester)

- **Purpose:** Utility to test OAuth 2.0/OIDC authentication workflow for the Web API.

### Unit Tests

- **Location:** `WeatherForecast.WebApi.UnitTests`, `WeatherForecast.Domain.UnitTests`.
- **Purpose:** Unit tests for Web API and domain logic.
- **Execution:** Part of CI/CD pipeline.

---

## Azure Resources

The project uses:

- **Azure Kubernetes Service (AKS):** Hosts the .NET Web API.
- **Azure API Management (APIM):** API Gateway for external exposure and security.
- **Azure Functions App:** Hosts the containerized Azure Function.
- **Azure SQL Database:** Relational DB for the Web API.
- **Azure Cosmos DB:** NoSQL DB for the Azure Function.
- **Azure Container Registry (ACR):** Stores Docker images.
- **Azure Key Vault:** Stores secrets.
- **Azure Active Directory (Azure AD):** Identity and access management.

---

## Authentication & Authorization

Secured with **OAuth 2.0 / OpenID Connect (OIDC)** via Azure AD.

- **Azure AD App Registrations:**
    - One for the **Web API**: Exposes custom scopes (e.g., `location.locations`).
    - One for **Client Applications** (e.g., Console App): Requests permissions to the Web API's scopes.
- **Authorization Policy (`location.locations`):** Defined in Web API's `Program.cs`. Requires tokens with the scope.
- **Service Principals:** Used by CI/CD (e.g., `github-acr-sp`), with permissions for Azure resources.
- **Azure AD Managed Identity:** Used by AKS pods to access Azure resources (e.g., Key Vault) securely.

---

## CI/CD Pipeline

Uses **GitHub Actions** (`.github/workflows/main.yaml`):

On every commit to `main` branch:

1. **Build .NET Projects:** Web API, Azure Function, unit tests.
2. **Run Unit Tests.**
3. **Build Docker Images:** For Web API and Azure Function.
4. **Push Images to ACR.**
5. **SQL Database Deployment:** Schema deployed to Azure SQL DB with connection string from Key Vault.
6. **Azure Function Deployment:** Deploy the container image to Function App.
7. **AKS Deployment:** Deploy Web API container to AKS.
    - Uses Service Principal with necessary roles.
8. **Secret Management:** Key Vault integration for sensitive config.

---

## Getting Started

### Prerequisites

- VS Code & .NET extensions
- Azure Subscription
- Azure CLI
- Docker Desktop
- .NET 8 SDK
- kubectl
- GitHub Account

### Local Development

1. **Clone the repo:**
    ```bash
    git clone https://github.com/<your-github-username>/WeatherForecast.git
    cd WeatherForecast
    ```
2. **Restore dependencies:**
    ```bash
    dotnet restore
    ```
3. **Run Web API locally:**
    ```bash
    cd WeatherForecast/WeatherForecast
    dotnet run
    ```
    API available at `https://localhost:<port>`

4. **Test OAuth Workflow:**
    - Configure `AuthenticationTester` with Azure AD client details.
    - Run the console app to obtain/test access tokens.

### Deployment (CI/CD)

Handled by GitHub Actions pipeline.

1. **Configure Azure Resources:** AKS, APIM, ACR, Function App, SQL DB, Cosmos DB, Key Vault, Azure AD App Registrations.
2. **Set GitHub Secrets:** For Azure Service Principal credentials (e.g., `AZURE_CREDENTIALS`).
3. **Push to `main` branch:** Triggers pipeline.

---

## Troubleshooting

- **`500 Internal Server Error` on AKS Web API**
    - **CrashLoopBackOff:** App failed to start in container.
        - **Diagnosis:** Check pod logs:  
          `kubectl logs <pod-name> -n <namespace>` or Azure Portal.
        - **Common causes:** Missing env vars (e.g., DB connection), wrong paths, unhandled exceptions.
    - **Missing .NET Runtime:**  
      Ensure Dockerfile uses correct .NET 8.0 base image.

- **`localhost:8080 connection refused` in CI/CD**
    - **Cause:** `kubectl` not connecting to AKS cluster.
    - **Solution:** Use manual Kubeconfig construction in GitHub Actions.

- **`404 Not Found` via APIM**
    - **Cause:** APIM forwarding with incorrect URL.
    - **Diagnosis:** Check APIM Trace logs.
    - **Solution:** Adjust APIM "Service URL" or add rewrite-uri policy.

- **`401 Access Denied due to invalid subscription key` from APIM**
    - **Cause:** Request missing/incorrect APIM subscription key.
    - **Solution:** Ensure you use an active key in your request.

- **`System.InvalidOperationException: The AuthorizationPolicy named: 'location.locations' was not found.`**
    - **Cause:** Policy not defined in API or scope not registered in Azure AD.
    - **Solution:**  
      1. Define policy in `Program.cs`.  
      2. Register scope in Azure AD.  
      3. Grant permission to client app.

- **Database Connectivity Issue**
    - **Symptom:** Web API/Azure Function can't connect to SQL/Cosmos DB.
    - **Cause:** Outbound IPs not allowed in DB firewall, or DB is behind Private Endpoint.
    - **Solution (Temporary):**
        1. **Identify outbound IPs** for AKS/Function App in Azure Portal.
        2. **Add to firewall** in SQL/Cosmos DB.
    - **Solution (Recommended):**
        1. Deploy AKS in Virtual Network.
        2. Enable VNet integration for Function App.
        3. Use Private Endpoints for SQL/Cosmos DB.

---

## Future Enhancements

Ideas for extending the project:

- **Azure OpenAI:** Integrate for intelligent weather insights, NLP, or AI-driven features.
- **Azure Front Door:** Add for global load balancing and advanced routing.
- **AKS Ingress Controllers:** Try advanced controllers (e.g., NGINX, AGIC) for better traffic management.
- **Microsoft Graph API:** Integrate to access Microsoft 365 data.
- **Monitoring & Alerting:** Add Azure Monitor dashboards, alerts, and Application Insights for deeper observability.

---

## Contributing

Contributions are welcome! Please fork the repo, create a branch, make your changes, and submit a pull request.

---

## License

[MIT License](LICENSE)

---