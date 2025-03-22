# Ruta base donde se generan los binarios
$releasePath = "C:\Users\Kratos\source\repos\ADUserGroupManager\ADUserGroupManager\bin\Release\net48"

# Carpeta donde ILMerge pondrá el ejecutable final
$outputPath = "$releasePath\Install"

# Crear carpeta Install si no existe
if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath
}

# Ruta exacta de ILMerge.exe
$ilMergePath = "C:\Users\Kratos\source\repos\ADUserGroupManager\packages\ILMerge.3.0.41\tools\net452\ILMerge.exe"
if (-not (Test-Path $ilMergePath)) {
    Write-Host "ILMerge.exe no encontrado en $ilMergePath" -ForegroundColor Red
    exit
}

# Obtener lista de DLLs en la carpeta Release\net48
$dlls = Get-ChildItem -Path $releasePath -Filter *.dll | ForEach-Object { $_.FullName }
if ($dlls.Count -eq 0) {
    Write-Host "No se encontraron DLLs en la carpeta $releasePath" -ForegroundColor Yellow
    exit
}

# Generar el comando de ILMerge
$exeFile = "$releasePath\ADUserGroupManager.exe"
$outputFile = "$outputPath\ADUserGroupManagerMerged.exe"
$cmdArgs = "/out:$outputFile $exeFile " + ($dlls -join " ")

# Ejecutar ILMerge
Write-Host "Ejecutando ILMerge..." -ForegroundColor Green
& $ilMergePath $cmdArgs.Split(" ")

if ($LASTEXITCODE -eq 0) {
    Write-Host "ILMerge completado exitosamente. Archivo generado en $outputFile" -ForegroundColor Green
} else {
    Write-Host "Error durante la ejecución de ILMerge." -ForegroundColor Red
    exit
}

# Copiar credentials.json al directorio Install
$credentialsFile = "$releasePath\credentials.json"
if (Test-Path $credentialsFile) {
    Copy-Item -Path $credentialsFile -Destination $outputPath -Force
    Write-Host "Archivo credentials.json copiado a $outputPath" -ForegroundColor Green
} else {
    Write-Host "El archivo credentials.json no se encontró en $releasePath" -ForegroundColor Yellow
}
