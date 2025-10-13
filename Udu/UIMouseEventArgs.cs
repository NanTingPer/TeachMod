using Microsoft.Xna.Framework;

namespace TeachMod.Udu;

public readonly struct UIMouseEventArgs(UIElement orig, Vector2 mousePosition)
{
    public UIElement Element { get; init; } = orig;
    public Vector2 MousePosition { get; init; } = mousePosition;
}

public delegate void UIMouseEvent(UIMouseEventArgs eventObj);
