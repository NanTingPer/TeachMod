using System.Collections.Generic;
using System.Text;
using TeachMod.Markdown.MarkdownNode;

namespace TeachMod.Markdown;

public class MarkdownDocument
{
    /// <summary>
    /// 本文本的全部节点
    /// </summary>
    public List<MarkdownNodeBase> Nodes { get; private set; }

    /// <summary>
    /// 向文档添加节点
    /// </summary>
    public MarkdownDocument Append(MarkdownNodeBase node)
    {
        Nodes.Add(node);
        return this;
    }

    /// <summary>
    /// 获取此文档的markdown文本
    /// </summary>
    public string GetMarkdownText()
    {
        var strBuild = new StringBuilder();
        foreach (var markdownNode in Nodes) {
            strBuild.Append(markdownNode.MarkdownText());
        }
        return strBuild.ToString();
    }
}
