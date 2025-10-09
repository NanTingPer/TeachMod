namespace TeachMod.Markdown.MarkdownNode;

/// <summary>
/// 纯文本节点
/// </summary>
public class TextNode(string text) : MarkdownNodeBase
{
    public string Text { get; set; } = text;
    public override bool NewLine()
    {
        return false;
    }
    protected override string ToMarkdownText()
    {
        return Text;
    }
}
