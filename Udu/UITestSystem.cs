using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            //Mod.Logger.Debug("在范围内");
        };
        element.MouseClick += (a) => {
            Main.NewText("点击了父集");
        };

        var element2 = new UIElement()
        {
            Name = "子",
            Active = true,
            Height = 100,
            Width = 100,
            DrawSelfAction = (ui, sb) => {
                sb.Draw(ui.Texture, ui.DrawRectangle, Color.White);
            },
        };
        element2.MouseClick += (a) => {
            Main.NewText("点击了子集");
        };


        element.Append(element2);
        base.Load();
    }
}
