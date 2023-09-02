# ShootingInteractions
Exiled [SCP: Secret Laboratory] plugin

Feel free to make suggestions

# Features
* Interact with door when a player is shooting on one of its buttons (Disable in config)
* Remote Keycard (Open keycard doors if the player has the right keycard in their inventory when shooting on it) (Enable in config, otherwise it nevers open)
* Keycard bypass support
* Interact with elevator when a player is shooting on one of its buttons (Disable in config)
* Random chance for buttons to "break" when shooting on it, can configure for how long, and can configure if the door should do its animation before the buttons breaking (Enable in config)
* Make grenade explode when a player shoots on it (Disable in config)

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
| grenades | bool | false | Should the plugin work with grenades
