namespace TeachMod.Markdown.MarkdownNode.Titles;

public class H4Node(string text) : TitleNode(text)
{
    public override int Level()
    {
        return 4;
    }
}
