using System;
using System.Reflection;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Items;

[Autoload]
public abstract class SummonItem<TProjectile, TBuff> : ModItem
    where TBuff : ModBuff
    where TProjectile : ModProjectile
{
    static SummonItem()
    {
        var tProjectileType = typeof(TProjectile);
        var tBuffType = typeof(TBuff);
        var thisType = typeof(SummonItem<TProjectile, TBuff>);
        var BFIP = BindingFlags.Instance | BindingFlags.Public;

        #region This
        var itemSetDefaults = thisType
            .GetMethod(nameof(ModItem.SetDefaults), BFIP);
        MonoModHooks.Add(itemSetDefaults, ItemSetDefaults);

        var itemUseItem = thisType
            .GetMethod(nameof(ModItem.UseItem), BFIP);
        MonoModHooks.Add(itemUseItem, ItemUseItem);
        #endregion

        #region Projectile
        var projectilePreAI = tProjectileType
            .GetMethod(nameof(ModProjectile.PreAI), BFIP);
        MonoModHooks.Add(projectilePreAI, ModProjectilePreAI);

        var projectileSetDefault = tProjectileType
            .GetMethod(nameof(ModProjectile.SetDefaults), BFIP);
        MonoModHooks.Add(projectileSetDefault, ModProjectilSetDefault);
        #endregion

        #region Buff
        var buffSetDefault = tBuffType
            .GetMethod(nameof(ModBuff.SetStaticDefaults), BFIP);
        MonoModHooks.Add(buffSetDefault, ModBuffSetStaticDefaults);
        #endregion
    }

    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;

    public static float MinionSlots { get; set; } = 1f;

    #region ThisHookMethod
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
        orig.Invoke(modProjectile);
        var projectile = modProjectile.Projectile;
        projectile.minionSlots = MinionSlots;
        Main.projPet[modProjectile.Type] = true;
    }

    private static bool ModProjectilePreAI(Func<ModProjectile, bool> orig, ModProjectile modProjectile)
    {
        Projectile projectile = modProjectile.Projectile;
        var player = Main.player[projectile.owner];
        //不存在buff
        if (!player.HasBuff<TBuff>()) {
            modProjectile.OnKill(projectile.timeLeft);
            projectile.active = false;
            return false;
        } else {
            //player.AddBuff(ModContent.BuffType<TBuff>(), 999);
            return orig.Invoke(modProjectile);
        }
    }
    #endregion

    #region Buff
    private static void ModBuffSetStaticDefaults(Action<ModBuff> orig, ModBuff buff)
    {
        orig.Invoke(buff);
        Main.debuff[buff.Type] = false;
        Main.buffNoTimeDisplay[buff.Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[buff.Type] = true;
    }
    #endregion
}

#pragma warning disable CA2255
//public static class SummonItemUpHook
//{
//[ModuleInitializer]
//public static void Hooks()
//{
//    var summaryGenericTypes = AssemblyManager
//        .GetLoadableTypes(typeof(SummonItemUpHook).Assembly)
//        .Where(type => type.BaseType != null && type.BaseType.GetGenericTypeDefinition() == typeof(SummonItem<,>))
//        .Select(type => type.GetGenericParameterConstraints())
//        ;
//
//    var projectileTypes = summaryGenericTypes.Select(f => f[0]);
//    var buffTypes = summaryGenericTypes.Select(f => f[1]);
//    var BFIP = BindingFlags.Instance | BindingFlags.Public;
//    foreach (var projType in projectileTypes) {
//        var projectilePreAI = projType.GetMethod("PreAI", BFIP);
//        MonoModHooks.Add(projectilePreAI, ModProjectilePreAI);
//    }
//
//    foreach (var buffType in buffTypes) {
//        var buffSetDefault = buffType.GetMethod("SetStaticDefaults", BFIP);
//        MonoModHooks.Add(buffSetDefault, ModBuffSetStaticDefaults);
//    }
//}

//private static bool ModProjectilePreAI(Func<ModProjectile, bool> orig, ModProjectile modProjectile)
//{
//    Projectile projectile = modProjectile.Projectile;
//    var player = Main.player[projectile.owner];
//
//    //不存在buff
//    if (!player.HasBuff<TBuff>()) {
//        modProjectile.OnKill(projectile.timeLeft);
//        projectile.active = false;
//        return false;
//    } else {
//        player.AddBuff(ModContent.BuffType<TBuff>(), 999);
//        return orig.Invoke(modProjectile);
//    }
//}
//private static void ModBuffSetStaticDefaults(Action<ModBuff> orig, ModBuff buff)
//{
//    orig.Invoke(buff);
//    Main.debuff[buff.Type] = false;
//    BuffID.Sets.TimeLeftDoesNotDecrease[buff.Type] = true;
//}

//}