#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace TeachMod.Udu;

public class IamgeUI(Asset<Texture2D> asset) : UIElement
{
    /// <summary>
    /// 此UI绘制的图片
    /// </summary>
    public Asset<Texture2D> Iamge { get; set; } = asset;

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Iamge.Value, DrawRectangle, Color.White);
    }
}
