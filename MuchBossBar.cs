using Microsoft.Xna.Framework;
using System.Linq;
using TeachMod.Teachs;
using Terraria;

namespace TeachMod;

public class MuchBossBar : TeachModSystem
{
    public override void PostUpdate(Main mian, ref GameTime gameTime)
    {
        if(Main.player.Length == 0)
            return;

        var boss = Main.ActiveNPCs.Where(n => n.boss).ToList();
        var bosslife = boss.Select(b => b.life).ToList();
    }
}
