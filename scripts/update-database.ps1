<#
.SYNOPSIS
  Applies the migrations to the configured database (the app also does this at start-up in Development).
#>
param(
  [Parameter(Mandatory)][ValidateSet('SqlServer', 'PostgreSql')][string]$Provider
)

$root = Split-Path -Parent $PSScriptRoot
$env:Database__Provider = $Provider
if (-not $env:ASPNETCORE_ENVIRONMENT) { $env:ASPNETCORE_ENVIRONMENT = 'Development' }

dotnet ef database update `
  --project "$root/src/RestaurantSaaS.Migrations.$Provider" `
  --startup-project "$root/src/RestaurantSaaS.Web"
