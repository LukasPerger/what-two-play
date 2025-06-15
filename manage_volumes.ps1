# Set volume prefix
$prefix = "what2play"

$dbVolume = "$prefix-db_data"

function Show-Usage {
    Write-Host "Usage: .\manage-volumes.ps1 [create|remove]"
    exit 1
}

if ($args.Count -ne 1) {
    Show-Usage
}

switch ($args[0]) {
    "create" {
        Write-Host "Creating volume..."
        docker volume create $dbVolume | Out-Null
        Write-Host "Volume created."
    }
    "remove" {
        Write-Host "Removing volume..."
        docker volume rm $dbVolume | Out-Null
        Write-Host "Volume removed."
    }
    default {
        Show-Usage
    }
}
