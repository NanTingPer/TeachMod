using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeachMod.Markdown.MarkdownNode;

public class TableNode : MarkdownNodeBase
{
    //x, y
    private string[][] tableText;

    /// <summary>
    /// 使用<see cref="List{string}"/>创建表格节点
    /// </summary>
    /// <param name="tableValue"> 表格内容 </param>
    /// <param name="colCount"> 表格列数 </param>
    public TableNode(List<string> tableValue, int colCount)
    {
        int skipCount = 0;
        int col = colCount;
        if(tableValue.Count % colCount != 0) {
            throw new System.Exception($"在创建{nameof(TableNode)}时, 列表内容的数量{tableValue.Count}，无法整除给定列{colCount}。");
        }

        //行数
        int rowCount = tableValue.Count / colCount;
        tableText = new string[rowCount][];
        for(int row = 0; row < rowCount; row++) {
            tableText[row] = tableValue.Skip(skipCount).Take(col).ToArray();
            skipCount += colCount;
        }
    }

    public override bool NewLine()
    {
        return true;
    }

    protected override string ToMarkdownText()
    {
        var strBulid = new StringBuilder();
        for (int x = 0; x < tableText.Length; x++) {
            foreach (var rowValue in tableText[x]) {
                strBulid.Append('|');
                strBulid.Append(rowValue);
            }
            strBulid.Append('|');
            strBulid.AppendLine();
            if (x == 0) { //第一行
                strBulid.Append("| ---- | ---- | ---- |");
                strBulid.AppendLine();
            }
        }

        return strBulid.ToString();
    }
}