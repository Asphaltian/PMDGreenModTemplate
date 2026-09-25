# Green Rescue Team Mod Template

This is an example mod for [Pokémon Mystery Dungeon: Green Rescue Team](https://github.com/Asphaltian/pmd-green) that you can use as a starting point for your own. It makes your team gain more experience, and players can pick how much from the mod's settings.

## Building

You will need the .NET 10 SDK and a copy of Green Rescue Team. Mods are built against the game's own files, so you need to tell the build where `pmd_green` is with `PMDGreenPath`. You can pass it to the build, or set it as an environment variable:

```
dotnet build -c Release -p:PMDGreenPath="C:\Games\Green Rescue Team"
```

This gives you `bin/Release/net10.0/pmd_green_mod_template.zip`. Put it in the `mods` folder where the game keeps [your data](https://github.com/Asphaltian/pmd-green#your-data) and start the game.

## Making It Your Own

First, change `id` in `mod.json` and set `AssemblyName` in the project to the same name. Please note that the game loads your code from `id.dll`, so if these two don't match, your code will never run.

The rest of `mod.json` is what players see in the mod list:

* `minimum_recomp_version` is the oldest version of the game your mod works with.
* `config_schema` lists the settings players can change. You read them from your code with `mod.Config`.

If you want your mod to have a picture, put a `thumb.png` next to `mod.json` and it goes into the zip too.

Any other files your mod needs go in the project as `ModFile` items, and you read them from your code with `mod.ReadFile`:

```xml
<ItemGroup>
  <!-- Ends up in the zip as files/T01P01.bpl, same path as in your project -->
  <ModFile Include="files/T01P01.bpl" />
</ItemGroup>
```

## Changing the Game

Your mod starts in `Load`, which runs once before the game starts. This is where you hook and replace the game's functions:

```csharp
// Run your code every time the game calls a function
mod.Hook(ref Funcs.Patches.AddExpPoints, ctx => ctx.R2 *= 2);

// Or swap the function for your own, and call the game's version whenever you need it
mod.Replace(ref Funcs.Patches.CalculateEXPGain, ctx =>
{
    Funcs.Original.CalculateEXPGain(ctx);
    ctx.R0 += 10;
});

// Read the game's variables by name
short map = Memory.Peek<short>(Data.gCurrentMap);
```

`Funcs` holds every function in the game, and `Data` holds the address of everything else. Both are named after the symbols from [pret/pmd-red](https://github.com/pret/pmd-red), so the decompilation is the best place to check what each of them does and what arguments it takes.

## Green Rescue Team API

On top of the game's own functions, we have a few classes made specifically for mods:

* `GroundMap.Selected` runs whenever a new map is loaded in towns and cutscenes.
* `WorldSprites.Drawing` is where you draw sprites that belong to the map. They scroll smoothly with the camera and carry on into the widescreen margins.
* `MonsterData<T>` keeps your own data for each monster in a dungeon.
* `FileSystem.Replace` makes the game load your own version of one of its files, like a map's palette. You'll find the names of the files in the decompilation's tables, like `gGroundFiles`.
* `RecompInput.CameraInputs` and `RecompInput.MouseDeltas` give you the right stick and the mouse, which the GBA doesn't have.
* `GameFrame.ShowsMargins` and `GameFrame.InterpolatesMotion` let you turn off widescreen and smooth motion while your mod shows something that doesn't work well with them.

Keep in mind that everything your mod puts in memory for the game, like the files you replace, comes out of 16 MB of extra memory that all mods share.
