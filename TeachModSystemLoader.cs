using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;

namespace TeachMod;

public class TeachModSystemLoader
{
    private static SpriteBatch _spriteBatch;
    private readonly static List<TeachModSystem> teachModSystems = [];

    
#pragma warning disable CA2255 // 不应在库中使用 “ModuleInitializer” 属性
    [ModuleInitializer]
#pragma warning restore CA2255 // 不应在库中使用 “ModuleInitializer” 属性
    public static void Init()
    {
        var mainType = typeof(Terraria.Main);
        #region DoDraw
        var doDraw = mainType.GetMethod("DoDraw", BindingFlags.NonPublic | BindingFlags.Instance);
        MonoModHooks.Add(doDraw, new DelegateDoDrawAction(DoDraw));
        Main.QueueMainThreadAction(() => _spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice));
        #endregion

        #region DoUpdate
        var doUpdate = mainType.GetPrivateInstance("DoUpdate");
        MonoModHooks.Add(doUpdate, new DelegateDoUpdateAction(DoUpdate));
        #endregion

        var types = typeof(TeachModSystemLoader).Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TeachModSystem)))
            ;
        foreach (var type in types) {
            teachModSystems.Add((TeachModSystem)System.Activator.CreateInstance(type));
        }
    }

    #region DoUpdate
    public delegate void DelegateDoUpdate(Main main, ref GameTime gameTime);
    public delegate void DelegateDoUpdateAction(DelegateDoUpdate orig, Main main, ref GameTime gameTime);
    private static void DoUpdate(DelegateDoUpdate orig, Main main, ref GameTime gameTime)
    {
        foreach (var item in teachModSystems) {
            try { item.PreUpdate(main, ref gameTime); } catch { }
        }
        orig.Invoke(main, ref gameTime);
        foreach (var item in teachModSystems) {
            try { item.PostUpdate(main, ref gameTime); } catch { }
        }
    }
    #endregion

    #region DoDraw
    public delegate void DelegateDoDraw(Main main, GameTime gametime);
    public delegate void DelegateDoDrawAction(DelegateDoDraw orig, Main main, GameTime gametime);
    private static void DoDraw(DelegateDoDraw orig, Main main, GameTime gametime)
    {
        if(_spriteBatch == null) {
            return;
        }

        foreach (var item in teachModSystems) {
            try { item.PreDoDraw(_spriteBatch, main, gametime); } catch { }
        }
        orig.Invoke(main, gametime);
        foreach (var item in teachModSystems) {
            try { item.PostDoDraw(_spriteBatch, main, gametime); } catch { }
        }

    }
    #endregion
}

public abstract class TeachModSystem : ModSystem
{
    public virtual void PreDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {

    }

    public virtual void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {

    }

    public virtual void PreUpdate(Main main, ref GameTime gametime)
    {

    }

    public virtual void PostUpdate(Main mian, ref GameTime gameTime)
    {

    }
}

public static class TypeExtensions
{
    extension(Texture2D texture2D)
    {
        /// <summary>
        /// 创建一个纯色纹理
        /// </summary>
        /// <param name="graphicsDevice"> gd </param>
        /// <param name="w"> 宽 </param>
        /// <param name="h"> 高 </param>
        /// <param name="color"> 颜色 </param>
        /// <returns></returns>
        public static Texture2D CreateTexture2D(GraphicsDevice graphicsDevice, int w, int h, Color color)
        {
            var texture = new Texture2D(graphicsDevice, w, h);
            var colors = new Color[w * h];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = color;
            }
            texture.SetData(colors);
            return texture;
        }

        public static Texture2D CreateTexture2D(int w, int h, Color color)
            => CreateTexture2D(Main.graphics.GraphicsDevice, w, h, color);

    }

    extension(Type type)
    {
        public MethodInfo GetPrivateInstance(string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public MethodInfo GetPublicInstance(string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        }

        public MethodInfo GetPrivateStatic(string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        }

        public MethodInfo GetPublicStatic(string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        }
    }
}