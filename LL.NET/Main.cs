using LiteLoader.Hook;
using LiteLoader.NET;
using MC;
using System;
using System.Collections.Generic;

namespace BlockIsland;
[PluginMain("BlockIsland")]
public class BlockIsland : IPluginInitializer
{
    public string Introduction => "刷矿机 - 用于方屿";
    public Dictionary<string, string> MetaData => new();
    public Version Version => new(1, 0, 0);
    internal static readonly Dictionary<string, int> data = new()
    {
        ["minecraft:cobblestone"] = 100,
        ["minecraft:coal_ore"] = 15,
        ["minecraft:copper_ore"] = 13,
        ["minecraft:iron_ore"] = 11,
        ["minecraft:gold_ore"] = 9,
        ["minecraft:redstone_ore"] = 7,
        ["minecraft:lapis_ore"] = 5,
        ["minecraft:emerald_ore"] = 3,
        ["minecraft:diamond_ore"] = 1
    };
    internal static int sum = 0;
    public void OnInitialize()
    {
        foreach (int weight in data.Values)
        {
            sum += weight;
        }
        Thook.RegisterHook<OnSolidifyHook, OnSolidifyHookDelegate>();
        Thook.RegisterHook<TrySpawnPortalHook, TrySpawnPortalHookDelegate>();
    }
}

internal delegate void OnSolidifyHookDelegate(nint @this, nint a2, BlockPos a3, BlockPos a4);
[HookSymbol("?solidify@LiquidBlock@@IEBAXAEAVBlockSource@@AEBVBlockPos@@1@Z")]
internal class OnSolidifyHook : THookBase<OnSolidifyHookDelegate>
{
    public override OnSolidifyHookDelegate Hook =>
        (@this, a2, a3, a4) =>
        {
            Original(@this, a2, a3, a4);
            BlockInstance bi = new BlockSource(a2).GetBlockInstance(a3);
            if (bi.Block.TypeName is "minecraft:stone" or "minecraft:cobblestone")
            {
                int randInt = Random.Shared.Next(BlockIsland.sum);
                foreach ((string block, int weight) in BlockIsland.data)
                {
                    if (weight > randInt)
                    {
                        _ = Level.SetBlock(a3, bi.DimensionId, block, 0);
                    }
                }
            }
        };
}

internal delegate bool TrySpawnPortalHookDelegate(nint a1, nint a2);
[HookSymbol("?trySpawnPortal@PortalBlock@@SA_NAEAVBlockSource@@AEBVBlockPos@@@Z")]
internal class TrySpawnPortalHook : THookBase<TrySpawnPortalHookDelegate>
{
    public override TrySpawnPortalHookDelegate Hook => (a1, a2) => false;
}
