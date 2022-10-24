# ASP.net Core Demo Application

## Setup

### Install dependencies
To install the application dependencies, run the following command:
```
cd todo-dotnet-v2
dotnet restore
```

### Update the `appsettings.Development.json` file
Update the content of the `appsettings.Development.json` file with the following:
```json
{
  "AllowedHosts": "*",
  "Aserto": {
    "PolicyRoot": "todoApp",
    "ServiceUrl": "https://localhost:8282"
  },
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

## Start the server
```
dotnet run
```
