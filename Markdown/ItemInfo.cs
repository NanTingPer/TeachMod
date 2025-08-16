using System.Collections.Generic;
using Terraria.ID;
using Terraria;

namespace TeachMod.Markdown;
public abstract class ItemInfo
{
    private static List<Item> items;
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
