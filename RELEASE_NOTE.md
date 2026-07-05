# Vanguard Galaxy Board Always — no more RNG boarding

Hey everyone — small BepInEx plugin that removes the RNG gate from ship boarding and gives you a few knobs to tune dungeon difficulty and hull integrity damage. If you've ever sat there pumping shots into a crippled enemy waiting for the boarding RNG to tick, this one's for you.

**What it does**

- **Always boardable** — enemy ships below 40% HP become boardable on every damage tick instead of relying on the vanilla RNG roll. The 15% damage accumulation gate is bypassed entirely.
- **EMP synergy** — boarding chance scales with EMP charge on the target, so EMP weapons are a strategic choice for capture-focused builds.
- **Difficulty modifier** — scales defender combat power and HP globally. Set below 1.0 to make high-level dungeons easier, or above 1.0 for a tougher challenge.
- **Integrity damage control** — configurable multiplier for hull integrity loss during boarding. Set to 0.0 to prevent integrity loss entirely, or a small value (e.g. 0.05) for very gradual degradation.
- **Scuttling protection** — integrity damage from emergency scuttling is also scaled by the multiplier, so your ship doesn't take unintended hull damage when boarding goes wrong.

All settings live in `BepInEx/config/vg.boardalways.cfg` after first launch — no config file editing required up front, the plugin works out of the box with sensible defaults.

**Install**
Grab the zip from the release, drop the `VGBoardAlways/` folder into `BepInEx/plugins/`. Requires BepInEx 5.x.

Release: https://github.com/fank/vanguard-galaxy-board-always/releases/latest
Source + readme: https://github.com/fank/vanguard-galaxy-board-always

Tested on the current Steam build. As with any Harmony patcher, a future game update could break the patches if the internal methods or fields it hooks are renamed. I'll push a fix when that happens.

Feedback / bug reports welcome — especially if you run into edge cases with unusual hull types, dungeon levels way outside the expected range, or interactions with other boarding-related mods.
