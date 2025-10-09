using System.Collections.Generic;
using System.Text;

namespace TeachMod.Markdown.MarkdownNode;

/// <summary>
/// HTML格式的表节点
/// </summary>
public class HTMLTableNode : MarkdownNodeBase
{
    /// <summary>
    /// 使用文本列表创建表格
    /// </summary>
    /// <param name="rowValues"> 此表格的文本 列表的前<paramref name="colCount"/>个作为表头  </param>
    /// <param name="colCount"> 有几列 </param>
    public HTMLTableNode(List<string> rowValues, int colCount)
    {
        Rows = [];
        int offset = 0;
        int count = colCount;
        for (int i = 0; i < rowValues.Count; i += count) {
            if(i == 0) {
                Rows.Add(TableRow.GetTableRow(rowValues, offset, count, isHand: true));
            } else {
                Rows.Add(TableRow.GetTableRow(rowValues, offset, count, isHand: false));
            }
            offset += count;
        }
    }

    public HTMLTableNode(List<TableRow> rows)
    {
        Rows = rows;
    }
    public HTMLTableNode(params TableRow[] rows)
    {
        Rows = [.. rows];
    }

    /// <summary>
    /// 表头
    /// </summary>
    public List<TableRow> Rows { get; private set; }

    /// <summary>
    /// 获取或设置此行的值
    /// <para>设置时，索引越界则添加</para>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.IndexOutOfRangeException">获取的索引超出限制</exception>
    public TableRow this[int index]
    {
        get
        {
            if(index > Rows.Count) {
                throw new System.IndexOutOfRangeException("给定的索引已经超越了当前表格的行数");
            }
            return Rows[index];
        }

        set
        {
            if(index > Rows.Count) {
                Rows.Add(value);
            } else {
                Rows[index] = value;
            }
        }
    }

    public static HTMLTableNode operator +(HTMLTableNode table, TableRow row)
    {
        table.Rows.Add(row);
        return table;
    }

    public static HTMLTableNode operator -(HTMLTableNode table, TableRow row)
    {
        table.Rows.Remove(row);
        return table;
    }

    public static implicit operator string(HTMLTableNode table)
    {
        return table.ToString();
    }

    protected override string ToMarkdownText()
    {
        var strBulid = new StringBuilder();
        strBulid.Append("<table>");
        strBulid.AppendLine();
        for (int i = 0; i < Rows.Count; i++) {
            strBulid.Append(Rows[i].FormateText);
        }
        strBulid.AppendLine();
        strBulid.Append("</table>");
        return strBulid.ToString();
    }

    public override string ToString()
    {
        return ToMarkdownText();
    }
}

/// <summary>
/// 行
/// </summary>
public class TableRow : TableRowCell
{
    public TableRow(params Cell[] cells)
    {
        Row = [.. cells];
    }

    public TableRow(List<Cell> cells)
    {
        Row = [.. cells];
    }

    public bool isHand = false;
    public List<Cell> Row { get; set; }
    public string FormateText
    {
        get
        {
            var strBulid = new StringBuilder();
            strBulid.Append("<tr>");
            strBulid.AppendLine();
            for (int i = 0; i < Row.Count; i++) {
                var item = Row[i];
                if (isHand) {
                    strBulid.Append("<th ");
                } else {
                    strBulid.Append("<td ");
                }
                strBulid.Append($"colspan=\"{item.ColSpan}\" ");
                strBulid.Append($"rowspan=\"{item.RowSpan}\" ");
                strBulid.Append('>');
                strBulid.Append(item.Text);

                if (isHand) {
                    strBulid.Append("</th>");
                } else {
                    strBulid.Append("</td>");
                }
                strBulid.AppendLine();
            }
            strBulid.Append("</tr>");
            return strBulid.ToString();
        }
    }

    public override string ToString()
    {
        return FormateText;
    }

    public static implicit operator string(TableRow tr)
    {
        return tr.FormateText;
    }
}

/// <summary>
/// 单元格
/// </summary>
public class Cell : TableRowCell
{
    public static Cell Create(string text, int rowSpan = 1, int colSpan = 1)
    {
        return new Cell() { Text = text, RowSpan = rowSpan, ColSpan = colSpan };
    }

    /// <summary>
    /// 本格文本
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 跨几行
    /// </summary>
    public int RowSpan { get; set; } = 1;

    /// <summary>
    /// 跨几列
    /// </summary>
    public int ColSpan { get; set; } = 1;
}

public abstract class TableRowCell
{
    /// <summary>
    /// 从列表创建列
    /// </summary>
    /// <param name="values">值</param>
    /// <param name="offset">开始的index</param>
    /// <param name="count"> 取多少个 </param>
    /// <returns></returns>
    public static Cell[] GetCells(List<string> values, int offset, int count)
    {
        List<Cell> cells = [];
        for(int i = offset; i < offset + count; i++) {
            if(i > values.Count) {
                throw new System.IndexOutOfRangeException("给定索引超出内容值");
            }
            cells.Add(new Cell() { Text = values[i] });
        }
        return [.. cells];
    }

    public static TableRow GetTableRow(List<string> values, int offset, int count, bool isHand = false) => new TableRow(GetCells(values, offset, count)) { isHand = isHand };
}