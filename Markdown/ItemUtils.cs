using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace TeachMod.Markdown;

public static class ItemUtils
{
    /// <summary>
    /// 获取此物品的合成表
    /// </summary>
    public static IEnumerable<Recipe> Recipes(this Item item)
    {
        return Main.recipe.OrderBy(r => r.RecipeIndex).Where(r => r.createItem.type == item.type);
    }
    /// <summary>
    /// 获取可以被此物品合成的合成表
    /// </summary>
    public static IEnumerable<Recipe> CanRecipes(this Item item)
    {
        return Main.recipe
            .OrderBy(r => r.RecipeIndex)
            .Where(r => r.requiredItem.Select(i => i.type).Contains(item.type));
    }
    /// <summary>
    /// <para> 解构合成表列表为 (recipeIndex, requiredItemName, count) </para> 
    /// <para> 使用recipeIndex进行GroupBy可以得到单个合成表解构的结果 </para>
    /// </summary>
    public static List<(int recipeIndex, string requiredItemName, int count)> RequiredItems(this IEnumerable<Recipe> recipes)
    {
        var list = new List<(int recipeIndex, string requiredItemName, int count)>();
        foreach (var recipe in recipes.OrderBy(r => r.RecipeIndex)) {
            foreach (var item in recipe.requiredItem) {
                list.Add((recipe.RecipeIndex, item.Name, item.stack));
            }
        }
        return list;
    }
    /// <summary>
    /// <para> 将此合成表列表装入字典 </para>
    /// <para> count => 第几个合成表 </para>
    /// <para> 每个List代表一个合成表内容，其他name是材料名，count是材料数量 </para>
    /// </summary>
    public static Dictionary<int, List<(string name, int count)>> RequiredItems(this IEnumerable<Recipe> recipes, ref int count)
    {
        var dic = new Dictionary<int, List<(string name, int count)>>();
        dic.RequiredItems(recipes, ref count);
        return dic;
    }
    /// <summary>
    /// <para> 将此合成表列表装入字典 </para>
    /// <para> count => 第几个合成表 </para>
    /// <para> 每个List代表一个合成表内容，其他name是材料名，count是材料数量 </para>
    /// </summary>
    public static void RequiredItems(this Dictionary<int, List<(string name, int count)>> dic, IEnumerable<Recipe> recipes, ref int count)
    {
        foreach (var recipe in recipes.OrderBy(r => r.RecipeIndex)) {
            dic[count] = [];
            foreach (var item in recipe.requiredItem) {
                dic[count].Add(item.Name, item.stack);
            }
            count++;
        }
    }
    /// <summary>
    /// <para> 将此合成表列表装入字典 </para>
    /// <para> count => 第几个合成表 </para>
    /// <para> 每个List代表一个合成表内容，其他name是材料名，count是材料数量 </para>
    /// </summary>
    public static void RequiredItems(this IEnumerable<Recipe> recipes, Dictionary<int, List<(string name, int count)>> dic, ref int count)
        => RequiredItems(dic, recipes, ref count);
    /// <summary>
    /// <para> 将此合成表列表装入字典，<paramref name="requiredDic"/>是材料名字字典，<paramref name="countDic"/>是该材料所需数量的字典</para>
    /// <para> count => 第几个合成表 </para>
    /// </summary>
    public static void RequiredItems(this IEnumerable<Recipe> recipes, Dictionary<int, List<string>> requiredDic, Dictionary<int, List<string>> countDic, ref int count)
    {
        var groupValue = recipes.RequiredItems()
            .GroupBy(v => v.recipeIndex)
            .Select(ig => ig.ToList());

        foreach (var values in groupValue) {
            requiredDic[count] = []; countDic[count] = [];
            foreach (var item in values) {
                requiredDic[count].Add(item.requiredItemName); countDic[count].Add(item.count.ToString());
            }
            count++;
        }
    }
    /// <summary>
    /// 获取此合成表索引对应合成表所需的制作站名称
    /// </summary>
    /// <param name="recipeIndex">此合成表在<see cref="Main.recipe"/>中的索引</param>
    /// <param name="items">全部物品</param>
    public static IEnumerable<string> Tiles(this int recipeIndex, List<Item> items)
    {
        foreach (var tileID in Main.recipe[recipeIndex].requiredTile) {
            var itemname = items
                .Where(f => f.createTile != -1)
                .Where(f => f.createTile == tileID)
                .FirstOrDefault();
            if (itemname != null) {
                yield return itemname.Name;
            } else {
                yield return TileID.Search.GetName(tileID);
            }
        }
        yield break;
    }
    /// <summary>
    /// 获取此合成表所需的制作站名称
    /// </summary>
    /// <param name="items">全部物品</param>
    public static IEnumerable<string> Tiles(this Recipe recipe, List<Item> items)
        => Tiles(recipe.RecipeIndex, items);

    public static void RequCountTiles(this IEnumerable<Recipe> currItemRecipe, Dictionary<int, List<string>> requireds, Dictionary<int, List<string>> nums, Dictionary<int, List<string>> tiles, ref int countv, List<Item> items, Dictionary<int, string> createItem = null)
    {
        foreach (var recipe in currItemRecipe) {
            createItem?.Add(countv, recipe.createItem.Name);
            requireds[countv] = [];
            nums[countv] = [];
            tiles[countv] = [];
            foreach (var requiredItem in recipe.requiredItem) {
                requireds[countv].Add(requiredItem.Name);
                nums[countv].Add(requiredItem.stack.ToString());
            }
            foreach (var tileID in recipe.requiredTile) {
                var itemname = items
                    .Where(f => f.createTile != -1)
                    .Where(f => f.createTile == tileID)
                    .FirstOrDefault();
                if (itemname != null) {
                    tiles[countv].Add(itemname.Name);
                } else {
                    tiles[countv].Add(TileID.Search.GetName(tileID));
                }
            }
            countv++;
        }
    }

    public static void Add<T1, T2>(this List<(T1, T2)> list, T1 t1, T2 t2)
    {
        list.Add((t1, t2));
    }
}
