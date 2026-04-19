using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Text;
using TeachMod.Extensions;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Arithmetic.Te;

public class CoroutineTypewriter : TeachModSystem
{
    private 携程管理 coroutine;
    public readonly static bool IsDraw = true;
    public readonly static StringBuilder drawString = new StringBuilder("Hello World!");
    public string drawText = "";
    public Vector2 drawPos = new Vector2(100, 100);

    public override void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        if(!IsDraw) return;
        coroutine ??= 协程.Add(UpdateDrawText, true);
        spriteBatch.GraphicsDevice.Clear(Color.Black);
        spriteBatch.SafeBegin();
        ReLogic.Graphics.DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.MouseText.Value, drawText, drawPos, Color.Red, 0f, Vector2.Zero, 5f, SpriteEffects.None, 1f);
        spriteBatch.SafeEnd();

    }

    private IEnumerator UpdateDrawText()
    {
        int count = 0;
        while(count < drawString.Length) {
            drawText = drawString.ToString()[0..count];
            count++;
            yield return new AwaitFrames() { Frames = 10 };
        }
        yield break;
    }
}
