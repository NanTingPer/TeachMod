using Terraria;

namespace TeachMod.TeachUtils.Markdown;

public static class ItemGenerator
{
    public static string Create(Item item)
    {
        var generator = new Generator();

        generator.AppendH(item.Name, 1);
        generator.AppendH("描述", 2);
        //描述
        var toolTipLines = item.ToolTip.Lines;
        for (int i = 0; i < toolTipLines; i++) {
            generator.AppendText(item.ToolTip.GetLine(i));
        }

        var recipeInfos = new ItemRecipeInfo(item);
        var lineRows = recipeInfos.GetAllLineRow();
        generator
            .AppendSplitLine()
            .AppendH("合成", 2)
            .AppendTable(lineRows, 4)
            ;
        return generator.Build;
    }
}