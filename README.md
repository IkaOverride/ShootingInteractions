# ShootingInteractions
Exiled [SCP: Secret Laboratory] plugin

Feel free to make suggestions

# Features
* Custom interactions when shooting with weapons.
* Supports doors' buttons and elevators' buttons for now (Disable in config)
* Random chance for a door/elevator's buttons to "break", can configure for how long and if the door/elevator should do its animation before the buttons breaking (Enable in config)
* Open keycard doors if the player has the right keycard in their inventory when shooting on it (Enable in config, otherwise it nevers open)

# Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Indicates whether the plugin is enabled or not
| debug | bool | false | Indicates whether the plugin's debug logs are enabled or not
| doors | bool | true | Should the plugin work with doors
| door_check_keycard | bool | false | Should the plugin work with keycard doors? (false = never opens, true = opens if the keycard is in their inventory)
| door_buttons_break_chance | byte | 0 | Percentage of chance for a door's buttons to break (0 = never break, 100 = always break)
| door_move_before_breaking | bool | true | Let the door move before breaking the buttons
| door_buttons_break_time | float | 15 | For how long should the door's buttons stay broken (0 = infinite)
| elevators | bool | true | Should the plugin work with elevators
| elevator_buttons_break_chance | byte | 0 | Percentage of chance for an elevator's buttons to break (0 = never break, 100 = always break)
| elevator_move_before_breaking | bool | false | Let the elevator move before breaking the buttons
| elevator_buttons_break_time | float | 15 | For how long should the elevator's buttons stay broken (0 = infinite)
