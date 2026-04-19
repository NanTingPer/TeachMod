using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using TeachMod.Extensions;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Arithmetic.Te;

/// <summary>
/// 打字机 非协程
/// </summary>
public class Typewriter : TeachModSystem
{
    private static bool IsDraw = false;
    public readonly static StringBuilder drawString = new StringBuilder("Hello World!");
    private int timer = 0;
    private Vector2 drawPos = new Vector2(100, 100);
    public override void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        if(!IsDraw) return;

        spriteBatch.GraphicsDevice.Clear(Color.Black);
        timer++;
        int length = drawString.Length;
        var viewCount = timer / 10 % (length + 1);                          //显示数量
        string draw = drawString.ToString()[0 ..viewCount];                      //真实显示的文本

        spriteBatch.SafeBegin();
        ReLogic.Graphics.DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.MouseText.Value, draw, drawPos, Color.Red, 0f, Vector2.Zero, 5f, SpriteEffects.None, 1f);
        spriteBatch.SafeEnd();
        base.PostDoDraw(spriteBatch, main, gameTime);
    }
}
