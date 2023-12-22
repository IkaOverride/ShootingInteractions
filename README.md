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
* Random chance for grenades to "malfunction" when shooting on them, you can configure the fuse time of the grenade (time it takes before exploding)
* Remote keycard (Check keycards in the player's inventory to open doors that requires a keycard)

# Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the plugin enabled
| debug | bool | false | Are the plugin's debug logs enabled
| accurate_bullets | bool | false | Should it take into account where the bullet actually lands, instead of the center of the player's screen
| doors | DoorInteraction | See DoorInteraction config | Default doors buttons interaction
| checkpoints | DoorInteraction | See DoorInteraction config | Checkpoint doors buttons interaction
| gates | DoorInteraction | See DoorInteraction config | Gates buttons interaction
| weapon_lockers | LockerInteraction | See LockerInteraction config | Weapon lockers interaction
| bulletproof_lockers | BulletproofLockerInteraction | See BulletproofLockerInteraction config | Bulletproof lockers interaction
| elevators | ElevatorInteraction | See ElevatorInteraction config | Elevators buttons interaction
| frag_grenades | TimedProjectileInteraction | See TimedProjectileInteraction config | Frag grenades interaction
| flashbangs | TimedProjectileInteraction | See TimedProjectileInteraction config | Flashbangs interaction
| custom_grenades | ProjectileInteraction | See ProjectileInteraction config | Custom grenades interaction
| scp2176 | ProjectileInteraction | See ProjectileInteraction config | SCP-2176 interaction

# DoorInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the interaction enabled
| remote_keycard | bool | false | Should it check keycards in the player's inventory
| buttons_break_chance | byte | 0 | Percentage of chance for the buttons to break (0 is disabled)
| buttons_break_time | float | 10 | For how long should the buttons stay broken (0 or less is infinite)
| move_before_breaking | bool | true | Should the door still move/do its animation before breaking

# LockerInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the interaction enabled
| remote_keycard | bool | true | Should it check keycards in the player's inventory

# BulletproofLockerInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the interaction enabled
| remote_keycard | bool | true | Should it check keycards in the player's inventory
| only_keypad | bool | true | Should it only work when shooting on the keypad (false = whole interactable)

# ElevatorInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the interaction enabled
| buttons_break_chance | byte | 0 | Percentage of chance for the elevator to break (0 is disabled)
| buttons_break_time | float | 10 | For how long should the elevator stay broken (0 or less is infinite)
| move_before_breaking | bool | true | Should the elevator still move/do its animation before breaking

# ProjectileInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the interaction enabled

# TimedProjectileInteraction Config
| Config | Type | Default | Description |
| :-------------: | :---------: | :---------: | :---------:
| is_enabled | bool | true | Is the interaction enabled
| malfunction_chance | byte | 0 | Percentage of chance for the grenade to malfunction (0 is disabled)
| malfunction_fuse_time | float | 0 | Grenade's fuse time when malfunction occurs
| additional_velocity | bool | false | Should the grenade get additional velocity to where the player is facing
| velocity_force | float | 7.5 | The force of the velocity that gets added
