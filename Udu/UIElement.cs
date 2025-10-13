using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace TeachMod.Udu;

public class UIElement
{
    public UIElement()
    {
        UIElementLoader.Region(this);
    }
    
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

    internal bool active = false;
    /// <summary>
    /// 决定此UI是否有效
    /// </summary>
    public bool Active
    {
        get => active;
        set
        {
            if (value == true) {
                UIElementLoader.ActiveTrue(this);
            } else {
                UIElementLoader.ActiveFalse(this);
            }
            active = value;
        }
    }
    /// <summary>
    /// 是否使用绝对定位
    /// </summary>
    public bool IsAbsLocate { get; set; } = false;
    /// <summary>
    /// 是否为面板
    /// </summary>
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

    /// <summary>
    /// 实际调用的Draw 在<see cref="UIElementLoader.DoDrawHook(UIElementLoader.DoDraw, Main, GameTime)"/> 中被调用
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        DrawSelf(spriteBatch);
        foreach (var element in elements) {
            element.Draw(spriteBatch);
        }
        spriteBatch.End();

        #region 测试鼠标
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
        //var mxy = Vector2.Transform(Main.MouseScreen, Main.GameViewMatrix.EffectMatrix);
        var mxy = Main.MouseScreen;
        var mwh = new Vector2(12, 12);
        var rect = new Rectangle((int)mxy.X, (int)mxy.Y, (int)mwh.X, (int)mwh.Y);
        spriteBatch.Draw(Texture, rect, Color.White);
        spriteBatch.End();
        #endregion
    }

    /// <summary>
    /// 由<see cref="Draw(SpriteBatch)"/>调用
    /// </summary>
    public virtual void DrawSelf(SpriteBatch spriteBatch)
    {
        if (IsPanle) {
            spriteBatch.Draw(Texture, DrawRectangle, Color.Black);
        }
    }

    /// <summary>
    /// 将此UI从<see cref="UIElementLoader"/>中移除
    /// </summary>
    public void Disposable()
    {
        UIElementLoader.Remove(this);
    }

    #region 辅助方法
    /// <summary>
    /// 判断鼠标是否在当前容器内
    /// </summary>
    public bool IsMouseHover()
    {
        var matrix = Main.UIScaleMatrix;
        var xy = Vector2.Transform(new Vector2(XOffset, YOffset), matrix);
        var wh = Vector2.Transform(new Vector2(Width, Height), matrix);
        var mxy = Vector2.Transform(Main.MouseScreen, Main.GameViewMatrix.ZoomMatrix); //鼠标位置
        var mwh = new Vector2(2, 2); //鼠标大小
        return Collision.CheckAABBvAABBCollision(xy, wh, mxy, mwh);
    }
    #endregion

    #region 事件调用方法
    /// <summary>
    /// 调用 <see cref="MouseHover"/> 由<see cref="UIElementLoader.DoUpdateHook(UIElementLoader.DoUpate, Main, ref GameTime)调用"/>
    /// </summary>
    internal void InvokMouseHover()
    {
        //var expc = new Exception();
        //var st = new StackTrace(expc);
        //var frame = st.GetFrame(st.FrameCount - 1);
        //if(frame.GetMethod()?.Name == "DoUpdateHook") {
        MouseHover.Invoke(new UIMouseEventArgs(this, Main.MouseScreen));
        //}
    }
    #endregion
}