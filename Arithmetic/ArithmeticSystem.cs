using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace TeachMod.Arithmetic;

public class ArithmeticSystem : TeachModSystem
{
    public static bool drawArithmentic = true;
    public static List<EntityPoint> pointList =[];

    public ArithmeticSystem()
    {
    }

    public override void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        if(drawArithmentic == false) return;
        foreach (var item in pointList) {
            item.velocity = Vector2.One;
            //item.position = new Vector2(200, 200);
            item.Draw(spriteBatch, main, gameTime);
        }
    }

    public override void PreUpdate(Main main, ref GameTime gametime)
    {
        if (drawArithmentic == false) return;
        foreach (var item in pointList) {
            item.Update(main, ref gametime);
        }
    }

    public static void DrawLine(SpriteBatch spriteBatch, List<EntityPoint> points, Color color)
    {
        for (int i = 0; i < points.Count - 1; i++) {
            Utils.DrawLine(spriteBatch, points[i].position, points[i + 1].position, color);
        }
    }
}
