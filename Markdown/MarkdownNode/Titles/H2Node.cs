namespace TeachMod.Markdown.MarkdownNode.Titles;

public class H2Node(string text) : TitleNode(text)
{
    public override int Level()
    {
        return 2;
    }
}
