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
        base.Load();
    }
}
