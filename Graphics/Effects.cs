
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace TeachMod.Graphics;

public class Effects : ModSystem
{
    public static Asset<Effect> Bloom { get; private set; }
    public static Asset<Effect> AlphaTestEffect { get; private set; }
    public override void Load()
    {
        Bloom = LoadEffect(nameof(Bloom));
        AlphaTestEffect = LoadEffect(nameof(AlphaTestEffect));
    }

    public static Asset<Effect> LoadEffect(string name)
    {
        return ModContent.Request<Effect>($"TeachMod/Graphics/Content/{name}");
    }
}
