# ShootingInteractions
Exiled [SCP: Secret Laboratory] plugin

New interactions when shooting on objects!
(If there's any issues, if you have any suggestions or if you want to help me improve the code feel free to DM me or open a github issue)
Discord : @ika7533

# Interactions
* Interact with doors by shooting on their buttons
* Interact with elevators by shooting on their buttons
* Interact with lockers and bulletproof (SCP) lockers when shooting on them
* Make grenades explode when shooting on them, and (not for custom grenades) choose if they explode instantly or not
* Make SCP-2176 shatter when shooting on it

# Other features
* Bypass mode support
* Random chance for buttons to "break" when shooting on them, you can configure for how long, and if the door/elevator should move before the buttons break
* Remote Keycard (Check keycards in the player's inventory to open doors that requires a keycard)

# Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Indicates whether the plugin is enabled or not
| debug | bool | false | Indicates whether the plugin's debug logs are enabled or not
| accurate_bullets | bool | false | Takes into account where the bullet actually lands instead of where the user is looking
| doors | DoorInteraction | See DoorInteraction config | Normal doors buttons
| checkpoints | DoorInteraction | See DoorInteraction config | Checkpoint doors buttons
| gates | DoorInteraction | See DoorInteraction config | Gates buttons
| weapon_lockers | LockerInteraction | See LockerInteraction config | Weapon lockers
| bulletproof_lockers | BulletproofLockerInteraction | See BulletproofLockerInteraction config | Bulletproof lockers
| elevators | ElevatorInteraction | See ElevatorInteraction config | Elevators buttons
| frag_grenades | TimedProjectileInteraction | See TimedProjectileInteraction config | Frag grenades
| flashbangs | TimedProjectileInteraction | See TimedProjectileInteraction config | Flashbangs
| custom_grenades | ProjectileInteraction | See ProjectileInteraction config | Custom grenades
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

# BulletproofLockerInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| remote_keycard | bool | true | Does the interaction check keycards in the player's inventory
| only_keypad | bool | true | Should the interaction work only when shooting on the locker's keypad

# ElevatorInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| buttons_break_chance | byte | 0 | Percentage of chance for the buttons to break
| buttons_break_time | float | 10 | For how long should the buttons stay broken
| move_before_breaking | bool | true | Should the door still move/do its animation if the buttons should break

# ProjectileInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled

# TimedProjectileInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the shooting interaction enabled
| explode_instantly | bool | false | Should the projectile explode instantly
