using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria;

namespace TeachMod.TeachUtils.Markdown;

public class ItemRecipeInfo : ItemInfo
{
    public int Count { get => count; }
    public Item Tar { get; init; }
    public Dictionary<int, List<string>> Requireds { get; init; } = [];
    public Dictionary<int, List<string>> Nums { get; init; } = [];
    public Dictionary<int, List<string>> Tiles { get; init; } = [];
    public Dictionary<int, List<string>> CanRequireds { get; init; } = [];
    public Dictionary<int, List<string>> CanNums { get; init; } = [];
    public Dictionary<int, List<string>> CanTiles { get; init; } = [];
    public Dictionary<int, string> CanCreateItem { get; init; } = [];

    public string CreateItme { get; init; }
    private int count = 0;
    private int canCount = 0;
    public ItemRecipeInfo(Item item)
    {
        Tar = item;
        CreateItme = item.Name;
        var currItemRecipe =
            Main.recipe.Where(r => r.createItem.type == item.type);
        var canRecipe = Main.recipe
            .Where(r => r.requiredItem.Select(i => i.Name).Contains(Tar.Name));
        InitValue(currItemRecipe, Requireds, Nums, Tiles, ref count);
        InitValue(canRecipe, CanRequireds, CanNums, CanTiles, ref canCount, CanCreateItem);
    }

    private static void InitValue(IEnumerable<Recipe> currItemRecipe, Dictionary<int, List<string>> requireds, Dictionary<int, List<string>> nums, Dictionary<int, List<string>> tiles,ref int count,  Dictionary<int, string> createItem = null)
    {
        foreach (var recipe in currItemRecipe) {
            createItem?.Add(count, recipe.createItem.Name);
            requireds[count] = [];
            nums[count] = [];
            tiles[count] = [];
            foreach (var requiredItem in recipe.requiredItem) {
                requireds[count].Add(requiredItem.Name);
                nums[count].Add(requiredItem.stack.ToString());
            }
            foreach (var tileID in recipe.requiredTile) {
                var itemname = Items
                    .Where(f => f.createTile != -1)
                    .Where(f => f.createTile == tileID)
                    .FirstOrDefault();
                if (itemname != null) {
                    tiles[count].Add(itemname.Name);
                } else {
                    tiles[count].Add(TileID.Search.GetName(tileID));
                }
            }
            count++;
        }
    }
    
    private List<string> GetAllLineRow(Dictionary<int, List<string>> requireds, Dictionary<int, List<string>> nums, Dictionary<int, List<string>> tiles, Dictionary<int, string> canCreateItem = null)
    {
        var lineRows = new List<string>();
        lineRows.AddRange(["材料", "数量", "制作站", "目标"]);
        for (int i = 0; i <= Count - 1; i++) {
            var cailiao = string.Join("<br>", requireds[i]);
            var shuliang = string.Join("<br>", nums[i]);
            var zhizuozhan = string.Join("<br>", tiles[i]);
            var tar = CreateItme;
            if (canCreateItem != null) {
                tar = canCreateItem[i];
            }
            lineRows.AddRange([cailiao, shuliang, zhizuozhan, tar]);
        }
        return lineRows;
    }

    /// <summary>
    /// 将所有内容合成为一张表格
    /// </summary>
    public List<string> GetAllLineRow()
    {
        return GetAllLineRow(Requireds, Nums, Tiles);
    }

    /// <summary>
    /// 获取使用此物品合成的内容
    /// </summary>
    public List<string> GetAllCanLineRow()
    {
        return GetAllLineRow(Requireds, Nums, Tiles, CanCreateItem);
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

