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
            IsPanle = true
        };

        var listUi = new ListUI<string>()
        {
            ItemHeight = 50,
            ItemWidth = 60,
            Active = true
        };
        listUi
            .Append("第一个元素")
            .Append("第二个元素")
            .Append("第三个元素")
            .Append("第四个元素")
            .Append("第五个元素")
            .Append("第六个元素")
            ;

        element.Append(listUi);
        base.Load();
    }
}
