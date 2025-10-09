namespace TeachMod.Markdown.MarkdownNode;

public abstract class MarkdownNodeBase()
{
    /// <summary>
    /// 当调用<see cref="MarkdownText"/>方法时，是否需要在末尾换行 (\r\n)
    /// </summary>
    public virtual bool NewLine()
    {
        return true;
    }

    /// <summary>
    /// 将此对象转换为Markdown文本
    /// </summary>
    protected abstract string ToMarkdownText();

    /// <summary>
    /// 获取此对象的Markdown文本
    /// </summary>
    public virtual string MarkdownText()
    {
        if (NewLine()) { //需要换行
            return ToMarkdownText() + "\r\n";
        }
        return ToMarkdownText();//不需要换行
    }

    public override string ToString()
    {
        return MarkdownText();
    }
}
