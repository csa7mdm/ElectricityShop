Write-Host "Checking for outdated packages..."
Write-Host "FluentValidation.AspNetCore:"
Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/fluentvalidation.aspnetcore/index.json" | ConvertTo-Json
Write-Host "Swashbuckle.AspNetCore:"
Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/swashbuckle.aspnetcore/index.json" | ConvertTo-Json
Write-Host "Moq:"
Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/moq/index.json" | ConvertTo-Json
Write-Host "Microsoft.AspNetCore.Authentication.JwtBearer:"
Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/microsoft.aspnetcore.authentication.jwtbearer/index.json" | ConvertTo-Json
