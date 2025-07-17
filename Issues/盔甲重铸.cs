#pragma warning disable CA2255
using Terraria.ModLoader;
using Terraria;
using System.Runtime.CompilerServices;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace TeachMod.Issues;

public class 盔甲重铸
{
    #region 上钩子 让盔甲能接收重铸
    internal delegate bool HookIsAPrefixableAccessoryD(Item obj);

    internal static bool HookMethod(HookIsAPrefixableAccessoryD orig, Item obj)
    {
        return true;
    }

    //IsAPrefixableAccessory
    /// <summary>
    /// Item.TML
    /// </summary>
    [ModuleInitializer]
    internal static void HookIsAPrefixableAccessory()
    {
        //IsAPrefixableAccessory
        var method = typeof(Item).GetMethod("IsAPrefixableAccessory");
        MonoModHooks.Add(method, HookMethod);
    }
    #endregion
}

/// <summary>
/// Item.prefix => 当前前缀修饰号
/// <code>
/// if (rolledPrefix >= PrefixID.Count)
///     PrefixLoader.GetPrefix(rolledPrefix)?.Apply(this); //这个方法就是应用前缀
/// </code>
/// </summary>
public class 盔甲重铸全局物品 : GlobalItem
{
    /// <summary>
    /// 配置项名称
    /// </summary>
    private const string SAVEPREFIXCONFIGNAME = "TeachModPrefix";

    /// <summary>
    /// 触发保存时调用
    /// <para> tag类似字典 </para>
    /// </summary>
    public override void SaveData(Item item, TagCompound tag)
    {
        if(item.prefix != 0) {
            tag[SAVEPREFIXCONFIGNAME] = item.prefix;
        }
        base.SaveData(item, tag);
    }

    /// <summary>
    /// 触发加载时调用
    /// </summary>
    public override void LoadData(Item item, TagCompound tag)
    {
        if(tag.TryGet(SAVEPREFIXCONFIGNAME, out int prefixid)) {
            item.prefix = prefixid;
        }
        base.LoadData(item, tag);
    }

}

public class 盔甲重铸玩家 : ModPlayer
{
    public override void UpdateEquips()
    {
        for(int i = 0; i <= 2; i++) {
            Item item = Player.armor[i];
            if (item != null) {
                Player.GrantPrefixBenefits(item);
            }
        }
        base.UpdateEquips();
    }
}