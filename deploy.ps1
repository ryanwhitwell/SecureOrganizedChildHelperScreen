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

function Format-Json {
    <#
    .SYNOPSIS
        Prettifies JSON output.
    .DESCRIPTION
        Reformats a JSON string so the output looks better than what ConvertTo-Json outputs.
    .PARAMETER Json
        Required: [string] The JSON text to prettify.
    .PARAMETER Minify
        Optional: Returns the json string compressed.
    .PARAMETER Indentation
        Optional: The number of spaces (1..1024) to use for indentation. Defaults to 4.
    .PARAMETER AsArray
        Optional: If set, the output will be in the form of a string array, otherwise a single string is output.
    .EXAMPLE
        $json | ConvertTo-Json  | Format-Json -Indentation 2
    #>
    [CmdletBinding(DefaultParameterSetName = 'Prettify')]
    Param(
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        [string]$Json,

        [Parameter(ParameterSetName = 'Minify')]
        [switch]$Minify,

        [Parameter(ParameterSetName = 'Prettify')]
        [ValidateRange(1, 1024)]
        [int]$Indentation = 4,

        [Parameter(ParameterSetName = 'Prettify')]
        [switch]$AsArray
    )

    if ($PSCmdlet.ParameterSetName -eq 'Minify') {
        return ($Json | ConvertFrom-Json) | ConvertTo-Json -Depth 100 -Compress
    }

    # If the input JSON text has been created with ConvertTo-Json -Compress
    # then we first need to reconvert it without compression
    if ($Json -notmatch '\r?\n') {
        $Json = ($Json | ConvertFrom-Json) | ConvertTo-Json -Depth 100
    }

    $indent = 0
    $regexUnlessQuoted = '(?=([^"]*"[^"]*")*[^"]*$)'

    $result = $Json -split '\r?\n' |
        ForEach-Object {
            # If the line contains a ] or } character, 
            # we need to decrement the indentation level, unless:
            #   - it is inside quotes, AND
            #   - it does not contain a [ or {
            if (($_ -match "[}\]]$regexUnlessQuoted") -and ($_ -notmatch "[\{\[]$regexUnlessQuoted")) {
                $indent = [Math]::Max($indent - $Indentation, 0)
            }

            # Replace all colon-space combinations by ": " unless it is inside quotes.
            $line = (' ' * $indent) + ($_.TrimStart() -replace ":\s+$regexUnlessQuoted", ': ')

            # If the line contains a [ or { character, 
            # we need to increment the indentation level, unless:
            #   - it is inside quotes, AND
            #   - it does not contain a ] or }
            if (($_ -match "[\{\[]$regexUnlessQuoted") -and ($_ -notmatch "[}\]]$regexUnlessQuoted")) {
                $indent += $Indentation
            }

            $line
        }

    if ($AsArray) { return $result }
    return $result -Join [Environment]::NewLine
}

# Update config file for deployment
$json = Get-Content ".\Sochs.App\wwwroot\appsettings.json" | ConvertFrom-Json 
$json.Application.WeatherApiKey = "" + $WeatherApiKey + ""
$json.Application.MockEnabled = "false"
$json.Application.MinecraftServerAddress = "localhost"
$json | ConvertTo-Json -Depth 99 | Format-Json -Indentation 2 | Out-File ".\Sochs.App\wwwroot\appsettings.json"

#<#

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

    $backupScriptPath = $SochsUsername + "@" + $SochsHost + ":/home/" + $SochsUsername

    Write-Host "Deploying Sochs to $deploymentPath"

    scp -vr ./backup_minecraft_server.sh $backupScriptPath
    scp -vr ./Sochs.App/bin/Release/net7.0/publish/wwwroot/* $deploymentPath

    Write-Host "Successfully deployed Sochs to $deploymentPath"
  }
} 
catch 
{
  Write-Host "Error deploying Sochs"
  Write-Host $_
}

##>


# Reset config file after deployment
$json = Get-Content ".\Sochs.App\wwwroot\appsettings.json" | ConvertFrom-Json 
$json.Application.WeatherApiKey = "WEATHER_API_KEY"
$json.Application.MockEnabled = "true"
.$json.Application.MinecraftServerAddress = "10.0.0.200"
$json | ConvertTo-Json -Depth 99 | Format-Json -Indentation 2 | Out-File ".\Sochs.App\wwwroot\appsettings.json"


