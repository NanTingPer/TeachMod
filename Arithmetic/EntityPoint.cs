using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TeachMod.Extensions;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Arithmetic;

/// <summary>
/// 用于绘制的实体点
/// </summary>
public class EntityPoint
{
    public static EntityPoint RandmoEntityPoint(int maxX, int maxY)
    {
        int x = Random.Shared.Next(0, maxX);
        int y = Random.Shared.Next(0, maxY);
        return new EntityPoint(){ position = new Vector2(x,y) };
    }
    public EntityPoint()
    {
        Main.QueueMainThreadAction(() => texture2d = Texture2D.CreateTexture2D(Width, Height, Color.White));
    }
    private bool reCreateTexture2D = false;
    public float scale = 1f;
    public int Width
    {
        get => field;
        set
        {
            field = value;
            if (value != field) {
                reCreateTexture2D = true;
            }
        }
    } = 20;
    public int Height 
    { 
        get => field; 
        set 
        { 
            field = value;
            if(value != field) {
                reCreateTexture2D = true;
            }
        } 
    } = 20;

    public Vector2 velocity = Vector2.Zero;
    public Vector2 position;
    public Color color = Color.White;
    private Texture2D texture2d;

    public void Update(Main main, ref GameTime gametime)
    {
        position += velocity;
        //position.X = position.X > Main.screenWidth ? Main.screenWidth : position.X;
        //position.Y = position.Y > Main.screenHeight ? Main.screenHeight : position.Y;
    }

    /// <summary>
    /// 在屏幕上绘制此实体点
    /// </summary>
    /// <param name="spriteBatch"> 画笔 </param>
    /// <param name="main"> main </param>
    /// <param name="gameTime"> gameTime </param>
    public void Draw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        if(texture2d == null) return;
        if (reCreateTexture2D) {
            texture2d.Dispose();
            texture2d = Texture2D.CreateTexture2D(Width, Height, Color.White);
        }
        spriteBatch.SafeBegin(null, null);
        spriteBatch.Draw(texture2d, position, color);
        ReLogic.Graphics.DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, FontAssets.MouseText.Value, position.ToString(), new Vector2(position.X, position.Y + (Width * scale)), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
        spriteBatch.End();
    }

    public float X => position.X;
    public float Y => position.Y;

    public static implicit operator Vector2(EntityPoint ep) => ep.position;
    public static bool operator ==(EntityPoint ep1, EntityPoint ep2) => ep1.position == ep2.position;
    public static bool operator !=(EntityPoint ep1, EntityPoint ep2) => ep1.position != ep2.position;
    public override string ToString()
    {
        return position.ToString();
    }
    public override bool Equals(object obj)
    {
        return (obj as EntityPoint) == this;
    }
}
