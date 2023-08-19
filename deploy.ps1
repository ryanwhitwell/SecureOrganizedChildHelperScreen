# Sochs Deployment Script (Powershell)
#
# This script moves the compiled WinForms app Sochs.Display to the Sochs application host using scp
# You must have the following environment variables set on the deployment machine
#
# Username = SOCHS_SSH_USERNAME
# Password = SOCHS_SSH_PASSWORD
# Host = SOCHS_SSH_HOST

$PsVersionTable

$SochsUsername = $Env:SOCHS_SSH_USERNAME
$SochsPassword = $Env:SOCHS_SSH_PASSWORD
$SochsHost = $Env:SOCHS_SSH_HOST
$SochsPort = $Env:SOCHS_SSH_PORT
$WeatherApiKey = $Env:WEATHER_API_KEY



# Only run the deployment if the host is available
try 
{
  dotnet publish .\Sochs.App\ -c Release --sc true

  Write-Host "Connecting to " + $SochsUsername + "@" + $SochsHost  

  $socket = New-Object System.Net.Sockets.TcpClient($SochsHost, $SochsPort)

  if($socket.Connected) 
  {
    $socket.Close()

    $deploymentPath = $SochsUsername + "@" + $SochsHost + ":/home/" + $SochsUsername + "/Sochs"

    Write-Host "Deploying Sochs to $deploymentPath"

    scp -vr ./Sochs.App/bin/Release/net7.0/publish/wwwroot/* $deploymentPath

    Write-Host "Successfully deployed Sochs to $deploymentPath"
  }
} 
catch 
{
  Write-Host "Error deploying Sochs"
  Write-Host $_
}