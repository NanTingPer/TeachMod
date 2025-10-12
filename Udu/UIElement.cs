using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace TeachMod.Udu;
public class UIMouseEventArgs(UIElement orig, Vector2 mousePosition)
{
    public UIElement Element { get; init; } = orig;
    public Vector2 MousePosition { get; init; } = mousePosition;
}

public class UIElement
{
    public UIElement()
    {
        UIElementLoader.Region(this);
    }
    public delegate void UIMouseEvent(UIMouseEventArgs eventObj);
    private static Texture2D DefaultTexture;
    static UIElement()
    {
        Main.QueueMainThreadAction(() => {
            DefaultTexture = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
            DefaultTexture.SetData([new Color(255, 255, 255)]);
        });
    }
    public Texture2D Texture { get; set; } = DefaultTexture;
    /// <summary>
    /// 鼠标悬浮触发
    /// </summary>
    public event UIMouseEvent MouseHover;
    /// <summary>
    /// 鼠标单击触发
    /// </summary>
    public event UIMouseEvent MouseClick;
    /// <summary>
    /// 决定此UI是否有效
    /// </summary>
    public bool Active { get; set; } = false;
    public bool IsAbsLocate { get; set; } = false;
    public bool IsPanle { get; set; } = true;
    private readonly List<UIElement> elements = [];
    /// <summary>
    /// 宽度
    /// </summary>
    public virtual float Height { get; set; }
    /// <summary>
    /// 高度
    /// </summary>
    public virtual float Width { get; set; }
    /// <summary>
    /// 相对于父容器的X偏移
    /// </summary>
    public virtual float XOffset { get; set; }
    /// <summary>
    /// 相对于父容器的Y偏移
    /// </summary>
    public virtual float YOffset { get; set; }
    private Rectangle DrawRectangle => new Rectangle((int)XOffset, (int)YOffset, (int)Width, (int)Height);

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        DrawSelf(spriteBatch);
        spriteBatch.End();
    }

    public virtual void DrawSelf(SpriteBatch spriteBatch)
    {
        if (IsPanle) {
            spriteBatch.Draw(Texture, DrawRectangle, Color.Black);
        }

        foreach (var element in elements) {
            element.Draw(spriteBatch);
        }
        Main.instance.DrawMouseOver();
    }

    public void Disposable()
    {
        UIElementLoader.Remove(this);
    }
}