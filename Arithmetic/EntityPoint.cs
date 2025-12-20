using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeachMod.Extensions;
using Terraria;

namespace TeachMod.Arithmetic;

public class EntityPoint
{
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

    public void Draw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        if(texture2d == null) return;
        if (reCreateTexture2D) {
            texture2d.Dispose();
            texture2d = Texture2D.CreateTexture2D(Width, Height, Color.White);
        }
        spriteBatch.Begin(null, null);
        spriteBatch.Draw(texture2d, position, color);
        spriteBatch.End();
    }

    public float X => position.X;
    public float Y => position.Y;

    public static implicit operator Vector2(EntityPoint ep) => ep.position;
}
