using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TeachMod.UtilityType;
using Terraria;
using Terraria.ModLoader;

namespace TeachMod.Udu;

public class UITestSystem : ModSystem
{
    public static Asset<Texture2D> Clp;
    public static Asset<Effect> AlphaTestEffect;

    private Vector2 mouseOffset;
    public override void Load()
    {
        Clp = ModContent.Request<Texture2D>("TeachMod/Udu/School/clp", AssetRequestMode.ImmediateLoad);
        AlphaTestEffect = ModContent.Request<Effect>("TeachMod/Effect/Content/AlphaTestEffect", AssetRequestMode.ImmediateLoad);
        var elementUpIndexTestUI = new UIElement()
        {
            Active = true,
            Height = 400,
            Width = 300,
            IsPanle = true
        };
        elementUpIndexTestUI.MouseHover += (a) => {
            UIElement @this = a.Element;
            if (Main.mouseLeft && Main.mouseLeftRelease) {
                mouseOffset = @this.MouseOffset;
            } else if (Main.mouseLeft) {
                //先计算当前鼠标位置与原点位置的差
                float elSubMouseY = Main.MouseScreen.Y - mouseOffset.Y;
                float elSubMouseX = Main.MouseScreen.X - mouseOffset.X;

                a.Element.TopPadding = elSubMouseY;
                a.Element.LeftPadding = elSubMouseX;
            }
        };
        
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
            Name = "ListUI",
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
            //ar.Entitys.Parent.Active = false;
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

        //_ = Main.screenHeight;
        //
        //var uiel = new UIElement()
        //{
        //    Width = Main.screenWidth,
        //    Height = Main.screenHeight,
        //    IsPanle = false,
        //    drawSelfPost = DrawPost,
        //    Active = true
        //};
        base.Load();
    }

    public static IfMaxSubElseAdd alphavalue = new IfMaxSubElseAdd(0.1f, 1f, 0f, 0.05f);
    public static void DrawPost(UIElement uielement, SpriteBatch spriteBatch)
    {
        var spriteWidthAndHeight = new Vector2(Clp.Width(), Clp.Height());
        var screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2;
        //绘制位置
        var drawPosition = screenCenter - (spriteWidthAndHeight / 2);

        spriteBatch.End();

        //////////////////////////
        var alphaTest = AlphaTestEffect.Value;
        alphaTest.Parameters["alpha"].SetValue(alphavalue.Run());
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, alphaTest, Main.GameViewMatrix.ZoomMatrix);
        spriteBatch.Draw(Clp.Value, drawPosition, null, Color.Aqua, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        //////////////////////////

        spriteBatch.End();
        spriteBatch.Begin();
    }
}
