# demo-blazority

Demo project to evaluate Blazority

## Build

```pwsh
dotnet new globaljson
dotnet new tool-manifest
dotnet tool install --local dotnet-ef
```

## Database migration

```pwsh
dotnet ef migrations add CreateInitSchema `
    --verbose `
    --context ApplicationDbContext `
    --output-dir ./Data/Migrations/Application
```

```bash
dotnet ef migrations add CreateInitSchema \
  --verbose \
  --context ApplicationDbContext \
  --output-dir ./Data/Migrations/Application
```

## Client Development

```pwsh
dotnet tool install --local StrawberryShake.Tools
```

```pwsh
dotnet graphql init https://localhost:5001/graphql/ `
  -n BlazorityClient `
  -p ./src/Demo.Blazor.Clarity/Client/Infrastructure/GraphQL
```

```pwsh
dotnet graphql download https://localhost:5001/graphql/ `
  -f ./src/Demo.Blazor.Clarity/Client/Infrastructure/GraphQL/schema.graphql
```
