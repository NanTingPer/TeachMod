using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Globals;

public class GetProjectileAI : GlobalProjectile
{
    public override void AI(Projectile projectile)
    {
        base.AI(projectile);
#if DEBUG
        if(projectile.type == ProjectileID.EmpressBlade) {
            Logging.PublicLogger.Debug("是我要的弹幕执行了AI！" + "  AIStyle: " + projectile.aiStyle);
            projectile.VanillaAI();
        }
#endif
    }
}
