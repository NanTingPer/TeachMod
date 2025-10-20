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

// 在触发事件时 遍历当前UI列表 为顶层UI触发
// 如果是单击低层UI 则将低层UI置于顶层

public static class UIElementLoader
{
    /// <summary>
    /// 放置层级修改时 Draw的foreach报错，采用队列
    /// <para> 锁: <see cref="_elementIndexEditLock"/> </para>
    /// </summary>
    private readonly static Queue<Action> _elementIndexEdit = [];
    /// <summary>
    /// <see cref="_elementIndexEdit"/> 队列添加与消耗的锁
    /// </summary>
    private readonly static object _elementIndexEditLock = new();
    /// <summary>
    /// 当前活跃的 UIElement
    /// </summary>
    //private static UIElement _currentActive = null;

    /// <summary>
    /// 历史活跃的UIElement
    /// </summary>
    //private readonly static List<UIElement> _oldElement = [];

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
        //遍历全部UI 为顶层UI发送事件
        for (int i = _elements.Count - 1; i >= 0; i--) {
            var forelement = _elements[i];
            if (forelement.active == false || forelement.IsMouseHover() == false)
                continue;
            forelement.InvokMouseHover();
            //没按下鼠标就不管了
            if (!Main.mouseLeft && !Main.mouseLeftRelease) {
                forelement.InvokMouseClick();
                //拉到顶
                lock (_elementIndexEditLock) {
                    try {
                        _elementIndexEdit.Enqueue(() => {
                            _elements.Remove(forelement);
                            _elements.Add(forelement);
                        });
                    } finally { }
                }
            }
            break;
        }

        //2
        //if (_currentActive?.IsMouseHover() ?? false) {
        //    _currentActive.InvokMouseHover();
        //    if(Main.mouseLeft && Main.mouseLeftRelease) {
        //        _currentActive.InvokMouseClick();
        //    }
        //}
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
        //这里可能会报错，因为遍历的时候不能添加内容
        foreach (var uIelement in _elements) {
            if (uIelement.Parent != null || uIelement.active == false) continue;
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            uIelement.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        //将元素移动到顶部
        lock (_elementIndexEditLock) {
            try {
                while(_elementIndexEdit.TryDequeue(out var action)) {
                    action.Invoke();
                }
            } finally { }
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
        _elementIndexEdit.Enqueue(() => _elements.Add(el));
    }

    /// <summary>
    /// 移除UI列表中的此UI
    /// </summary>
    public static void Remove(UIElement el)
    {
        _elementIndexEdit.Enqueue(() => _elements.Remove(el));
        
    }

    /// <summary>
    /// 将传入的UIElement设置为不活跃，其<see cref="UIElement.elements"/>也会变为不活跃
    /// </summary>
    public static void ActiveFalse(UIElement el)
    {
        //1. 将el的全部elements设置为false (包括elements的element)
        //2. 将el的全部elements在oldElement删除
        //3. 设置并删除最后活跃的element
        el.TraverseElements(u => u.active = false);

        //var stack = new Stack<UIElement>();
        //stack.Push(el);
        //while (stack.Count != 0) {
        //    var cuel = stack.Pop(); //当前el
        //    cuel.active = false;
        //    _oldElement.Remove(cuel);
        //    foreach (var sel in cuel.elements) {
        //        stack.Push(sel);
        //    }
        //}
        //
        ////3. 如果历史不为空 则设置
        //if (_oldElement.Count != 0) {
        //    var oldel = _oldElement[^1];
        //    oldel.Active = true; _ = nameof(ActiveTrue);
        //    _oldElement.Remove(_oldElement[^1]);
        //}
    }

    /// <summary>
    /// 将传入的UIElement设置为活跃，其父也会变为活跃
    /// </summary>
    /// <param name="el"></param>
    public static void ActiveTrue(UIElement el)
    {
        el.active = true;
        el.TraverseParent(u => u.active = true);
        //foreach (var uel in _elements) {
        //    uel.active = false;
        //}
        //
        //foreach (var uel in _elements) {
        //    if (uel == el) {
        //        uel.active = true;
        //        _oldElement.Add(_currentActive); //将当前活跃添加到历史列表
        //        _currentActive = el;
        //        ParentActive(uel, true); //设置父集活跃
        //        return;
        //    }
        //}
    }

    /// <summary>
    /// 设置此element为给定状态，同时此element的递归父类也会设置为此状态
    /// </summary>
    //public static void ParentActive(UIElement element, bool activeStatu)
    //{
    //    do { //父集应当活跃
    //        element.active = activeStatu;
    //        element = element.Parent;
    //    } while (element != null);
    //}
}
