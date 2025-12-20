using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Teachs.Projectiles;

public class CopyAIStyle : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;

    public override void SetDefaults()
    {
        Projectile.aiStyle = ProjAIStyleID.Terraprisma; //AI样式
        AIType = ProjectileID.EmpressBlade; //对应弹幕
        Projectile.minion = true;
        base.SetDefaults();
    }

    public override void AI()
    {
        Projectile.timeLeft = 66;
        Main.NewText(Projectile.velocity + "        " + Projectile.position);
        var player = Main.player[Projectile.owner];
        if (Projectile.Distance(player.position) > 1000) {
            Projectile.position = player.position;
        }
        base.AI();
    }

    public override bool PreKill(int timeLeft)
    {
        return false;
    }
}
