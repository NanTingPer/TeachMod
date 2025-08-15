using TeachMod.TeachUtils.Markdown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Systems;

public class GetItemMarkdownSystem : ModSystem
{
    public override void PostSetupRecipes()
    {
        //var item = new Item(ItemID.Zenith);
        var item = new Item(ItemID.Wood);
        var makstr = ItemGenerator.Create(item);
        base.PostSetupRecipes();
    }
}
