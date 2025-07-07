# WeatherForecast Project - Personal Learning & Experimentation

**_Disclaimer:_** This project is developed purely for personal learning, experimentation, and demonstrating cloud-native concepts on Azure. It is not recommended for production usage reference or deployment.

This repository contains a comprehensive cloud-native application designed to provide weather forecasting functionalities. It leverages Azure Kubernetes Service (AKS) for scalable API deployment, Azure Functions for event-driven processing, and integrates with various Azure resources for a robust, secure, and modern cloud architecture.

## Table of Contents

1.  [Overview](#1-overview)
2.  [Architecture](#2-architecture)
3.  [Components](#3-components)
    * [.NET Web API](#net-web-api)
    * [Azure Function](#azure-function)
    * [SQL Database Project](#sql-database-project)
    * [Console App (OAuth Tester)](#console-app-oauth-tester)
    * [Unit Tests](#unit-tests)
4.  [Azure Resources](#4-azure-resources)
5.  [Authentication & Authorization](#5-authentication--authorization)
6.  [CI/CD Pipeline](#6-cicd-pipeline)
7.  [Getting Started](#7-getting-started)
    * [Prerequisites](#prerequisites)
    * [Local Development](#local-development)
    * [Deployment (CI/CD)](#deployment-cicd)
8.  [Troubleshooting](#8-troubleshooting)
9.  [Future Enhancements](#9-future-enhancements)
10. [Contributing](#10-contributing)
11. [License](#11-license)

---

## 1. Overview

The WeatherForecast project is a microservices-based application demonstrating a complete end-to-end development and deployment workflow on Azure. It showcases best practices for containerization, API management, authentication & authorization, CI/CD, secret management, and more.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 2. Architecture

The application's architecture includes:

- **API Gateway:** Azure API Management provides a unified, secure entry point for the Web API.  
  https://weatherforecastapims.azure-api.net
- **Web API:** A .NET 8 Web API deployed to Azure Kubernetes Service (AKS) for scalable exposure of weather data and business logic.  
  AKS Public IP - weatherforecast.canadacentral.cloudapp.azure.com/swagger
- **Azure Function:** An HTTP Azure Function triggered for a POST call, writing into Azure Cosmos DB.
- **Databases:**
    - **Azure SQL Database:** For relational data storage.
    - **Azure Cosmos DB:** For NoSQL data storage (e.g., event logs, historical data from Azure Function).
- **Container Registry:** Azure Container Registry (ACR) stores Docker images for the Web API and Azure Function.
- **Authentication & Authorization:** Leverages Azure Active Directory (Azure AD) for OAuth 2.0 / OpenID Connect (OIDC) authentication, ensuring secure access to the Web API.
- **Secret Management:** Azure Key Vault securely stores sensitive information like database connection strings.
- **CI/CD:** GitHub Actions workflows automate the build, test, and deployment processes.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 3. Components

### .NET Web API

- **Location:** `WeatherForecast` directory.
- **Purpose:** Provides core weather forecasting API endpoints.
- **Technology:** .NET 8, ASP.NET Core Web API.
- **Deployment:** Containerized using Docker and deployed to Azure Kubernetes Service (AKS).
- **Exposure:** Exposed externally via Azure API Management with a clean URL (e.g., `https://weatherforecastapims.azure-api.net/locations`).
- **Authentication:** Secured with OAuth 2.0 / OIDC using Azure AD. Requires specific scopes (e.g., `location.locations`) for access.
- **Database Integration:** Connects to Azure SQL Database for data storage, with connection strings securely retrieved from Azure Key Vault during deployment.
- **Dockerfile:** Located at `WeatherForecast/Dockerfile` for building the Web API image.

### Azure Function

- **Location:** `WeatherForecast.Function` directory.
- **Purpose:** An HTTP-triggered Azure Function to process incoming requests and write data to Azure Cosmos DB.
- **Technology:** .NET, Azure Functions runtime.
- **Deployment:** Containerized and deployed as a Docker image to an Azure Function App.
- **Database Integration:** Writes data to Azure Cosmos DB.
- **Dockerfile:** Located at `WeatherForecast.Function/Dockerfile` for building the Azure Function image.

### SQL Database Project

- **Location:** `WeatherForecast_DB` directory (implied).
- **Purpose:** Contains the database schema and migration scripts for the Azure SQL Database.
- **Deployment:** Deployed to Azure SQL Database via the CI/CD pipeline.
- **Connection String:** Retrieved securely from Azure Key Vault during deployment.

### Console App (OAuth Tester)

- **Purpose:** A utility console application designed to test the OAuth 2.0 / OIDC authentication workflow for the Web API. This helps in debugging token acquisition and validation.

### Unit Tests

- **Location:** `WeatherForecast.WebApi.UnitTests` and `WeatherForecast.Domain.UnitTests` directories.
- **Purpose:** Contains unit tests for the Web API and Domain logic to ensure code quality and correctness.
- **Execution:** Run as part of the CI/CD pipeline.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 4. Azure Resources

The project deploys and interacts with the following Azure services:

- **Azure Kubernetes Service (AKS):** Hosts the .NET Web API.
- **Azure API Management (APIM):** Acts as the API Gateway for the Web API, providing external exposure, security, and policy management.
- **Azure Functions App:** Hosts the containerized Azure Function.
- **Azure SQL Database:** Relational database for the Web API.
- **Azure Cosmos DB:** NoSQL database for the Azure Function.
- **Azure Container Registry (ACR):** Stores Docker images built by the CI pipeline.
- **Azure Key Vault:** Securely stores secrets like database connection strings and other sensitive configuration.
- **Azure Active Directory (Azure AD):** Provides identity and access management for authentication and authorization.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 5. Authentication & Authorization

The Web API is secured using **OAuth 2.0 / OpenID Connect (OIDC)** with Azure AD.

- **Azure AD App Registrations:**
    - One for the **Web API itself**: Defines and exposes custom scopes (e.g., `location.locations`). This is the "resource" or "audience" for access tokens.
    - One for **Client Applications** (e.g., the Console App, Postman): These applications request permissions to the Web API's exposed scopes.
- **Authorization Policy (`location.locations`):** In the Web API's `Program.cs`, an authorization policy ensures that only authenticated users presenting an access token with the `location.locations` scope can access protected endpoints.
- **Azure AD Managed Identity & Service Principals:**
    - **Service Principals:** Used by the CI/CD pipeline (e.g., `github-acr-sp` with `AcrPush` and `Azure Kubernetes Service Cluster Admin Role`) for secure, automated access to Azure resources like ACR and AKS.
    - **Azure AD Managed Identity (for AKS pods):** Used by AKS pods to securely access Azure resources like Azure Key Vault, eliminating the need to manage credentials in code. This is configured via Azure AD Workload Identity or Managed Identity.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 6. CI/CD Pipeline

The project employs a robust CI/CD pipeline using **GitHub Actions**, defined in `.github/workflows/main.yaml`.

The pipeline automates the following upon every commit to the `main` branch:

1.  **Build .NET Projects:** Compiles the Web API, Azure Function, and unit test projects.
2.  **Run Unit Tests:** Executes all unit tests.
3.  **Docker Image Build:** Builds Docker images for the Web API and Azure Function.
4.  **Image Push to ACR:** Pushes the built Docker images to Azure Container Registry (ACR).
5.  **SQL Database Deployment:** Deploys the SQL project to Azure SQL Database. Connection strings for the database are securely fetched from Azure Key Vault.
6.  **Azure Function Deployment:** Deploys the containerized Azure Function image to an Azure Function App.
7.  **AKS Deployment:** Deploys the Web API container image to the AKS cluster.
    - The pipeline configures `kubectl` with Azure AD service principal credentials to connect to AKS, even with disabled local accounts.
    - It uses a Service Principal with `Azure Kubernetes Service Cluster Admin Role` for sufficient permissions.
8.  **Secret Management Integration:** Azure Key Vault is integrated into the pipeline to replace sensitive configuration like database connection strings at deployment time.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 7. Getting Started

### Prerequisites

- VS Code Installed with necessary extensions for .NET development
- Azure Subscription (Free first 12 months)
- Azure CLI installed and configured
- Docker Desktop installed
- .NET 8 SDK installed
- kubectl installed
- GitHub Account

### Local Development

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/<your-github-username>/WeatherForecast.git
    cd WeatherForecast
    ```
2.  **Restore .NET dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Run the Web API locally:**
    ```bash
    cd WeatherForecast/WeatherForecast # Navigate to the Web API project directory
    dotnet run
    ```
    The API will typically be available at `https://localhost:<port>`.

4.  **Test OAuth Workflow with Console App:**
    - Configure the `AuthenticationTester` project with your Azure AD client application details.
    - Run the console app to obtain and test access tokens.
    - Also run Web API in parallel to test the token.

### Deployment (CI/CD)

The deployment is handled automatically by the GitHub Actions pipeline.

1.  **Configure Azure Resources:** Ensure all necessary Azure resources (AKS, APIM, ACR, Azure Function App, SQL DB, Cosmos DB, Key Vault, Azure AD App Registrations) are provisioned and correctly configured.
2.  **GitHub Actions Secrets:** Set up the required GitHub Secrets in your repository for the Azure Service Principal credentials (e.g., `AZURE_CREDENTIALS`).
3.  **Push to `main` branch:** Any push to the `main` branch will trigger the CI/CD pipeline.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 8. Troubleshooting

- **`500 Internal Server Error` on AKS Web API:**
    - **Application Crash (`CrashLoopBackOff`):** This often means your .NET application failed to start inside the container.
        - **Diagnosis:** Check AKS pod logs using `kubectl logs <pod-name> -n <namespace>` or via Azure Portal's Container Insights (`AKS Cluster -> Monitoring -> Logs -> ContainerLogV2`). Look for unhandled exceptions or configuration issues.
        - **Common Causes:** Missing environment variables (e.g., database connection strings), incorrect paths, or unhandled exceptions in `Program.cs` or `Startup.cs`.
    - **Missing .NET Runtime:** The application requires a specific .NET runtime version (e.g., .NET 8.0), but it's not present in the container image or on the host running the app.
        - **Error Message Example:** `"You must install or update .NET to run this application. ... Framework: 'Microsoft.AspNetCore.App', version '8.0.0' ... The following frameworks were found: 6.0.[...]`
        - **Solution:** Ensure your Dockerfile's `FROM` statement specifies the correct .NET 8.0 SDK/Runtime base image for your architecture (e.g., `mcr.microsoft.com/dotnet/aspnet:8.0-alpine-arm64v8`).

- **`localhost:8080 connection refused` in CI/CD (during `kubectl` commands):**
    - **Error Message Example:** `"Unhandled Error" err="couldn't get current server API group list: Get \"http://localhost:8080/api?timeout=32s\": dial tcp [::1]:8080: connect: connection refused"`
    - **Cause:** The `kubectl` command on the GitHub Actions runner is failing to correctly configure its context to connect to your remote AKS cluster, often defaulting to a local address.
    - **Solution:** Implement the "Full Manual Kubeconfig Construction" method in your GitHub Actions workflow. This explicitly fetches the AKS cluster's FQDN and CA certificate data, then manually constructs the kubeconfig.

- **`404 Not Found` when accessing via Azure API Management (APIM):**
    - **Cause:** APIM is forwarding the request to your backend with an incorrect URL path.
    - **Diagnosis:** Check the APIM Trace logs in the "Test" tab.
    - **Solution 1 (Backend Service URL Adjustment):** If your backend API has a common base path (e.g., `/api/location`), set the APIM API's "Service URL" (in the "Settings" tab) to `http://weatherforecast.canadacentral.cloudapp.azure.com`.
    - **Solution 2 (Rewrite URL Policy for Clean Gateway URL):**
        1.  Set the API's "Service URL" in APIM Settings to just the base: `http://weatherforecast.canadacentral.cloudapp.azure.com`.
        2.  Set the API Operation's "URL template" in the "Design" tab to the clean path (e.g., `/locations`).
        3.  Add a `rewrite-uri` policy in the "Inbound processing" of the operation's policies to transform the path (e.g., `<rewrite-uri template="/api/location/locations" />`).

- **`401 Access Denied due to invalid subscription key` from APIM:**
    - **Cause:** The request to APIM is missing a valid subscription key or the provided key is incorrect/inactive.
    - **Solution:**
        1.  Ensure you have an active subscription key for a product associated with your API. Find or create a key under **APIM -> Products -> [Your Product] -> Subscriptions**.
        2.  Provide the correct key in your request, either as a query parameter (`?subscription-key=YOUR_KEY`) or, preferably, as an HTTP header (`Ocp-Apim-Subscription-Key: YOUR_KEY`).

- **`System.InvalidOperationException: The AuthorizationPolicy named: 'location.locations' was not found.`**
    - **Cause:** Your .NET Web API is trying to enforce an authorization policy (`location.locations`), but it hasn't been correctly defined in your application's startup (`Program.cs` or `Startup.cs`).
    - **Solution:**
        1.  **Define the policy in `Program.cs`:** Ensure you have `builder.Services.AddAuthorization(options => { options.AddPolicy("location.locations", ...); });` with the correct requirements (e.g., scope claim).
        2.  **Define the Scope in Azure AD (Web API App Registration):** In the Azure Portal, go to the **App registration for your Web API** -> **"Expose an API"** and add a new scope named `location.locations`.
        3.  **Grant Permission to Client App (Client App Registration):** In the Azure Portal, go to the **App registration for your Client Application** -> **"API permissions"**, add a permission to your API for the `location.locations` scope, and grant admin consent.

- **Database Connectivity Issue (e.g., timeout, connection refused) due to Restricted Public Access:**
    - **Symptom:** Your Web API or Azure Function cannot connect to Azure SQL Database or Azure Cosmos DB. This often happens when database public network access is set to "Selected networks" or completely disabled.
    - **Cause:** The outbound IP addresses of your AKS cluster or Azure Function App are not explicitly allowed in the database firewall rules, or the database is behind a Private Endpoint and your compute resources can't reach it.
    - **Solution (Temporary/IP-based):**
        1.  **Identify Outbound IPs:**
            - **For AKS:** Find the cluster's egress IP address (or Load Balancer/NAT Gateway public IP) in the AKS cluster's "Properties" or "Networking" settings in the Azure Portal.
            - **For Azure Function App:** Find the "Outbound IP addresses" in the Function App's "Properties" in the Azure Portal.
        2.  **Add to Database Firewall:**
            - **Azure SQL Database:** Go to your SQL Server's **"Networking"** blade, then **"Firewall rules"**, and add the identified IPs. Ensure "Allow Azure services and resources to access this server" is enabled (if needed).
            - **Azure Cosmos DB:** Go to your Cosmos DB account's **"Networking"** blade, select **"Selected networks"** for "Public network access", and add the identified IPs under "Firewall".
    - **Solution (Recommended Secure Approach - Private Endpoints):**
        1.  Ensure your AKS cluster is deployed into an Azure Virtual Network.
        2.  Ensure your Azure Function App has VNet integration enabled.
        3.  Configure Private Endpoints for your Azure SQL Database and Azure Cosmos DB within the same (or peered) Virtual Network. This removes reliance on public IPs entirely.

[⬆ Back to Table of Contents](#table-of-contents)

---

## 9. Future Enhancements

Here are some ideas for extending and enhancing this project:

- **Experiment with Azure OpenAI:** Explore integrating Azure OpenAI services to add intelligent capabilities, such as natural language processing for weather insights, predictive analysis, or conversational interfaces.
- **Play with Azure Front Door:** Implement Azure Front Door in front of Azure API Management to provide global load balancing, application acceleration (Anycast), advanced routing rules, and Web Application Firewall (WAF) protection.
- **Explore AKS Ingress Controllers:** Investigate and implement advanced AKS Ingress controllers (e.g., NGINX Ingress Controller, Azure Application Gateway Ingress Controller) for more sophisticated routing, TLS termination, and security.
- **Integrate Microsoft Graph API:** Add functionality to interact with Microsoft Graph, allowing the application to access data from Microsoft 365 services (e.g., calendar events, user profiles) to enhance weather-based recommendations or notifications.
- **Monitoring & Alerting:** Integrate comprehensive Azure Monitor dashboards, alerts, and Application Insights for detailed application performance monitoring (APM) and operational insights.

[⬆ Back to Table of Contents](#table-of-contents)

---