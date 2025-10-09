namespace TeachMod.Markdown.MarkdownNode.Titles;

public class H3Node(string text) : TitleNode(text)
{
    public override int Level()
    {
        return 3;
    }
}
