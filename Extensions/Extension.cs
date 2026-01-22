using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace TeachMod.Extensions;

public static partial class Extension
{
    /// <summary>
    /// <para> 如果开始 则End </para>
    /// <para> <see cref="BlendState"/> : <see cref="BlendState.AlphaBlend"/> </para>
    /// <para> <see cref="SamplerState"/> : <see cref="SamplerState.LinearWrap"/> </para>
    /// <para> <see cref="DepthStencilState"/> : <see cref="DepthStencilState.None"/> </para>
    /// <para> <see cref="RasterizerState"/> : <see cref="RasterizerState.CullNone"/> </para>
    /// </summary>
    /// <param name="sbatch"></param>
    /// <param name="eff"></param>
    /// <param name="matrix"></param>
    public static void SafeBegin(this Microsoft.Xna.Framework.Graphics.SpriteBatch sbatch, Effect eff, Matrix? matrix = null)
    {
        if (matrix == null)
            matrix = Matrix.Identity;
        if (sbatch.IsCall()) {
            sbatch.End();
        }
        sbatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,DepthStencilState.None, RasterizerState.CullNone, eff, matrix.Value);
    }

    public static void SafeBegin(this Microsoft.Xna.Framework.Graphics.SpriteBatch sbatch)
    {
        SafeBegin(sbatch, null ,null);
    }

    public static void SafeEnd(this Microsoft.Xna.Framework.Graphics.SpriteBatch sbatch)
    {
        if (sbatch.IsCall()) {
            sbatch.End();
        }
    }

    /// <summary>
    /// 判断画笔是否已经被<see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch.Begin()"/>
    /// </summary>
    /// <param name="sbatch"></param>
    /// <returns></returns>
    public static bool IsCall(this Microsoft.Xna.Framework.Graphics.SpriteBatch sbatch) => beginCalled(sbatch);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "beginCalled")]
    private static extern ref bool beginCalled(Microsoft.Xna.Framework.Graphics.SpriteBatch sprite);
}