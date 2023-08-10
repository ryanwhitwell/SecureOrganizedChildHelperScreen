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

# Only run the deployment if the host is available
try 
{
  $socket = New-Object System.Net.Sockets.TcpClient($SochsHost, $SochsPort)

  if($socket.Connected) 
  {
    $socket.Close()

    $deploymentPath = $SochsUsername + "@" + $SochsHost + ":/home/" + $SochsUsername + "/Sochs"

    "Deploying Sochs to $deploymentPath"

    scp -v ./Sochs.Display/bin/Release/Sochs.Display.exe $deploymentPath

    "Successfully deployed Sochs to $deploymentPath"
  }
} 
catch 
{
  "Error deploying Sochs"
}