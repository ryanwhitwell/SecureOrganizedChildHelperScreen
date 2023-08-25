# S.O.C.H.S.
## Secure Organized Child Helper Screen
A secure sandbox display used to help include young children in the day-to-day of getting things done... it's also a Minecraft server :)

>Please note this is a Blazor WASM application and is meant to be viewed in portrait orientation at a resolution of 1080 X 1920. Just keep that in mind when you're running it locally.

This application is inteneded to give my kids enough information about their day to allow them to self-serve. Here's a list of the high-level features:

- Basic date/time information
  - To help reinforce learning to read/manage time
- Weather data
  - To help manage expectations about what we can do outside for the day
- Recommended clothes
  - To help the kids understand what kinds of clothes to wear for the day given the current weather 
- Shool lunch menu
  - To help the family decide if we're making lunch in the morning or eating at school
- Activity flow chart
  - To help the kids keep on task in the morning, afternoon, and at night
    - *Morning* - What does each child need to do to get ready to go to school (weekdays) or transition into the afternoon (weekends)
    - *Afternoon* -  What does each child need to do after school (weekdays) or after lunch (weekends) to successfully transition into the evening
    - *Evening* - What does each child need to do after dinner to get ready for bed 
  - This flow chat contains a visual reward system to help promote engagement and gives the kids some bragging right when they've done everything on their lists
___

## Minecraft Info

### URIs
> Thsese are the URIs of the Minecraft services available on Sochs

| Service               |URI                          
|------------------------|---------------------------------|
|Minecraft Server Address|`http://whitwell.hopto.org:9132` |
|Minecraft Player Stats  |`http://whitwell.hopto.org:8804` |

*If anonymous users start logging into the server then I'll implement a white list for connectivity.*
___

## Sochs Info

### Required Environment Variables
> These are the environemt variables needed on the **deployment host** in order to successfully deploy the application.
- WEATHER_API_KEY

### Backup process
Run the following script from SSH using the command below while in the `/home/pi/` directory replacing the `DROPBOX_API_KEY` value with a real API Key

`nohup ./backup_minecraft_server.sh DROPBOX_API_KEY >> ./minecraft_backup.log 2>&1 &`

## Credits

>Sochs makes use of the following applications and services.

 - [Dropbox API](https://dropbox.github.io/dropbox-api-v2-explorer)
 - [Weather API](https://www.weatherapi.com/)
 - [Nukkit](https://github.com/Nukkit/Nukkit)