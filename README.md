# Recipe Scroll Fix

Fixes the recipe list in the crafting / upgrade window (workbench, forge, cauldron, ...) jumping straight to the
end of the list when you scroll the mouse wheel - especially noticeable with long lists from many mods.

The list now moves a fixed number of rows per wheel notch instead of using the oversized scroll value the game
passes to the list.

## Features

- Fixed scroll step per mouse wheel notch for the recipe list
- Configurable speed, changes apply immediately (no restart)
- Can be toggled off to get vanilla behaviour back
- Client-side only - not needed on servers
- Only touches the recipe list; inventory, chests and other scroll views are untouched

## Configuration

Use [Configuration Manager](https://thunderstore.io/c/valheim/p/shudnal/ConfigurationManager/) (default key F1)
or edit `BepInEx/config/r4b2.RecipeScrollFix.cfg`.

| Setting | Default | Description |
| --- | --- | --- |
| Enabled | true | Use the fixed scroll step. Off = vanilla. |
| Rows per mouse wheel notch | 2 | 0.25 - 10. How many recipe rows one notch scrolls. |
| Log scroll values (advanced) | false | Logs raw scroll values to the BepInEx log, handy for bug reports. |

## Installation

Install with Gale, r2modman or the Thunderstore Mod Manager, or manually place `RecipeScrollFix.dll` in
`BepInEx/plugins`. Requires BepInExPack Valheim.

## Troubleshooting

If scrolling is unchanged, check `BepInEx/LogOutput.log` for the line `Recipe list ScrollRect found`. If you see
`Could not find the recipe list ScrollRect` instead, another mod may have restructured the crafting UI - please
open an issue and include your log.

## Source / issues

https://github.com/r4b2/RecipeScrollFix

## AI disclosure

This mod was written with the help of an AI assistant (Claude) and tested in-game by the author.
