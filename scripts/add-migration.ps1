<#
.SYNOPSIS
  Creates an EF Core migration (code first) for one of the two supported databases.
.EXAMPLE
  ./scripts/add-migration.ps1 -Provider SqlServer -Name Initial
  ./scripts/add-migration.ps1 -Provider PostgreSql -Name Initial
#>
param(
  [Parameter(Mandatory)][ValidateSet('SqlServer', 'PostgreSql')][string]$Provider,
  [Parameter(Mandatory)][string]$Name
)

$root = Split-Path -Parent $PSScriptRoot
$env:Database__Provider = $Provider
if (-not $env:ASPNETCORE_ENVIRONMENT) { $env:ASPNETCORE_ENVIRONMENT = 'Development' }

if ($Provider -eq 'PostgreSql' -and -not $env:Database__ConnectionString) {
  # Add-Migration does not connect to the database; it only needs a valid connection string for the provider.
  $env:Database__ConnectionString = 'Host=localhost;Database=restaurantsaas_design;Username=postgres;Password=postgres'
}

dotnet ef migrations add $Name `
  --project "$root/src/RestaurantSaaS.Migrations.$Provider" `
  --startup-project "$root/src/RestaurantSaaS.Web" `
  --output-dir Migrations
