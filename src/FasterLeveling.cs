using LibRecomp.Mods;
using RecompiledFuncs;

public sealed class FasterLeveling : IMod
{
    public void Load(Mod mod)
    {
        // AddExpPoints(pokemon, target, exp) gives the target exp points, so scale the third argument
        mod.Hook(ref Funcs.Patches.AddExpPoints, ctx =>
        {
            ctx.R2 = (uint)((int)ctx.R2 * (int)mod.Config.GetNumber("multiplier"));
        });
    }
}
