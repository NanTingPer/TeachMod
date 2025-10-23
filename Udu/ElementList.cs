#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace TeachMod.Udu;


public class ElementList<Entity> : UIElement where Entity : class
{
    //public ListUI(Func<Entity, string> drawfunc)
    //{

    //}
    public ElementList()
    {
        MouseClick += ItemClickEventAction;
    }

    /// <summary>
    /// 当前单个对象的大小
    /// </summary>
    public Vector2 ItemSize { get; private set; }
    public event Action<ItemClickEventArgs>? ItemClickEvent;
    /// <summary>
    /// 本UI承载的全部对象
    /// </summary>
    private readonly List<Entity> entitys = [];
    /// <summary>
    /// 每个元素的宽度
    /// </summary>
    private int ItemWidth => (int)ItemSize.X;
    /// <summary>
    /// 每个元素的高度
    /// </summary>
    private int ItemHeight => (int)ItemSize.Y;
    /// <summary>
    /// 每个元素垂直方向的间距
    /// </summary>
    public int ItemVerticalSpacing { get; set; }

    private float height = 0f;

    //public override float Height { get { _ = field; return height; } set; }
    /// <summary>
    /// 本UI当前的高度 每次<see cref="Append(Entity)"/> or <see cref="Remove(Entity)"/> 都会重计算
    /// </summary>
    public override float Height { get => height; set => height = value; }

    private float width = 0f;
    //public new float Width { get {/* _ = field;*/ return width; } private set => width = value; }

    /// <summary>
    /// 本UI当前的宽度
    /// </summary>
    public override float Width { get => width; set => width = value; }

    /// <summary>
    /// <code>
    /// Vector2.Transform(new Vector2(height, width), Main.UIScaleMatrix)
    /// </code>
    /// </summary>
    public Vector2 HeightWidth => Vector2.Transform(new Vector2(height, width), Main.UIScaleMatrix);

    public sealed override void DrawSelf(SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        foreach (var item in entitys) {
            ReLogic.Graphics.DynamicSpriteFontExtensionMethods.DrawString(
                spriteBatch,
                FontAssets.MouseText.Value,
                item?.ToString() ?? "",
                GetItemPosition(item!),
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
    public ElementList<Entity> Append(Entity entity)
    {
        entitys.Add(entity);

        #region 计算单个元素的宽高
        var entitySize = FontAssets.MouseText.Value.MeasureString(entity.ToString());
        var newSize = Vector2.Zero;
        newSize.X = entitySize.X > ItemSize.X ? entitySize.X : ItemSize.X;
        newSize.Y = entitySize.Y > ItemSize.Y ? entitySize.Y : ItemSize.Y;
        ItemSize = newSize;
        #endregion

        height = ItemHeight * entitys.Count; //高度
        width = newSize.X;
        return this;
    }

    /// <summary>
    /// 删除实体列表中的元素
    /// <para> 删除后会重写计算此元素的宽度 </para>
    /// </summary>
    public ElementList<Entity> Remove(Entity entity)
    {
        entitys.Remove(entity);
        height = ItemHeight * entitys.Count;
        return this;
    }

    /// <summary>
    /// 此Element被点击后调用，如果此次点击 点到了元素则触发<see cref="ItemClickEvent"/>
    /// </summary>
    /// <param name="args"></param>
    private void ItemClickEventAction(UIMouseEventArgs args)
    {
        IsMouseClickItem(args.MousePosition);
    }

    /// <summary>
    /// 鼠标是否点击元素（调用此元素的点击事件）
    /// </summary>
    private Entity? IsMouseClickItem(Vector2 mousePosition)
    {
        //1. 计算变换后的UI大小
        //2. 遍历列表元素 并判断鼠标位置是否进行碰撞
        //鼠标位置使用 Main.GameViewMatrix.ZoomMatrix 矩阵
        //列表元素统一使用 Main.UIScaleMatrix

        //TODO 绘制使用了矩阵 这里计算也要 不然会偏移
        var newv2 = Vector2.Transform(new Vector2(ItemWidth, ItemHeight), Main.UIScaleMatrix);

        var rectangle = new Rectangle(0, 0, (int)newv2.X, (int)newv2.Y);
        foreach (var item in entitys) {
            //绘制位置未应用矩阵，因为在Begin中已经指定
            //如果要将未应用矩阵的元素与应用了矩阵的元素比较，会造成偏移
            //消除偏移 需要手动应用矩阵
            var itemPosition = GetItemPosition(item);
            itemPosition = Vector2.Transform(itemPosition, Main.UIScaleMatrix);
            rectangle.X = (int)itemPosition.X;
            rectangle.Y = (int)itemPosition.Y;
#if DEBUG
            TeachMod.Mod!.Logger.Debug($"{rectangle} {MouseRectangle}");
#endif
            if (CheckAABBvAABBCollision(rectangle, MouseRectangle)) {
                ItemClickEvent?.Invoke(new ItemClickEventArgs(item, this, mousePosition));
            }
        }
        return null;
    }

    public static bool CheckAABBvAABBCollision(Rectangle dimensions1x, Rectangle dimensions2x)
    {
        Vector2 position1 = new Vector2(dimensions1x.X, dimensions1x.Y);
        Vector2 dimensions1 = new Vector2(dimensions1x.Width, dimensions1x.Height);
        Vector2 position2 = new Vector2(dimensions2x.X, dimensions2x.Y);
        Vector2 dimensions2 = new Vector2(dimensions2x.Width, dimensions2x.Height);
        if (position1.X < position2.X + dimensions2.X && position1.Y < position2.Y + dimensions2.Y && position1.X + dimensions1.X > position2.X)
            return position1.Y + dimensions1.Y > position2.Y;

        return false;
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
        //1. 先计算此元素在列表中的索引 然后根据索引计算其左上角位置
        _ = Main.UIScaleMatrix;

        //数量 * 元素高度 + 元素间距
        var itemOffset = new Vector2(0, index * ItemHeight + ItemVerticalSpacing); //TODO 现在X是没变的
        var origOffset = new Vector2(LeftPadding, TopPadding);

        return origOffset + itemOffset ;//origOffset + itemOffset;
    }
    public class ItemClickEventArgs(Entity entity, ElementList<Entity> entitys, Vector2 pos)
    {
        /// <summary>
        /// 当前被点击的实体
        /// </summary>
        public Entity CuEntity { get; init; } = entity;
        public ElementList<Entity> Entitys { get; init; } = entitys;
        public Vector2 Position { get; init; } = pos;
    }
}
