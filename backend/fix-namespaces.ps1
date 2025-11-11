# Fix namespace issues in all C# files

$filesToFix = @(
    "src/Menufy.Application/Features/Users/Queries/GetUsers/GetUsersQuery.cs",
    "src/Menufy.Application/Features/Users/Queries/GetUsers/GetUsersQueryHandler.cs",
    "src/Menufy.Application/Features/Users/Commands/UpdateUser/UpdateUserCommand.cs",
    "src/Menufy.Application/Features/Users/Commands/UpdateUser/UpdateUserCommandHandler.cs",
    "src/Menufy.Application/Features/Admin/Queries/GetDashboardAnalytics/GetDashboardAnalyticsQuery.cs",
    "src/Menufy.Application/Features/Admin/Queries/GetDashboardAnalytics/GetDashboardAnalyticsQueryHandler.cs",
    "src/Menufy.Application/Features/Files/Commands/UploadImage/UploadImageCommand.cs",
    "src/Menufy.Application/Features/Files/Commands/UploadImage/UploadImageCommandHandler.cs",
    "src/Menufy.Application/Features/Restaurants/Queries/GetRestaurants/GetRestaurantsQuery.cs",
    "src/Menufy.Application/Features/Restaurants/Queries/GetRestaurants/GetRestaurantsQueryHandler.cs",
    "src/Menufy.Application/Common/Mappings/MappingProfile.cs"
)

foreach ($file in $filesToFix) {
    $fullPath = Join-Path $PSScriptRoot $file
    if (Test-Path $fullPath) {
        $content = Get-Content $fullPath -Raw
        $content = $content -replace 'using Menufy\.Application\.Common;', 'using Menufy.Application.Common.Models;'
        $content = $content -replace 'using Menufy\.Application\.Features\.Auth\.Common;', 'using Menufy.Application.Features.Auth.DTOs;'
        Set-Content -Path $fullPath -Value $content -NoNewline
        Write-Host "Fixed: $file" -ForegroundColor Green
    } else {
        Write-Host "Not found: $file" -ForegroundColor Yellow
    }
}

Write-Host "`nAll files fixed!" -ForegroundColor Cyan
