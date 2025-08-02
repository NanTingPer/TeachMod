#pragma warning disable CA2255
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using Terraria.UI;

namespace TeachMod.Issues;

public class NoNPCSpawn
{
    internal static Func<Item, bool> FavoritedItem = f => {
        return f.createTile != -1
               && Item.BannerToNPC(f.type) != NPCID.None
               && f.favorited == true;
    };

    #region 逻辑钩子
    internal delegate int NewNPCdelegate(IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255);
    private static int NewNPCHookMethod(NewNPCdelegate orig, IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255)
    {
        if (Main.netMode == NetmodeID.SinglePlayer ||
           Main.netMode == NetmodeID.Server) {
            var noSpawnNpcTypes = new HashSet<int>();
            foreach (var pl in Main.ActivePlayers) {
                _ = pl.inventory
                    .Where(FavoritedItem)//ModBannerTile
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
    internal static void OnHookMethod()
    {
        var method = typeof(NPC).GetMethod(nameof(NPC.NewNPC), BindingFlags.Public | BindingFlags.Static);
        MonoModHooks.Add(method, NewNPCHookMethod);
    }
    #endregion

    #region 钩子
    [ModuleInitializer]
    internal static void ILHookItemSlot()
    {
        On_ItemSlot.OverrideLeftClick += On_ItemSlot_OverrideLeftClick; ;
    }

    private static bool On_ItemSlot_OverrideLeftClick(On_ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot)
    {
        var oldFavoritedStatu = inv[slot].favorited;
        var retValue = orig.Invoke(inv, context, slot);
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            var newFavoritedStatu = inv[slot].favorited;
            if (oldFavoritedStatu != newFavoritedStatu) {
                var packet = ModLoader.GetMod(nameof(TeachMod)).GetPacket();
                packet.Write(slot);
                packet.Write(newFavoritedStatu);
                packet.Send();
            }
        }
        return retValue;
    }
    #endregion
}

/// <summary>
/// 仅是为了联机同步，这样才能让服务器初始化旗帜，不然已经被收藏的，服务器不能同步
/// </summary>
public class NoNPCSpawnPlayer : ModPlayer
{
    private volatile List<Action> updateExter = [];

    public override void PostUpdate()
    {
        foreach (var action in updateExter) {
            action.Invoke();
        }
        updateExter.Clear();
        base.PostUpdate();
    }

    public override void OnEnterWorld()
    {
        updateExter.Add(SendItemFavorited);
        base.OnEnterWorld();
    }
    private void SendItemFavorited()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            var packet = ModLoader.GetMod(nameof(NoNPCSpawn)).GetPacket();
            List<int> items = [];
            for (int i = 0; i < Player.inventory.Length; i++) {
                if (NoNPCSpawn.FavoritedItem(Player.inventory[i])) {
                    items.Add(i);
                }
            }
            items.ForEach(f => {
                packet.Write(f);
                packet.Write(true);
                packet.Send();
            });
        }
    }
}
