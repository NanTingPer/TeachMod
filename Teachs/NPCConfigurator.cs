using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Teachs;

public class NPCConfigurator(ModNPC npc)
{
    private readonly ModNPC thisNPC = npc;

    public static NPCConfigurator CreateUtil(ModNPC npc)
    {
        return new NPCConfigurator(npc);
    }

    /// <summary>
    /// NPC帧图数量 <see cref="Main.npcFrameCount"/>
    /// </summary>
    public NPCConfigurator FrameCount(int frameCount)
    {
        Main.npcFrameCount[thisNPC.Type] = frameCount;
        return this;
    }

    /// <summary>
    /// 额外帧数量 <see cref="NPCID.Sets.ExtraFramesCount"/>
    /// </summary>
    public NPCConfigurator ExtraFramesCount(int extraCount)
    {
        NPCID.Sets.ExtraFramesCount[thisNPC.Type] = extraCount;
        return this;
    }

    /// <summary>
    /// 攻击帧数量 <see cref="NPCID.Sets.AttackFrameCount"/>
    /// </summary>
    public NPCConfigurator AttackFrameCount(int frameCount)
    {
        NPCID.Sets.AttackFrameCount[thisNPC.Type] = frameCount;
        return this;
    }

    /// <summary>
    /// 攻击类型 <see cref="NPCID.Sets.AttackType"/>
    /// </summary>
    public NPCConfigurator AttackType(int type)
    {
        NPCID.Sets.AttackType[thisNPC.Type] = type;
        return this;
    }

    /// <summary>
    /// 攻击类型 <see cref="NPCID.Sets.AttackType"/>
    /// </summary>
    public NPCConfigurator AttackType(AttackTypes type)
    {
        NPCID.Sets.AttackType[thisNPC.Type] = (int)type;
        return this;
    }

    /// <summary>
    /// 攻击间隔 <see cref="NPCID.Sets.AttackTime"/>
    /// </summary>
    public NPCConfigurator AttackTime(int time)
    {
        NPCID.Sets.AttackTime[thisNPC.Type] = time;
        return this;
    }

    /// <summary>
    /// 攻击概率 <see cref="NPCID.Sets.AttackAverageChance"/>
    /// </summary>
    public NPCConfigurator AttackAverageChance(int time)
    {
        NPCID.Sets.AttackAverageChance[thisNPC.Type] = time;
        return this;
    }

    /// <summary>
    /// 威胁半径 <see cref="NPCID.Sets.DangerDetectRange"/>
    /// </summary>
    public NPCConfigurator DangerDetectRange(int range)
    {
        NPCID.Sets.DangerDetectRange[thisNPC.Type] = range;
        return this;
    }

    /// <summary>
    /// 帽子Y偏移 <see cref="NPCID.Sets.HatOffsetY"/>
    /// </summary>
    public NPCConfigurator HatOffsetY(int offset)
    {
        NPCID.Sets.HatOffsetY[thisNPC.Type] = offset;
        return this;
    }

    /// <summary>
    /// 法术攻击光芒颜色 <see cref="NPCID.Sets.MagicAuraColor"/>
    /// </summary>
    public NPCConfigurator MagicAuraColor(Color color)
    {
        NPCID.Sets.MagicAuraColor[thisNPC.Type] = color;
        return this;
    }

    /// <summary>
    /// 添加到TownNPC图鉴 <see cref="NPCID.Sets.TownNPCBestiaryPriority"/>
    /// </summary>
    /// <returns></returns>
    public NPCConfigurator AddTownNPC()
    {
        NPCID.Sets.TownNPCBestiaryPriority.Add(thisNPC.Type);
        return this;
    }

    /// <summary>
    /// 设置NPC图鉴绘制状态 <see cref="NPCID.Sets.NPCBestiaryDrawOffset"/>
    /// </summary>
    public NPCConfigurator NPCBestiaryDrawOffset(NPCID.Sets.NPCBestiaryDrawModifiers nbm)
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(thisNPC.Type, nbm);
        return this;
    }

    /// <summary>
    /// 设置环境爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// </summary>
    public NPCConfigurator SetBiomeAffection<T>(AffectionLevel level) where T : class, IShoppingBiome, ILoadable
    {
        thisNPC.NPC.Happiness.SetBiomeAffection<T>(level);
        return this;
    }

    /// <summary>
    /// 设置环境爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// </summary>
    public NPCConfigurator SetBiomeAffection(IShoppingBiome isb, AffectionLevel level)
    {
        thisNPC.NPC.Happiness.SetBiomeAffection(isb, level);
        return this;
    }

    /// <summary>
    /// 设置NPC爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// <para> 模组NPC请使用这个 </para>
    /// </summary>
    public NPCConfigurator SetNPCAffection<T>(AffectionLevel level) where T : ModNPC
    {
        thisNPC.NPC.Happiness.SetNPCAffection<T>(level);
        return this;
    }

    /// <summary>
    /// 设置NPC爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// <para> <see cref="NPCID"/> </para>
    /// </summary>
    public NPCConfigurator SetNPCAffection(int npcid, AffectionLevel level)
    {
        thisNPC.NPC.Happiness.SetNPCAffection(npcid, level);
        return this;
    }

    public enum AttackTypes
    {
        Default = -1,
        Ranged = 1,
        Magic = 2,
        Melee = 3,
    }
}
