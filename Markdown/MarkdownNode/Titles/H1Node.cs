namespace TeachMod.Markdown.MarkdownNode.Titles;

public class H1Node(string text) : TitleNode(text)
{
    public override int Level()
    {
        return 1;
    }
}
