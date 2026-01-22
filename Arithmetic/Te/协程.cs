using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;

namespace TeachMod.Arithmetic.Te;

public class 协程 : TeachModSystem
{
    public static List<IEnumerator> enumerators = [];
    public static void Add(IEnumerator enumerator)
    {
        enumerators.Add(enumerator);
    }
    public static void Add(Func<IEnumerator> func)
    {
        enumerators.Add(func());
    }
    public override void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        foreach (var item in enumerators) {
            item.MoveNext();
        }
        base.PostDoDraw(spriteBatch, main, gameTime);
    }
}

public class 协程模型
{

}