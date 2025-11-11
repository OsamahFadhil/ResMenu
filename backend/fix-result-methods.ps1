# PowerShell script to fix Result method calls

Get-ChildItem -Recurse -Filter "*.cs" -Path "src" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $modified = $false

    # Fix Result.Success() -> Result.SuccessResult()
    if ($content -match 'Result\.Success\(') {
        $content = $content -replace 'Result\.Success\(', 'Result.SuccessResult('
        $modified = $true
    }

    # Fix Result.Failure( -> Result.FailureResult(
    if ($content -match 'Result\.Failure\(') {
        $content = $content -replace 'Result\.Failure\(', 'Result.FailureResult('
        $modified = $true
    }

    # Fix Result<T>.Success( -> Result<T>.SuccessResult(
    if ($content -match 'Result<([^>]+)>\.Success\(') {
        $content = $content -replace 'Result<([^>]+)>\.Success\(', 'Result<$1>.SuccessResult('
        $modified = $true
    }

    # Fix Result<T>.Failure( -> Result<T>.FailureResult(
    if ($content -match 'Result<([^>]+)>\.Failure\(') {
        $content = $content -replace 'Result<([^>]+)>\.Failure\(', 'Result<$1>.FailureResult('
        $modified = $true
    }

    if ($modified) {
        Set-Content -Path $_.FullName -Value $content -NoNewline
        Write-Host "Fixed: $($_.FullName)"
    }
}

Write-Host "Done!"
