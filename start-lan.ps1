# DnD Companion — LAN startup script
# Run: .\start-lan.ps1

$Port = 5098
$Project = "$PSScriptRoot\DndCompanion.Web\DndCompanion.Web.csproj"

# Resolve current Wi-Fi IPv4
$IP = (
    Get-NetIPAddress -AddressFamily IPv4 |
    Where-Object { $_.IPAddress -notmatch '^(127\.|169\.254\.)' } |
    Sort-Object InterfaceMetric |
    Select-Object -First 1
).IPAddress

# Ensure firewall rule exists
$ruleName = "DndCompanion LAN $Port"
$existing = netsh advfirewall firewall show rule name="$ruleName" 2>&1
if ($existing -notmatch 'OK\.') {
    Write-Host "Adding firewall rule for port $Port..." -ForegroundColor Yellow
    netsh advfirewall firewall add rule name="$ruleName" dir=in action=allow protocol=TCP localport=$Port | Out-Null
    Write-Host "Firewall rule added." -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  DnD Companion starting on LAN" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Local:    localhost:$Port" -ForegroundColor White
Write-Host "  Network:  ${IP}:$Port" -ForegroundColor Green
Write-Host ""
Write-Host "  Share 'Network' URL with your players." -ForegroundColor Yellow
Write-Host "  To exit - press CTRL+C" -ForegroundColor Yellow
Write-Host ""

dotnet run --project $Project --urls "http://0.0.0.0:$Port"

