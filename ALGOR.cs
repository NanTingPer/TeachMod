#nullable enable
using System;
using Terraria.Localization;

namespace TeachMod;

public static class ALGOR
{
    public static LocalizedText GetLocalTextValue(this Type type, string name, string? defaultValue)
    {
        defaultValue ??= name;
        string keyName = $"Mods.{type.Namespace!.Split('.')[0]}.{type.FullName!.Split('.')[1..]}.{name}";
        return Language.GetOrRegister(keyName, () => defaultValue);
    }
}
