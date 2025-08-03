using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Items.SummaryItems;

public class CountSummaryItem : SummonItem<CountSummaryProjectile, CountSummaryBuff>
{
}

public class CountSummaryProjectile : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;

    public override void AI()
    {
        Projectile.position = Main.player[Projectile.owner].position + new Microsoft.Xna.Framework.Vector2(200, -200);
        base.AI();
    }
}

public class CountSummaryBuff : ModBuff
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;
}