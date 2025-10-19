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
using ParentUIElement = TeachMod.Udu.UIElement;

namespace TeachMod.Udu;

//1. 将事件只对ActiveUIElement生效 迁移到每个UIElement拥有一个Active
//  1. 在UIElement添加一个字段 --> currentActive
//  2. UIElementLoader遍历每个(没有父元素 && Active) 的UIElement，并进行Draw

public static class UIElementLoader
{
    /// <summary>
    /// 当前活跃的 UIElement
    /// </summary>
    private static UIElement _currentActive = null;

    /// <summary>
    /// 每个顶级父元素当前活跃的UIElement 其中Key是顶级父元素
    /// </summary>
    private readonly static Dictionary<ParentUIElement, UIElement> _currentActives = [];

    /// <summary>
    /// 历史活跃的UIElement
    /// </summary>
    private readonly static List<UIElement> _oldElement = [];

    /// <summary>
    /// 每个顶级父元素的历史活跃UIElement 其中Key是顶级父元素
    /// </summary>
    private readonly static Dictionary<ParentUIElement, UIElement> _oldElements = [];
    
    private static SpriteBatch _spriteBatch;
    /// <summary>
    /// 鼠标悬浮触发
    /// </summary>
    public static event UIMouseEvent MouseHover;

    private static readonly List<Action> _doUpdateHooks = [];
    private static readonly List<UIElement> _elements = [];

    private delegate void DoUpate(Main main, ref GameTime gametime);
    private delegate void DoUpdateAction(DoUpate orig, Main main, ref GameTime gametime);
    /// <summary>
    /// 更新UI 如 触发事件等
    /// </summary>
    private static void DoUpdateHook(DoUpate orig, Main main, ref GameTime gameTime)
    {
        orig.Invoke(main, ref gameTime);
        if(_currentActive?.IsMouseHover() ?? false) {
            _currentActive.InvokMouseHover();
            if(Main.mouseLeft && Main.mouseLeftRelease) {
                _currentActive.InvokMouseClick();
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
        foreach (var uIelement in _elements) {
            if (uIelement.Parent != null || uIelement.active == false) continue;
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            uIelement.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }

    [ModuleInitializer]
    internal static void Hook()
    {
        var mainType = typeof(Terraria.Main);
        Main.QueueMainThreadAction(() => _spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice));
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
        _elements.Add(el);
    }

    /// <summary>
    /// 移除UI列表中的此UI
    /// </summary>
    public static void Remove(UIElement el)
    {
        _elements.Remove(el);
    }

    /// <summary>
    /// 将el的状态设置为False
    /// </summary>
    public static void ActiveFalse(UIElement el)
    {
        //1. 将el的全部elements设置为false (包括elements的element)
        //2. 将el的全部elements在oldElement删除
        //3. 设置并删除最后活跃的element
        var stack = new Stack<UIElement>();
        stack.Push(el);
        while (stack.Count != 0) {
#if DEBUG
            Main.NewText($"UI名称: {el.Name} 被设置为False");
            TeachMod.Mod.Logger.Debug($"UI名称: {el.Name} 被设置为False");
#endif
            var cuel = stack.Pop(); //当前el
            cuel.active = false;
            _oldElement.Remove(cuel);
            foreach (var sel in cuel.elements) {
                stack.Push(sel);
            }
        }

        //3. 如果历史不为空 则设置
        if (_oldElement.Count != 0) {
            var oldel = _oldElement[^1];
            oldel.Active = true; _ = nameof(ActiveTrue);
            _oldElement.Remove(_oldElement[^1]);
        }
    }

    /// <summary>
    /// 设置当前活跃的el
    /// </summary>
    /// <param name="el"></param>
    public static void ActiveTrue(UIElement el)
    {
        foreach (var uel in _elements) {
            uel.active = false;
        }

        foreach (var uel in _elements) {
            if (uel == el) {
                uel.active = true;
                _oldElement.Add(_currentActive); //将当前活跃添加到历史列表
                _currentActive = el;
#if DEBUG
                Main.NewText($"UI名称: {uel.Name} 因为 {el.Name} 被启用而启用");
                Main.NewText($"当前活跃: {el.Name}");
                TeachMod.Mod.Logger.Debug($"UI名称: {uel.Name} 因为 {el.Name} 被启用而启用");
                TeachMod.Mod.Logger.Debug($"当前活跃: {el.Name}");
#endif
                ParentActive(uel, true); //设置父集活跃
                return;
            }
        }
    }

    /// <summary>
    /// 设置此element为给定状态，同时此element的递归父类也会设置为此状态
    /// </summary>
    public static void ParentActive(UIElement element, bool activeStatu)
    {
        do { //父集应当活跃
            element.active = activeStatu;
            element = element.Parent;
        } while (element != null);
    }
}
