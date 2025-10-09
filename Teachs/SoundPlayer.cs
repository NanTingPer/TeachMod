using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Utilities;
using System.Threading.Tasks;

namespace TeachMod.Teachs;

public class SoundPlayer : ModPlayer
{
    //public static SlotId slotid;
    public override async void OnEnterWorld()
    {
        //var ss = new SoundStyle("TeachMod/hsgj");
        //ss.MaxInstances = 999999999;
        //ss.Type = SoundType.Music;
        //ss.Volume = 9999999;
        //await Task.Delay(1000);
        //slotid = SoundEngine.PlaySound(ss);
        base.OnEnterWorld();
    }

    public override void PreUpdate()
    {
        //if(SoundEngine.TryGetActiveSound(slotid, out var acs)) {
        //    acs.Position = Main.MouseWorld;
        //}
        base.PreUpdate();
    }
}
