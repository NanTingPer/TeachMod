using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using TeachMod.Extensions;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Arithmetic.Te;

public class Typewriter : TeachModSystem
{
    public readonly static StringBuilder drawString = new StringBuilder("Hello World!");
    private int timer = 0;
    private bool IsViewShu = true;
    public static int count = 0;
    public override void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        spriteBatch.GraphicsDevice.Clear(Color.Black);
        timer++;
        int length = drawString.Length;                                                 //文本长度
        var DrawPos = new Vector2(100, 100);
        var viewCount = timer / 10 % (length + 1);                          //显示数量
        string draw = drawString.ToString()[0 ..viewCount];                      //真实显示的文本

        #region 最后的 | 符号
        if (IsViewShu) draw += "I";
        if (timer % 30 == 0) IsViewShu = false;
        if (timer % 60 == 0) IsViewShu = true;
        #endregion
        spriteBatch.SafeBegin();
        ReLogic.Graphics.DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.MouseText.Value, draw, DrawPos, Color.Red, 0f, Vector2.Zero, 5f, SpriteEffects.None, 1f);
        spriteBatch.SafeEnd();
        base.PostDoDraw(spriteBatch, main, gameTime);
    }
}
