# Set image prefix
$prefix = "what2play"

function Build-Image {
    param (
        [string]$dir,
        [string]$component
    )
    $tag = "$prefix-$component"
    Write-Host "Building $tag"
    Push-Location $dir
    docker build -t $tag .
    Pop-Location
}

function Build-Migrations-Image {
    param (
        [string]$dir
    )
    $tag = "$prefix-migrations"
    Write-Host "Building $tag"
    Push-Location $dir
    docker build -t $tag -f "Dockerfile.Migrations" .
    Pop-Location
}

Build-Image -dir "backend" -component "backend"
Build-Image -dir "frontend" -component "frontend"
Build-Migrations-Image -dir "backend"

Write-Host "Done"