# Namespace Fixes Required

## Issue
Many new files are using:
- `using Menufy.Application.Common;` instead of `using Menufy.Application.Common.Models;`
- `using Menufy.Application.Features.Auth.Common;` instead of `using Menufy.Application.Features.Auth.DTOs;`

## Files to Fix

### Auth Commands
1. RefreshTokenCommand.cs - Change Auth.Common to Auth.DTOs
2. RefreshTokenCommandHandler.cs - Change Auth.Common to Auth.DTOs, Add Common.Models

### User Commands & Queries
3. GetUsersQuery.cs - Change Auth.Common to Auth.DTOs
4. GetUsersQueryHandler.cs - Change Auth.Common to Auth.DTOs, Add Common.Models

### All New Command/Query Files
Need to add: `using Menufy.Application.Common.Models;` for Result/Result<T>

Files:
- ChangePasswordCommand.cs
- ChangePasswordCommandHandler.cs
- DeleteCategoryCommand.cs
- DeleteCategoryCommandHandler.cs
- DeleteMenuItemCommand.cs
- DeleteMenuItemCommandHandler.cs
- ForgotPasswordCommand.cs
- ForgotPasswordCommandHandler.cs
- GetDashboardAnalyticsQuery.cs
- GetDashboardAnalyticsQueryHandler.cs
- ResetPasswordCommand.cs
- ResetPasswordCommandHandler.cs
- UpdateCategoryCommand.cs
- UpdateCategoryCommandHandler.cs
- UpdateMenuItemCommand.cs
- UpdateMenuItemCommandHandler.cs
- UpdateRestaurantCommand.cs (need to completely rewrite)
- UpdateRestaurantCommandHandler.cs (need to rewrite)
- UpdateUserCommand.cs
- UpdateUserCommandHandler.cs
- UploadImageCommand.cs
- UploadImageCommandHandler.cs

### Mapping Profile
- MappingProfile.cs - Change Auth.Common to Auth.DTOs

## Quick Fix Command (PowerShell)

```powershell
# Fix Common namespace
Get-ChildItem -Path "C:\Users\pc1\Documents\menufy\backend\src\Menufy.Application\Features" -Recurse -Filter "*.cs" |
ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'using Menufy\.Application\.Common;', 'using Menufy.Application.Common.Models;'
    $content = $content -replace 'using Menufy\.Application\.Features\.Auth\.Common;', 'using Menufy.Application.Features.Auth.DTOs;'
    Set-Content -Path $_.FullName -Value $content
}

# Also fix MappingProfile
$file = "C:\Users\pc1\Documents\menufy\backend\src\Menufy.Application\Common\Mappings\MappingProfile.cs"
$content = Get-Content $file -Raw
$content = $content -replace 'using Menufy\.Application\.Features\.Auth\.Common;', 'using Menufy.Application.Features.Auth.DTOs;'
Set-Content -Path $file -Value $content
```
