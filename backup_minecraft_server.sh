#!/bin/bash

# Run this script from SSH using the command below while in the /home/pi/ directory
# nohup ./backup_minecraft_server.sh DROPBOX_API_KEY >> ./minecraft_backup.log 2>&1 &

# Get API Token from parameter
api_token=$1 &&\

# Stop the script if API token was not provided
if [[ -z "$api_token" ]]; then
   exit 1
fi

# Stop Minecraft server if it is running
if [[ -z $(ps aux | grep '[m]inecraft_server.jar' | awk '{print $2}') ]]; then
  echo "Minecraft server is not running."
else
  echo "Minecraft server is running." &&\
  echo "Stopping Minecraft server." &&\
  kill $(ps aux | grep '[m]inecraft_server.jar' | awk '{print $2}')
fi

# Generate backup file
backup_file_name='minecraft_server_bak_'$(date -Iseconds)'tar.gz' &&\
folder="/minecraft_server_bak/" &&\
path=${folder}${backup_file_name} &&\

tar -czf /home/pi/${backup_file_name} /home/pi/mcserver &&\

# Upload data to dropbox
curl -X POST https://content.dropboxapi.com/2/files/upload \
  --header 'Authorization: Bearer '${api_token} \
  --header 'Content-Type: application/octet-stream' \
  --header 'Dropbox-API-Arg: {"path":"'${path}'"}' \
  --data-binary @'/home/pi/'${backup_file_name} &&\

# Remove local backup file
rm /home/pi/${backup_file_name} &&\

# Start server
cd /home/pi/mcserver &&\
java -jar minecraft_server.jar