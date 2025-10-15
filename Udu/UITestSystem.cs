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
            Width = 100,
            Active = true,
            drawSelfPost = (u, sb) => {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                sb.Draw(u.Texture, new Rectangle(u.MouseRectangle.X, u.MouseRectangle.Y, 10, 10), Color.Aqua);
                sb.End();
                sb.Begin();
            }
        };

        listUi.ItemClickEvent += (ar) => {
            Mod.Logger.Debug($"Click : {ar.CuEntity}");
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
