#pragma warning disable CA2255
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace TeachMod.Issues;

/// <summary>
/// 当NPC对应的旗帜被收藏后，此NPC的生成被禁止
/// </summary>
public class NoNPCSpawn
{
    internal delegate int NewNPCdelegate(IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255);
    private static int NewNPCHookMethod(NewNPCdelegate orig, IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255)
    {
        if(Main.netMode == NetmodeID.SinglePlayer ||
           Main.netMode == NetmodeID.Server) {
            var noSpawnNpcTypes = new HashSet<int>();
            foreach (var pl in Main.ActivePlayers) {
                _ = pl.inventory
                    .Where(f =>
                                  f.createTile != -1
                               && Item.BannerToNPC(f.type) != NPCID.None
                               && f.favorited == true)//ModBannerTile
                    .Select(f => noSpawnNpcTypes.Add(f.type))
                    .ToArray()
                    ;
            }
                //NPCLoader.GetNPC(Type).NPC.BannerID())
            if (noSpawnNpcTypes.Contains(Item.BannerToItem(Item.NPCtoBanner(Type)))) {
                return Main.npc.Length - 1;
            }
        }
        return orig.Invoke(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
    }

    [ModuleInitializer]
    internal static void NewNPCHook()
    {
        var method = typeof(NPC).GetMethod(nameof(NPC.NewNPC), BindingFlags.Public | BindingFlags.Static);
        MonoModHooks.Add(method, NewNPCHookMethod);
    }
}
