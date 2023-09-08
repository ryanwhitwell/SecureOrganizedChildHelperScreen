# S.O.C.H.S.
## Secure Organized Child Helper Screen
A secure sandbox display used to help include young children in the day-to-day of getting things done... it's also a [Minecraft](https://www.minecraft.net/en-us) server.

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
- Daily Tasks
  - To help the kids keep on task in the following times of the day:
    - *Morning* - What does each child need to do to in the morning to transition into the afternoon
    - *Afternoon* - What does each child need to do in the afternoon to successfully transition into the evening
    - *Evening* - What does each child need to do after dinner to get ready for nighttime 
    - *Night* - What does each child need to do leading up to bedtime
  - This flow chart contains a visual reward system to help promote engagement and gives the kids some bragging right when they've done everything on their lists
- Date Events
  - Date events are events that happen on a specific date and are available to view for the entire day on the date when then event occurs
  - The event can be viewed by clicking the below icon on the interface  
    ![image](https://img.icons8.com/stickers/64/light-on.png)
- Minecraft Server Info
  - Server data is provided by the [Plan](https://cloudburstmc.org/resources/plan-player-analytics.345/) plugin for the [Nukkit](https://github.com/Nukkit/Nukkit) minecraft server 
  - You can view details of the minecraft server by clicking on the below icon  
   ![image](https://img.icons8.com/stickers/64/minecraft-pickaxe.png)
___

## Minecraft Info

### URIs
> Thsese are the URIs of the Minecraft services available on Sochs

| Service               |URI                          
|------------------------|---------------------------------|
|Minecraft Server Address|`http://whitwell.hopto.org:9132` |
|Minecraft Player Stats  |`http://whitwell.hopto.org:8804` |

*If anonymous users start logging into the server then I'll implement a white-list to prevent unwanted connections.*
___

## Operational Info

### Required Environment Variables
> These are the environemt variables needed on the **deployment host** in order to successfully deploy the application.
- WEATHER_API_KEY

### Backup Minecraft server process
Run the following script from SSH using the command below while in the `/home/pi/` directory replacing the `DROPBOX_API_KEY` value with a real API Key

`nohup ./backup_minecraft_server.sh DROPBOX_API_KEY >> ./minecraft_backup.log 2>&1 &`

### Enable startup in kiosk mode
Edit this file

sudo nano /etc/xdg/lxsession/LXDE-pi/autostart

And add this:

```
@xset s off
@xset -dpms
@xset s noblank
@chromium-browser --kiosk http://localhost  # load chromium after boot and open the website in full screen mode
```

Then reboot. Chromium should automatically launch in fullscreen mode after the desktop has loaded.

Note: The default file ( /etc/xdg/lxsession/LXDE-pi/autostart ) is the global autostart and is executed each time a user logs into the desktop. In case you want to use local autostart, use /home/pi/.config/lxsession/LXDE-pi/autostart. Local autostart(s) are useful when you want to run different programs or configurations for each user.


### Update Mouse X-Axis Orientation
 https://www.instructables.com/Rotate-Raspberry-Pi-Display-and-Touchscreen/
 > This guide provides notes on the screen rotation and hints at the mouse x-axis problem, but doesn't offer a solution.

 ---

## Credits

>Sochs makes use of the following applications and services.

 - [Raspberry Pi](https://www.raspberrypi.com/)
 - [CanaKit Raspberry Pi 4 1GB](https://www.amazon.com/s?k=CanaKit+Raspberry+Pi+4+1GB&i=electronics&crid=PCE7UVWEZZ3E&sprefix=canakit+raspberry+pi+4+1gb%2Celectronics%2C131&ref=nb_sb_noss)
 - [Dropbox API](https://dropbox.github.io/dropbox-api-v2-explorer)
 - [Weather API](https://www.weatherapi.com/)
 - [Nukkit](https://github.com/Nukkit/Nukkit)
 - [Nukkit Plugins](https://cloudburstmc.org/resources/categories/nukkit-plugins.1/)
 - [Icons - Stickers](https://icons8.com/icons/stickers)
 - [Icons - Cute Color](https://icons8.com/icons/dusk)
 - [Toch Screen](https://www.planar.com/products/desktop-touch-screen-monitors/15-22-inch-touch-screen-monitors/pct2265/)


## Image Gallery

  |           |       |
  |--------------|-----------|
  | ![image](/IMG_4009.jpg) | ![image](/IMG_4010.jpg) |
  | ![image](/IMG_4011.jpg) | ![image](/IMG_4013.jpg) |
  | ![image](/IMG_4014.jpg) | ![image](/IMG_4015.jpg) |
  | ![image](/IMG_4016.jpg) | ![image](/IMG_4017.jpg) |