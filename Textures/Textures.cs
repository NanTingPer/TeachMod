using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;

namespace TeachMod.Textures;

public static class Textures
{
    public static Asset<Texture2D> Cusp { get; private set; } = GetTexture(nameof(Cusp).ToLower());

    public static Asset<Texture2D> GetTexture(string name)
    {
        return ModContent.Request<Texture2D>($"TeachMod/Textures/{name}");
    }
}
