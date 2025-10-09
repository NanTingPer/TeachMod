using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace TeachMod.Markdown;
public abstract class ItemInfo
{
    private static List<Item> items;

    /// <summary>
    /// 全部的物品
    /// </summary>
    public static List<Item> Items
    {
        get
        {
            if (items == null) {
                items = [];
                var count = ItemID.Search.Count;
                for (int i = 0; i < count; i++) {
                    items.Add(new Item(i));
                }
            }
            return items;
        }
    }
}
