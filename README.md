# Recipes
local.settings.json file on Recipes.Api:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CosmosDBConnection": "{Connection String}",
    "DatabaseName": "{Database Name}",
    "ContainerName": "{Container Name}"
  },
  "Host": {
    "CORS": "*"
  }
}
```
Another local.settings.json required on Test project but only needs Values:CosmosDBConnection, Values:DatabaseName and Values:ContainerName.