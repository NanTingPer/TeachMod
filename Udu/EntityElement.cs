#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TeachMod.Udu;

public class EntityElement : UIElement
{
    private ElementList<TestNode> Nodes { get; init; }
    private ElementNode? _currentNode;
    private Vector2 _mouseOffset;
    public EntityElement()
    {
        Nodes = new ElementList<TestNode>();
        Nodes.Name = "EntityElementNodes";
        //Nodes.Width = 200;
        Nodes.ItemClickEvent += SetCurrentNode;
        Nodes.MouseHover += NodeMouseHoverTest;
        MouseHover += MoveNode;
        Nodes.Active = true;
        ((UIElement)this).Append(Nodes);
    }

    private void NodeMouseHoverTest(UIMouseEventArgs eventObj)
    {
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        _currentNode?.VirDraw(spriteBatch, Main.MouseScreen);
    }

    private void MoveNode(UIMouseEventArgs eventObj)
    {
        //if (!Main.mouseLeftRelease) {
        //    _currentNode = null;
        //}
    }

    private void SetCurrentNode(ElementList<TestNode>.ItemClickEventArgs obj)
    {
        _currentNode = obj.CuEntity;
        _mouseOffset = obj.Entitys.MouseOffset;
        Main.NewText("HHHHHHH");
    }
    
    public void Append(TestNode node)
    {
        Nodes.Append(node);
    }

    public void Remove(TestNode node)
    {
        Nodes.Remove(node);
    }
}

public abstract class ElementNode : UIElement
{
    public new abstract string Name { get; set; }
    /// <summary>
    /// 临时高度
    /// </summary>
    public float VirHeight { get; set; }
    /// <summary>
    /// 临时宽度
    /// </summary>
    public float VirWidth { get; set; }
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// 绘制选中并拖动此元素的虚
    /// 退出方法时候 应保持 <see cref="SpriteBatch.Begin()"/>
    /// </summary>
    public virtual void VirDraw(SpriteBatch spriteBatch, Vector2 position)
    {
        var virDrawRectangle = new Rectangle((int)position.X, (int)position.Y, (int)VirWidth, (int)VirHeight);
        spriteBatch.Draw(Texture, virDrawRectangle, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
    }
}

public class TestNode : ElementNode
{
    public TestNode()
    {
        VirHeight = 200;
        VirWidth = 200;
    }
    public Color Background { get; set; } = Color.White;
    public override string Name { get; set; } = "Test";
    public override float Height { get => 200; set => base.Height = value; }
    public override void VirDraw(SpriteBatch spriteBatch, Vector2 position)
    {
        var virDrawRectangle = new Rectangle((int)position.X, (int)position.Y, (int)VirWidth, (int)VirHeight);
        spriteBatch.Draw(Texture, virDrawRectangle, null, Background, 0f, Vector2.Zero, SpriteEffects.None, 1f);
    }
}

public class ElementStyle
{
    public float Height { get; set; }
    public float Width { get; set; }
    public float TopPadding { get; set; }
    public float LeftPadding { get; set; }
}