#nullable enable
#pragma warning disable CA2255 // 不应在库中使用 “ModuleInitializer” 属性
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TeachMod.Teachs.Issues;
using TeachMod.Teachs.UIs;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace TeachMod;

	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public class TeachMod : Mod
{
    public TeachMod()
    {
        //ContentAutoloadingEnabled = true;
    }

    private static ILHook? _shopCount;

    private static Dictionary<string, Assembly> modAssemblys = [];

    [ModuleInitializer]
    internal static void TeachModInit()
    {
        var assemblyField = typeof(ModLoader)
            .Assembly
            .GetType("Terraria.ModLoader.Core.AssemblyManager+ModLoadContext")!
            .GetField("assembly")
            ;

        IDictionary modContexts = (IDictionary)typeof(AssemblyManager).GetField("loadedModContexts", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        foreach (var modNameObj in modContexts.Keys) {
            string modName = (modNameObj as string)!;
            var loadContext = modContexts[modName];
            modAssemblys[modName] = (Assembly)assemblyField!.GetValue(loadContext)!;
        }
    }

    public override void Load()
    {
        ModItemUIMouseHook();
        base.Load();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        _ = nameof(NoNPCSpawnPlayer.PlayerConnect);
        _ = nameof(NoNPCSpawn.ILHookItemSlot);
        if (Main.netMode == NetmodeID.Server) {
            var slot = reader.ReadInt32();
            var newFavoritedStatu = reader.ReadBoolean();
            Main.player[whoAmI].inventory[slot].favorited = newFavoritedStatu;
        }
        base.HandlePacket(reader, whoAmI);
    }

    #region Mod列表点击事件
    //Terraria.ModLoader.UI.UIModItem
    //      public override void MouseOut(UIMouseEvent evt)
    //      public override void MouseOver(UIMouseEvent evt)
    //      private UIText _modName

    public readonly static Type UIModItemType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UIModItem")!;
    public readonly static FieldInfo UIModItemModName = UIModItemType!.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance)!;
    private static Hook? _outHook;
    private static Hook? _overHook;
    public static void ModItemUIMouseHook()
    {
        var mouseOut = UIModItemType.GetMethod("MouseOut")!;
        var mouseOver = UIModItemType.GetMethod("MouseOver")!;
        _outHook = new Hook(mouseOut, ModItemUIMouseClickOut);
        _outHook.Apply();
        _overHook = new Hook(mouseOver, ModItemUIMouseClickOver);
        _overHook.Apply();
    }
    
    public delegate void HookMouseOut(object obj, UIMouseEvent evt);
    public static void ModItemUIMouseClickOut(HookMouseOut self, object obj, UIMouseEvent evt)
    {
        self.Invoke(obj, evt);
        //
        var value = (UIText?)UIModItemModName.GetValue(obj);
        if(value != null && value.Text.Contains("TeachMod")) {
            //value.OnLeftClick +=  //yes

            var uielm = (UIElement)obj;
            uielm.OnLeftClick -= OnClick;
        }
    }

    public delegate void HookMouseOver(object obj, UIMouseEvent evt);
    public static void ModItemUIMouseClickOver(HookMouseOut self, object obj, UIMouseEvent evt)
    {
        self.Invoke(obj, evt);
        //
        var value = (UIText?)UIModItemModName.GetValue(obj);
        if (value != null && value.Text.Contains("TeachMod")) {
            var uielm = (UIElement)obj;
            uielm.OnLeftClick += OnClick;
        }
    }

    private static void OnClick(UIMouseEvent ume, UIElement uie)
    {
        Logging.PublicLogger.Debug("点击！");
    }
    #endregion

    #region 商店单次购买数量
    public static void ShopGetCount()
    {
        var methodInfo = typeof(ItemSlot).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(f => f.Name == "HandleShopSlot")!;
        _shopCount =
        new ILHook(methodInfo, il => {
            var ils = il.Body.Instructions;
            var ilc = new ILCursor(il);
            while (ilc.Next != null) {
                if (ilc.TryGotoNext(il => {
                    if (il.MatchLdsfld(out var field)) {
                        return field.Name == nameof(Main.superFastStack);
                    }
                    return false;
                }
                )) {
                    ilc.RemoveRange(2);
                    ilc.Next.OpCode = OpCodes.Ldc_I4;
                    ilc.Next.Operand = 30;
                    break;
                } else {
                    ilc.GotoNext();
                }
            }
        });
    }
    #endregion
}

[Autoload(Side = ModSide.Client)]
public class ListUISystem : ModSystem
{
    private static UIState? _localiListState;
    private static UserInterface? _localiListInterface;
    private static SpriteBatch? _spriteBatch;
    private static GameTime? gameTime;

    private readonly static Type UIModsType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UIMods")!;
    public delegate void UIModsDraw(object uiMods, SpriteBatch sb);
    private static void UIModsDrawHookDel(UIModsDraw self, object uiMods, SpriteBatch sb)
    {
        self.Invoke(uiMods, sb);
        if (_spriteBatch != null && gameTime != null) {
            _localiListInterface?.Update(gameTime);
            _spriteBatch.Begin();
            _localiListInterface?.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();
        }
    }
    private static Hook UIModsDrawHook;
    static ListUISystem()
    {
        var drawMethod = UIModsType.GetMethod("Draw")!;
        UIModsDrawHook = new Hook(drawMethod, UIModsDrawHookDel);
        if (Main.netMode == NetmodeID.Server)
            return;
        Main.QueueMainThreadAction(() => _spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice));
    }

    public override void Load()
    {
        _localiListState = new ListUI();
        _localiListInterface = new UserInterface();
        _localiListInterface.SetState(_localiListState);
        base.Load();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        ListUISystem.gameTime ??= gameTime;
        base.UpdateUI(gameTime);
    }
}