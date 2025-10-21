using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Teachs.Items;

public abstract class SummonItem<TProjectile, TBuff, TSelf> : ModItem
    where TBuff : ModBuff
    where TProjectile : ModProjectile
    where TSelf : SummonItem<TProjectile, TBuff, TSelf>
{
    static SummonItem()
    {
        var tProjectileType = typeof(TProjectile);
        var tBuffType = typeof(TBuff);
        var thisType = typeof(TSelf);
        var BFIP = BindingFlags.Instance | BindingFlags.Public;

        #region Get This Method
        var itemSetDefaults = thisType
            .GetMethod(nameof(SetDefaults), BFIP);

        MonoModHooks.Add(itemSetDefaults, ItemSetDefaults);

        var itemUseItem = thisType
            .GetMethod(nameof(UseItem), BFIP);
        MonoModHooks.Add(itemUseItem, ItemUseItem);
        #endregion

        #region Get Projectile Method
        var projectilePreAI = tProjectileType
            .GetMethod(nameof(ModProjectile.PreAI), BFIP);
        MonoModHooks.Add(projectilePreAI, ModProjectilePreAI);

        var projectileSetDefault = tProjectileType
            .GetMethod(nameof(ModProjectile.SetDefaults), BFIP);
        MonoModHooks.Add(projectileSetDefault, ModProjectilSetDefault);
        #endregion

        #region Get Buff Method
        var buffSetDefault = tBuffType
            .GetMethod(nameof(ModBuff.SetStaticDefaults), BFIP);
        MonoModHooks.Add(buffSetDefault, ModBuffSetStaticDefaults);
        #endregion
    }
    public static float MinionSlots { get; set; } = 1f;

    #region This
    private static bool? ItemUseItem(Func<ModItem, Player, bool?> orig, ModItem modItem, Player player)
    {
        if (!player.HasBuff<TBuff>()) {
            player.AddBuff(ModContent.BuffType<TBuff>(), 99);
        }
        return orig.Invoke(modItem, player);
    }

    private static void ItemSetDefaults(Action<ModItem> orig, ModItem modItem)
    {
        var Item = modItem.Item;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.autoReuse = false;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = ItemUseStyleID.Swing;

        Item.shoot = ModContent.ProjectileType<TProjectile>();
        ItemID.Sets.StaffMinionSlotsRequired[modItem.Type] = MinionSlots;
        orig.Invoke(modItem);
    }
    #endregion

    #region Projectile
    private static void ModProjectilSetDefault(Action<ModProjectile> orig, ModProjectile modProjectile)
    {
        var projectile = modProjectile.Projectile;
        projectile.minionSlots = MinionSlots;
        projectile.minion = true;
        projectile.friendly = true;
        projectile.ContinuouslyUpdateDamageStats = true; //基于弹幕伤害进行伤害计算
        projectile.penetrate = -1;
        projectile.tileCollide = false;

        ProjectileID.Sets.MinionSacrificable[modProjectile.Type] = true; //牺牲
        Main.projPet[modProjectile.Type] = true;
        orig.Invoke(modProjectile);
    }

    private static bool ModProjectilePreAI(Func<ModProjectile, bool> orig, ModProjectile modProjectile)
    {
        Projectile projectile = modProjectile.Projectile;
        var player = Main.player[projectile.owner];
        if (!player.HasBuff<TBuff>()) {
            projectile.active = false;
            return false;
        } else {
            return orig?.Invoke(modProjectile) ?? false;
        }
    }
    #endregion

    #region Buff
    private static void ModBuffSetStaticDefaults(Action<ModBuff> orig, ModBuff buff)
    {
        Main.debuff[buff.Type] = false;
        Main.buffNoTimeDisplay[buff.Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[buff.Type] = true;
        orig.Invoke(buff);
    }
    #endregion
}