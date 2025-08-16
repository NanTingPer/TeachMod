using System;
using System.Collections.Generic;
using System.Text;

namespace TeachMod.Markdown;

public class Generator
{
    private StringBuilder markdownValue = new();
    public string Build { get => markdownValue.ToString(); }
    public Generator AppendH(string value, int live)
    {
        markdownValue.AppendLine();
        for (int i = 1; i <= Math.Min(6, live); i ++) {
            markdownValue.Append('#');
        }

        markdownValue
            .Append(' ')
            .Append(value)
            ;
        return this;
    }

    public Generator AppendSplitLine()
    {
        markdownValue.AppendLine();
        markdownValue.AppendLine("---");
        return this;
    }

    public Generator AppendIamge(string src, string desc)
    {
        markdownValue
            .AppendLine()
            .Append('!')
            .Append('[')
            .Append(desc)
            .Append(']')
            .Append('(')
            .Append(src)
            .Append(')')
            ;
        return this;
    }

    public Generator AppendTable(string[][] value)
    {
        markdownValue.AppendLine();

        #region 表头
        for (int i = 0; i < value[0].Length; i ++) {
            markdownValue
                .Append('|')
                .Append(value[0][i])
                ;
        }
        markdownValue.Append('|');
        #endregion 表头

        #region 分割线
        markdownValue.AppendLine();
        for (int i = 0; i < value[0].Length; i++) {
            markdownValue
                .Append('|')
                .Append("----")
                ;
        }
        markdownValue.Append('|');
        #endregion 分割线

        #region 内容
        for (int i = 1; i < value.Length; i++) {
            markdownValue.AppendLine();
            for (int j = 0; j < value[i].Length; j++) {
                markdownValue
                    .Append('|')
                    .Append(value[i][j])
                    ;
            }
            markdownValue.Append('|');
        }
        #endregion
        return this;
    }

    /// <summary>
    /// 第0行应该是表头
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Generator AppendTable(string[,] value)
    {
        var line = value.GetLength(0);//行
        var row = value.GetLength(1);//列

        var arrays = new string[line][];
        for(int l = 0; l < line; l++) {
            var derrow = new string[row];
            for (int r = 0; r < row; r++) {
                derrow[r] = value[l, r];
            }
            arrays[l] = derrow;
        }
        AppendTable(arrays);
        return this;
    }

    /// <param name="line">行数</param>
    /// <returns></returns>
    public Generator AppendTable(List<string> value, int row)
    {
        if (value == null)
            throw new Exception("value是null");
        if (row <= 0)
            throw new ArgumentException("列数必须大于 0", nameof(row));
        if (value.Count == 0)
            return this; // 空列表，不生成表格

        var line = value.Count / row;
    
        string[][] array = new string[line][];
        for(int l = 0; l < line; l++) {
            var rowArray = new string[row];
            for(int r = 0; r < row; r++) {
                rowArray[r] = value[l * row + r];
            }
            array[l] = rowArray;
        }
        AppendTable(array);
        return this;
    }

    public Generator AppendText(string value)
    {
        markdownValue.AppendLine();
        markdownValue.Append(value);
        return this;
    }

    public Generator AppendLine()
    {
        markdownValue.AppendLine();
        return this;
    }
}
