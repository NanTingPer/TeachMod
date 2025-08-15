using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria;

namespace TeachMod.TeachUtils.Markdown;

public class ItemRecipeInfo : ItemInfo
{
    public int Count { get => count; }
    public Dictionary<int, List<string>> Requireds { get; init; } = [];
    public Dictionary<int, List<string>> Nums { get; init; } = [];
    public Dictionary<int, List<string>> Tiles { get; init; } = [];
    public string CreateItme { get; init; }
    private int count = 0;
    public ItemRecipeInfo(Item item)
    {
        CreateItme = item.Name;
        var currItemRecipe =
            Main.recipe.Where(r => r.createItem.type == item.type);

        foreach (var recipe in currItemRecipe) {
            Requireds[count] = [];
            Nums[count] = [];
            Tiles[count] = [];
            foreach (var requiredItem in recipe.requiredItem) {
                Requireds[count].Add(requiredItem.Name);
                Nums[count].Add(requiredItem.stack.ToString());
            }
            foreach (var tileID in recipe.requiredTile) {
                var itemname = Items
                    .Where(f => f.createTile != -1)
                    .Where(f => f.createTile == tileID)
                    .FirstOrDefault();
                if (itemname != null) {
                    Tiles[count].Add(itemname.Name);
                } else {
                    Tiles[count].Add(TileID.Search.GetName(tileID));
                }
            }
            count++;
        }
    }

    /// <summary>
    /// 将所有内容合成为一张表格
    /// </summary>
    public List<string> GetAllLineRow()
    {
        var lineRows = new List<string>();
        lineRows.AddRange(["材料", "数量", "制作站", "目标"]);
        for (int i = 0; i <= Count - 1; i++) {
            var cailiao = string.Join("<br>", Requireds[i]);
            var shuliang = string.Join("<br>", Nums[i]);
            var zhizuozhan = string.Join("<br>", Tiles[i]);
            var tar = CreateItme;
            lineRows.AddRange([cailiao, shuliang, zhizuozhan, tar]);
        }
        return lineRows;
    }

    /// <summary>
    /// 每个列表就是一个合成配方
    /// </summary>
    public List<List<string>> GetLineRow()
    {
        var values = new List<List<string>>();

        for (int i = 0; i <= Count - 1; i++) {
            var lineRows = new List<string>();
            lineRows.AddRange(["材料", "数量", "制作站", "目标"]);
            var cailiao = string.Join("<br>", Requireds[i]);
            var shuliang = string.Join("<br>", Nums[i]);
            var zhizuozhan = string.Join("<br>", Tiles[i]);
            var tar = CreateItme;
            lineRows.AddRange([cailiao, shuliang, zhizuozhan, tar]);
            values.Add(lineRows);
        }
        return values;
    }
}

