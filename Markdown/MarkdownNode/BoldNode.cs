namespace TeachMod.Markdown.MarkdownNode;

/// <summary>
/// 加粗
/// </summary>
/// <param name="text"> 要被加粗的文本 </param>
public class BoldNode(string text) : MarkdownNodeBase
{
    public string Text { get; init; } = text;
    public override bool NewLine()
    {
        return false;
    }

    protected override string ToMarkdownText()
    {
        return "**" + Text + "**";
    }
}
