using System.Text;

namespace TeachMod.Markdown.MarkdownNode.Titles;

public abstract class TitleNode(string text) : MarkdownNodeBase
{
    public string Text { get; init; } = text;

    /// <summary>
    /// 此标题的级别
    /// </summary>
    public abstract int Level();

    /// <summary>
    /// 标题往往都会换行
    /// </summary>
    public override bool NewLine()
    {
        return true;
    }

    protected override string ToMarkdownText()
    {
        StringBuilder sb = new StringBuilder();
        int level = Level();
        for(int i = 0; i < level; i++) {
            sb.Append('#');
        }
        sb.Append(Text);
        return sb.ToString();
    }
}
