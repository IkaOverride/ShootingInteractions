# TargetDoor
Exiled [SCP: Secret Laboratory] plugin

Take over shooting on doors' buttons!

Feel free to make suggestions

# Features
* Open doors by shooting on one of their button (Does not work with elevators nor bulletproof lockers).
* Chance for the door's buttons to break and choose if it breaks before or after the door's animation.
* Open the door if the player has the right keycard in their inventory 

# Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Indicates whether the plugin is enabled or not
| debug | bool | false | Indicates whether the plugin's debug logs are enabled or not
| check_keycard | bool | false | Should a door that requires a keycard open if the player has a keycard to open it
| door_break_chance | byte | 0 | The percentage of chance for a door's buttons to break when a player shoot on one
| door_break_before | bool | true | Should the button break BEFORE the doors open or close
| door_break_time | float | 15 | For how long should the door stay broke (0 = infinite)
