# DonutzMozaPlugin

This Simhub plugin is meant to control Moza Wheel Base settings based on active game/SIM and the currently selected car.

With help of the included Simhub Dashboard you can configure the wheelbase settings even while driving. When you tick the box at "ACTIVE GAME MAPPING" and/or "ACTIVE CAR MAPPING" and then start a game which does not already have a saved profile assigned, the plugin will load the current settings from MOZA Pit House.
You might need hit the "REFRESH PROFILE LIST" button in order to see the new entry. Then you can adjust the wheel base settings inside the plugin. Alternatively you can use the included Dashboard from inside the car.

This project is still in alpha phase, so stay tuned for updates in near future.

<br>Buy me a coffee if you want at [Paypal](https://paypal.me/donutz75?country.x=DE&locale.x=de_DE).

## Installation: 
Just drop all three .dll files (DonutzMozaPlugin.dll, MOZA_API_C.dll and MOZA_API_CSharp.dll) into your Simhub install folder and restart Simhub.

## Release Notes
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


