using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Teachs.RecipeGroupDir;

public class RecipeGroupSystem : ModSystem
{
    public override void AddRecipeGroups()
    {
        //int[] ids = [
        //    ItemID.DirtWall,
        //    ItemID.StoneWall
        //];

        List<int> ids = [
            ItemID.DirtWall,
            ItemID.StoneWall
        ];
        
        if (ModLoader.TryGetMod("Nam", out var mod)) {
            if(mod.TryFind<ModItem>("", out var value)) {
                ids.Add(value.Type);
            }
        }

        var rg = new RecipeGroup(GetName, [.. ids]);

        RecipeGroup.RegisterGroup("TeachMod:Wall", rg);
        base.AddRecipeGroups();
    }

    private static string GetName()
    {
        return "土墙 或 石墙";
    }
}

public class RecipeGroupItem : ModItem
{
    public override void AddRecipes()
    {
        CreateRecipe(1)
            .AddRecipeGroup("TeachMod:Wall", 2)
            .Register()
            ;
        base.AddRecipes();
    }
}
