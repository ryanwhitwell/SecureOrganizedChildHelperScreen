#!/bin/sh

# Stop server
kill $(ps aux | grep '[m]inecraft_server.jar' | awk '{print $2}') &&\

# Get API Token
api_token=$DROPBOX_API_KEY &&\

# Generate backup file
backup_file_name='minecraft_server_bak_'$(date -Iseconds)'tar.gz' &&\

tar -czf /home/pi/${backup_file_name} /home/pi/mcserver &&\

# Upload data to dropbox
folder="/minecraft_server_bak/" &&\

path=${folder}${backup_file_name} &&\

curl -X POST https://content.dropboxapi.com/2/files/upload \
  --header 'Authorization: Bearer '${api_token} \
  --header 'Content-Type: application/octet-stream' \
  --header 'Dropbox-API-Arg: {"path":"'${path}'"}' \
  --data-binary @'/home/pi/'${backup_file_name} &&\

# Remove local backup file
rm /home/pi/${backup_file_name} &&\

# Start server
cd /home/pi/mcserver && java -jar minecraft_server.jar