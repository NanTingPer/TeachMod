using Terraria;
using Terraria.ModLoader;

namespace TeachMod.Udu;

public class UITestSystem : ModSystem
{
    public override void Load()
    {
        var element = new UIElement()
        {
            Active = true,
            Height = 200,
            Width = 200,
        };
        element.MouseHover += (a) => {
            Mod.Logger.Debug("在范围内");
        };
        base.Load();
    }
}
