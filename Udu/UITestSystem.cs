using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace TeachMod.Udu;

public class UITestSystem : ModSystem
{
    public override void Load()
    {
        //var testUIElement = new UIElement()
        //{
        //    Active = true,
        //    Height = 100,
        //    Width = 100 
        //};

        //testUIElement.DrawSelfPost += TestUIElement_DrawSelfPost;

        base.Load();
    }

    private void TestUIElement_DrawSelfPost(UIElement arg1, SpriteBatch arg2)
    {
        //var curt2d = arg2.GraphicsDevice.GetRenderTargets();
        //arg2.End();
        //var texture = Textures.Textures.Cusp;
        //var rt2d = new RenderTarget2D(arg2.GraphicsDevice, 100, 100);
        //arg2.GraphicsDevice.SetRenderTarget(rt2d); //设置rt2d
        //arg2.Begin(Effects.Bloom.Value);                              //往rt2d画东西
        //arg2.Draw(texture.Value, Vector2.Zero, Color.White);          //往rt2d画东西
        //arg2.End();
        ////arg2.GraphicsDevice.SetRenderTarget(curt2d[0].RenderTarget as RenderTarget2D);
        //arg2.Begin(null, null);
        //arg2.Draw(rt2d, Vector2.Zero, Color.White);
        //rt2d.Dispose();
    }

    #region UITestSystem => 251103之前
    //public static Asset<Texture2D> Clp;
    //public static Asset<Effect> AlphaTestEffect;

    //private Vector2 mouseOffset;
    //public override void Load()
    //{
    //    Clp = ModContent.Request<Texture2D>("TeachMod/Udu/School/clp", AssetRequestMode.ImmediateLoad);
    //    AlphaTestEffect = ModContent.Request<Effect>("TeachMod/Graphics/Content/AlphaTestEffect", AssetRequestMode.ImmediateLoad);

    //    var eeent = new EntityElement()
    //    {
    //        Height = Main.screenHeight,
    //        Width = Main.screenWidth,
    //        IsPanle = true,
    //        Active = true
    //    };
    //    eeent.Append(new TestNode());

    //    #region 拖动示例
    //    //elementUpIndexTestUI.MouseHover += (a) => {
    //    //    UIElement @this = a.Element;
    //    //    if (Main.mouseLeft && Main.mouseLeftRelease) {
    //    //        mouseOffset = @this.MouseOffset;
    //    //    } else if (Main.mouseLeft) {
    //    //        //先计算当前鼠标位置与原点位置的差
    //    //        float elSubMouseY = Main.MouseScreen.Y - mouseOffset.Y;
    //    //        float elSubMouseX = Main.MouseScreen.X - mouseOffset.X;
    //    //
    //    //        a.Element.TopPadding = elSubMouseY;
    //    //        a.Element.LeftPadding = elSubMouseX;
    //    //    } else if (Main.mouseLeftRelease) {
    //    //        mouseOffset = Vector2.Zero;
    //    //    }
    //    //};
    //    #endregion
    //    base.Load();
    //}

    //public static IfMaxSubElseAdd alphavalue = new IfMaxSubElseAdd(0.1f, 1f, 0f, 0.05f);
    //public static void DrawPost(UIElement uielement, SpriteBatch spriteBatch)
    //{
    //    var spriteWidthAndHeight = new Vector2(Clp.Width(), Clp.Height());
    //    var screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2;
    //    //绘制位置
    //    var drawPosition = screenCenter - (spriteWidthAndHeight / 2);

    //    spriteBatch.End();

    //    //////////////////////////
    //    var alphaTest = AlphaTestEffect.Value;
    //    alphaTest.Parameters["alpha"].SetValue(alphavalue.Run());
    //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, alphaTest, Main.GameViewMatrix.ZoomMatrix);
    //    spriteBatch.Draw(Clp.Value, drawPosition, null, Color.Aqua, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    //    //////////////////////////

    //    spriteBatch.End();
    //    spriteBatch.Begin();
    //}
    #endregion
}