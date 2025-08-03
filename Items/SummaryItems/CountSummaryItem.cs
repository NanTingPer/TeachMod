using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Items.SummaryItems;

public class CountSummaryItem : SummonItem<CountSummaryProjectile, CountSummaryBuff>
{
    public override void SetDefaults()
    {
        Item.damage = 20;
        base.SetDefaults();
    }
}

public class CountSummaryProjectile : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;

    private NPC tarNPC;
    private List<Action> preAIExter = [];
    private Player player => Main.player[Projectile.owner];
    public int count = 1;

    public override void SetDefaults()
    {
        Projectile.height = 20;
        Projectile.width = 20;
        Projectile.damage = 20;
        Projectile.DamageType = DamageClass.Summon;
        base.SetDefaults();
    }

    public override bool PreAI()
    {
        preAIExter.ForEach(ac => ac.Invoke());
        preAIExter.Clear();

        Main.NewText(count);
        Projectile.damage = count * 20;
        //Projectile.minionSlots = count * 1f;
        return base.PreAI();
    }

    public override void AI()
    {
        if(tarNPC == null || tarNPC.active == false) {
            Projectile.position = Main.player[Projectile.owner].position + new Vector2(0, -100);
            tarNPC = Main.ActiveNPCs
                .AsIEnumerable()
                .Where(f => f.friendly == false)
                .MinBy(f => f.Distance(player.position));
        } else {
            Projectile.position = tarNPC.position;
        }
        base.AI();
    }

    public override void OnSpawn(IEntitySource source)
    {
        preAIExter.Add(() => {
            var ttype = Main
                .ActiveProjectiles
                .Where(f =>
                    f.ModProjectile != null &&
                    f.ModProjectile.GetType() == GetType());
            if (ttype.Count() != 1) {
                var tarProj = ttype.FirstOrDefault();
                if (tarProj != null) {
                    var proj = (CountSummaryProjectile)tarProj.ModProjectile;
                    proj.count += 1;
                    Projectile.active = false;
                    //Projectile.Kill();
                }
            }
        });
        base.OnSpawn(source);
    }
}

public class CountSummaryBuff : ModBuff
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;
}