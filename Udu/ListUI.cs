using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Udu;

public class ListUI<Entity> : UIElement
{
    //public ListUI(Func<Entity, string> drawfunc)
    //{
        
    //}

    /// <summary>
    /// 本UI承载的全部对象
    /// </summary>
    private readonly List<Entity> entitys = [];
    /// <summary>
    /// 每个元素的宽度
    /// </summary>
    public int ItemWidth { get; set; }
    /// <summary>
    /// 每个元素的高度
    /// </summary>
    public int ItemHeight { get; set; }
    /// <summary>
    /// 每个元素垂直方向的间距
    /// </summary>
    public int ItemVerticalSpacing { get; set; }

    private float height = 0f;
    /// <summary>
    /// 本UI当前的高度
    /// </summary>
    public override float Height { get { _ = field; return height; } set; }

    private float width = 0f;
    /// <summary>
    /// 本UI当前的宽度
    /// </summary>
    public override float Width { get { _ = field; return width; } set; }
    public sealed override void DrawSelf(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        foreach (var item in entitys) {
            ReLogic.Graphics.DynamicSpriteFontExtensionMethods.DrawString(
                spriteBatch,
                FontAssets.MouseText.Value,
                item.ToString(),
                GetItemPosition(item),
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                1f
                );
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
    }

    /// <summary>
    /// 往实体列表添加内容
    /// <para> 添加后会重新计算此元素的高度 </para>
    /// </summary>
    public ListUI<Entity> Append(Entity entity)
    {
        entitys.Add(entity);
        height = ItemHeight * entitys.Count; //高度
        return this;
    }

    /// <summary>
    /// 删除实体列表中的元素
    /// <para> 删除后会重写计算此元素的宽度 </para>
    /// </summary>
    public ListUI<Entity> Remove(Entity entity)
    {
        entitys.Remove(entity);
        height = ItemHeight * entitys.Count;
        return this;
    }

    /// <summary>
    /// 获取此实体需要绘制的位置
    /// <code>
    /// (index + 1) * ItemHeight + ItemVerticalSpacing
    /// </code>
    /// </summary>
    private Vector2 GetItemPosition(Entity entity) => GetItemPosition(entitys.FindIndex(f => f.Equals(entity)));

    /// <summary>
    /// 获取此索引对应实体绘制的位置
    /// <code>
    /// (index + 1) * ItemHeight + ItemVerticalSpacing
    /// </code>
    /// </summary>
    /// <returns></returns>
    private Vector2 GetItemPosition(int index)
    {
        //数量 * 元素高度 + 元素间距
        var itemOffset = new Vector2(0, index * ItemHeight + ItemVerticalSpacing); //TODO 现在X是没变的
        var origOffset = new Vector2(LeftPadding, TopPadding);
        return origOffset + itemOffset;
    }
}
