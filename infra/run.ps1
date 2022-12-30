# Start up docker-compose
docker compose up -d

# Copy over all environment variables from the .env file to be used in the dotnet project
$EnvFileContent = Get-Content -Path .\.env | Out-String
$Regex = '(?<name>\w+)=(?<value>\w+)'

foreach($Line in Get-Content .\.env) {
    if($Line -match $Regex){
        [Environment]::SetEnvironmentVariable($Matches.name, $Matches.value)
    }
}

# Start up dotnet project to ingest active process
dotnet run --project ../src/WorkStationMonitor.Console/WorkStationMonitor.Console.csproj
