#pragma warning disable CA2255
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace TeachMod.Udu;

public static class UIElementLoader
{
    /// <summary>
    /// 当前活跃的 UIElement
    /// </summary>
    private static UIElement currentActive = null;
    /// <summary>
    /// 历史活跃的UIElement
    /// </summary>
    private static List<UIElement> oldElement = [];
    private static SpriteBatch spriteBatch;
    /// <summary>
    /// 鼠标悬浮触发
    /// </summary>
    public static event UIMouseEvent MouseHover;

    private static readonly List<Action> doUpdateHooks = [];
    private static readonly List<UIElement> elements = [];

    private delegate void DoUpate(Main main, ref GameTime gametime);
    private delegate void DoUpdateAction(DoUpate orig, Main main, ref GameTime gametime);
    /// <summary>
    /// 更新UI 如 触发事件等
    /// </summary>
    private static void DoUpdateHook(DoUpate orig, Main main, ref GameTime gameTime)
    {
        orig.Invoke(main, ref gameTime);
        if(currentActive?.IsMouseHover() ?? false) {
            currentActive.InvokMouseHover();
            if(Main.mouseLeft && Main.mouseLeftRelease) {
                currentActive.InvokMouseClick();
            }
        }
    }

    private delegate void DrawCapture(Main main, Microsoft.Xna.Framework.Rectangle area, CaptureSettings settings);
    private delegate void DrawCaptureAction(DrawCapture orig, Main main, Microsoft.Xna.Framework.Rectangle area, CaptureSettings settings);
    private static void DrawCaptureHook(DrawCapture orig, Main main, Microsoft.Xna.Framework.Rectangle area, CaptureSettings settings)
    {
        orig.Invoke(main, area, settings);
    }

    private delegate void DoDraw(Main main, GameTime gametime);
    private delegate void DoDrawAction(DoDraw orig, Main main, GameTime gametime);

    private static void DoDrawHook(DoDraw orig, Main main, GameTime gametime)
    {
        orig.Invoke(main, gametime);
        foreach (var uIElement in elements) {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            if (uIElement.Active && uIElement.Parent == null)
                uIElement.Draw(spriteBatch);
            spriteBatch.End();
        }
    }

    [ModuleInitializer]
    internal static void Hook()
    {
        var mainType = typeof(Terraria.Main);
        Main.QueueMainThreadAction(() => spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice));
        var doUpdate = mainType.GetMethod("DoUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        var drawCapture = mainType.GetMethod("DrawCapture", BindingFlags.Public | BindingFlags.Instance);
        var doDraw = mainType.GetMethod("DoDraw", BindingFlags.NonPublic | BindingFlags.Instance);
        //MonoModHooks.Add(doUpdate, new DoUpdateAction(DoUpdateHook));
        MonoModHooks.Add(drawCapture, new DrawCaptureAction(DrawCaptureHook));
        MonoModHooks.Add(doUpdate, new DoUpdateAction(DoUpdateHook));
        MonoModHooks.Add(doDraw, new DoDrawAction(DoDrawHook));
    }

    /// <summary>
    /// 往UI列表添加UI
    /// </summary>
    /// <param name="el"></param>
    public static void Region(UIElement el)
    {
        elements.Add(el);
    }

    /// <summary>
    /// 移除UI列表中的此UI
    /// </summary>
    public static void Remove(UIElement el)
    {
        elements.Remove(el);
    }

    /// <summary>
    /// 将el的状态设置为False
    /// </summary>
    public static void ActiveFalse(UIElement el)
    {
        foreach (var element in elements) {
            if(element == el) {
                el.active = false;
                if(oldElement.Count != 0) {
                    var oldel = oldElement[^1];
                    oldel.Active = true; _ = nameof(ActiveTrue);
                    oldElement.Remove(oldElement[^1]);
                }
                return;
            }
        }
    }

    /// <summary>
    /// 设置当前活跃的el
    /// </summary>
    /// <param name="el"></param>
    public static void ActiveTrue(UIElement el)
    {
        foreach (var uel in elements) {
            uel.active = false;
        }

        foreach (var uel in elements) {
            if (uel == el) {
                uel.active = true;
                oldElement.Add(currentActive); //将当前活跃添加到历史列表
                currentActive = el;
                ParentActive(uel, true);
                return;
            }
        }
    }

    public static void ParentActive(UIElement element, bool activeStatu)
    {
        do { //父集应当活跃
            element.active = true;
            element = element.Parent;
        } while (element != null);
    }
}
