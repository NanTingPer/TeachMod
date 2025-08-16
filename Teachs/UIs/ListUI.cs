using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TeachMod.Teachs.UIs;

public class ListUI : UIState
{
    public override void OnInitialize()
    {
        Height.Pixels = 500;
        Width.Pixels = 500;
        var uipanel = new UIPanel();
        uipanel.Height.Set(500, 0);
        uipanel.Width.Set(500, 0);
        Append(uipanel);
        base.OnInitialize();
    }
}
