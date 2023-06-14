# ASP.net Core Demo Application

## Setup

### Install dependencies
To install the application dependencies, run the following command:
```
cd todo-dotnet-v2
dotnet restore
```

### Topaz
#### Update the `appsettings.Development.json` file
Update the content of the `appsettings.Development.json` file with the following:
```json
{
  "AllowedHosts": "*",
  "Aserto": {
    "Insecure": true,
    "PolicyRoot": "todoApp",
    "ServiceUrl": "https://localhost:8282"
  },
  "Urls": "http://localhost:3001",
  "Directory": {
    "Insecure": true,
    "ServiceUrl": "https://localhost:9292"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "OAuth": {
    "Audience": "citadel-app",
    "Domain": "citadel.demo.aserto.com/dex"
  }
}
```
### Aserto hosted authorizer
#### Update the `appsettings.Development.json` file
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Urls": "http://localhost:3001",
  "OAuth": {
    "Domain": "citadel.demo.aserto.com/dex",
    "Audience": "citadel-app"
  },
  "Directory": {
    "APIKey": "{Your Directory (read-only) API Key}",
    "Insecure": false,
    "ServiceUrl": "https://directory.prod.aserto.com:8443",
    "TenantID": "{Your Aserto Tenant ID UUID}"
  },
  "Aserto": {
    "AuthorizerApiKey": "{Your Authorizer API Key}",
    "TenantID": "{Your Aserto Tenant ID UUID}",
    "PolicyName": "todo",
    "PolicyInstanceLabel": "todo",
    "PolicyRoot": "todoApp",
    "ServiceUrl": "https://authorizer.prod.aserto.com:8443"
  },
  "AllowedHosts": "*"
}
```

## Start the server
```
dotnet run
```
