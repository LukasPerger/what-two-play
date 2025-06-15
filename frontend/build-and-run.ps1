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

Build-Image -dir "" -component "frontend"
docker run -d --rm -p 80:80 --name what2play-frontend $prefix-frontend
