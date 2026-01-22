using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeachMod.Extensions;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Arithmetic;

internal class ArithmeticUtils
{
    public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color colorStart, Color colorEnd, float width)
    {
        spriteBatch.SafeBegin(null, null); //aaa
        float num = Vector2.Distance(start, end);
        Vector2 vector = (end - start) / num;
        Vector2 vector2 = start;
        float rotation = vector.ToRotation();
        float scale = width / 16f;
        for (float num2 = 0f; num2 <= num; num2 += width) {
            float amount = num2 / num;
            spriteBatch.Draw(TextureAssets.BlackTile.Value, vector2, null, Color.Lerp(colorStart, colorEnd, amount), rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
            vector2 = start + num2 * vector;
        }
    }
}
