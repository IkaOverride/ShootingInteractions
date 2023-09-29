# ShootingInteractions
Exiled [SCP: Secret Laboratory] plugin

New interactions when shooting on objects!
(If there's any issues, if you have any suggestions or if you want to help me improve the code feel free to DM me or open a github issue)
Discord : @ika7533

# Features
* Interact with doors by shooting on their buttons
* Remote Keycard (Check keycards in the player's inventory to open doors that requires a keycard)
* Interact with elevators by shooting on their buttons
* Random chance for buttons to "break" when shooting on them, you can configure for how long, and if the door/elevator should move before the buttons break
* Interact with bulletproof lockers (SCP Lockers) when shooting on their keycard reader
* Bypass support
* Make frag grenades and flashbangs explode when shooting on them, and choose if they explode instantly or not
* Make SCP-2176 shatter when shooting on it

# Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Indicates whether the plugin is enabled or not
| debug | bool | false | Indicates whether the plugin's debug logs are enabled or not
| doors | DoorInteraction | See DoorInteraction config | Normal doors buttons
| checkpoints | DoorInteraction | See DoorInteraction config | Checkpoint doors buttons
| gates | DoorInteraction | See DoorInteraction config | Gates buttons
| bulletproof_lockers | LockerInteraction | See LockerInteraction config | Bulletproof lockers keycard readers
| elevators | ElevatorInteraction | See ElevatorInteraction config | Elevators buttons
| frag_grenades | TimedProjectileInteraction | See TimedProjectileInteraction config | Frag grenades
| flashbangs | TimedProjectileInteraction | See TimedProjectileInteraction config | Flashbangs
| scp2176 | ProjectileInteraction | See ProjectileInteraction config | SCP-2176

# DoorInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| remote_keycard | bool | false | Does the interaction check keycards in the player's inventory
| buttons_break_chance | byte | 0 | Percentage of chance for the buttons to break
| buttons_break_time | float | 10 | For how long should the buttons stay broken
| move_before_breaking | bool | true | Should the door still move/do its animation if the buttons should break

# LockerInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| remote_keycard | bool | true | Does the interaction check keycards in the player's inventory

# ElevatorInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| buttons_break_chance | byte | 0 | Percentage of chance for the buttons to break
| buttons_break_time | float | 10 | For how long should the buttons stay broken
| move_before_breaking | bool | true | Should the door still move/do its animation if the buttons should break

# TimedProjectileInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| explode_instantly | bool | false | Should the projectile explode instantly

# ProjectileInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
