#pragma warning disable CA2255
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TeachMod.Nets;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class NPCNetAttribute : Attribute;
public sealed class NPCAutoNet : ANet<ModNPC, NPCNetAttribute>
{
    
}


/// <summary>
/// <see cref="GlobalNPC"/> 的自动同步类
/// </summary>
public sealed class GlobalNPCAutoNet : ANet<GlobalNPC, NPCNetAttribute>
{
    private static List<Hook> hks = [];
    [ModuleInitializer]
    internal static void InitNPCAutoNet()
    {
        foreach (var netType in tarType) {
            if (_netFieldInfo[netType].Count != 0 || _netPropInfo[netType].Count != 0) {
                var sendMethod = netType.GetMethod(nameof(GlobalNPC.SendExtraAI));
                var receiveMethod = netType.GetMethod(nameof(GlobalNPC.ReceiveExtraAI));

                hks.Add(new Hook(sendMethod, SendExtraAIHook));
                hks.Add(new Hook(receiveMethod, ReceiveExtraAIHook));
            }
        }
    }

    private delegate void SendExtraAI(object gnpc, NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter);
    private static void SendExtraAIHook(SendExtraAI orig, object gnpc, NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        if (_netFieldInfo.TryGetValue(gnpc.GetType(), out var fields)) {
            foreach (var fieldInfo in fields) {
                try {
                    binaryWriter.Write(fieldInfo, fieldInfo.GetValue(gnpc));
                } catch {
                }
            }
        }
        if (_netPropInfo.TryGetValue(gnpc.GetType(), out var props)) {
            foreach (var propInfo in props) {
                binaryWriter.Write(propInfo, propInfo.GetValue(gnpc));
            }
        }
        orig.Invoke(gnpc, npc, bitWriter, binaryWriter);
    }

    private delegate void ReceiveExtraAI(object gnpc, NPC npc, BitReader bitReader, BinaryReader binaryReader);
    private static void ReceiveExtraAIHook(ReceiveExtraAI orig, object gnpc, NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        //binaryReader.BaseStream.Position = 0;
        var gtype = gnpc.GetType();
        if (_netFieldInfo.TryGetValue(gtype, out var fields)) {
            foreach (var field in fields) {
                _ = binaryReader.Read(field, gnpc);
            }
        }
        if (_netPropInfo.TryGetValue(gtype, out var props)) {
            foreach (var propinfo in props) {
                binaryReader.Read(propinfo, gnpc);
            }
        }
        orig.Invoke(gnpc, npc, bitReader, binaryReader);
    }
}
