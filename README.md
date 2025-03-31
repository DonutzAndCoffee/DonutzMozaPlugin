# DonutzMozaPlugin

This Simhub plugin is meant to control Moza Wheel Base settings based on active game/SIM and the currently selected car.

With help of the included Simhub Dashboard you can configure the wheelbase settings even while driving. When you tick the box at "ACTIVE GAME MAPPING" and/or "ACTIVE CAR MAPPING" and then start a game which does not already have a saved profile assigned, the plugin will load the current settings from MOZA Pit House.
You might need hit the "REFRESH PROFILE LIST" button in order to see the new entry. Then you can adjust the wheel base settings inside the plugin. Alternatively you can use the included Dashboard from inside the car.

This project is still in alpha phase, so stay tuned for updates in near future.

Discord: https://discord.gg/KuSsEYgB3k

<br>Buy me a coffee if you want at [Paypal](https://paypal.me/donutz75?country.x=DE&locale.x=de_DE).

## Source Code Availability

This repository contains only the compiled version of the Simhub Moza Plugin.  
The source code is **not publicly available** at the moment, but it may be released in a future version.

## Included Moza SDK Files

This plugin package includes two DLL files from the Moza SDK:

- `MOZA_API_C.dll`
- `MOZA_API_CSharp.dll`

These files are provided by Moza and are included here solely for the purpose of enabling integration with the Moza hardware.  
For more information about the Moza SDK, please visit [Moza's official website](https://www.mozaracing.com/moza-sdk/). The latest version of the dll files can be dowloaded there.

### Disclaimer

The included Moza SDK files are the property of Moza.  
I am not affiliated with Moza, and any issues related to these files should be addressed to Moza's support.

## Installation: 
Just drop all three .dll files (DonutzMozaPlugin.dll, MOZA_API_C.dll and MOZA_API_CSharp.dll) into your Simhub install folder and restart Simhub.

## Release Notes
v0.2 (2025-03-31)
- added an indicator that marks the active profile
  
v0.1.2 (2024-11-24)
- bug fix: car model name was handled incorrectly in some situations when adding new car related profiles.

v0.1.1 (2024-11-22)
- changed behaviour of the mouse wheel. It should scroll the page correctly now.

v0.1.0 (2024-11-22)
- changed behaviour for new car profiles. It will pick the game related profile if there is one already instead of the current Moza settings.

v0.0.5 (2024-11-20):
- added some tooltips and minor bugfixes.

v0.0.4 (2024-11-14): 
- added Center Wheel keybinding
- added readable car names to profiles
- added reverse FFB to game settings

v0.0.3: Bug fixes and new feature: Learn New Cars (yes/no). If set to no, the plugin tries to find the corresponding profile for the car. If there is none, it will fall back to the game profile.

v0.0.1: Initial release

![grafik](https://github.com/user-attachments/assets/732f3598-6342-409a-bd15-612604a3f4fa)


