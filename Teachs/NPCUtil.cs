using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Teachs;

public class NPCUtil(ModNPC npc)
{
    private readonly ModNPC thisNPC = npc;

    public static NPCUtil CreateUtil(ModNPC npc)
    {
        return new NPCUtil(npc);
    }

    /// <summary>
    /// NPC帧图数量 <see cref="Main.npcFrameCount"/>
    /// </summary>
    public NPCUtil FrameCount(int frameCount)
    {
        Main.npcFrameCount[thisNPC.Type] = frameCount;
        return this;
    }

    /// <summary>
    /// 额外帧数量 <see cref="NPCID.Sets.ExtraFramesCount"/>
    /// </summary>
    public NPCUtil ExtraFramesCount(int extraCount)
    {
        NPCID.Sets.ExtraFramesCount[thisNPC.Type] = extraCount;
        return this;
    }

    /// <summary>
    /// 攻击帧数量 <see cref="NPCID.Sets.AttackFrameCount"/>
    /// </summary>
    public NPCUtil AttackFrameCount(int frameCount)
    {
        NPCID.Sets.AttackFrameCount[thisNPC.Type] = frameCount;
        return this;
    }

    /// <summary>
    /// 攻击类型 <see cref="NPCID.Sets.AttackType"/>
    /// </summary>
    public NPCUtil AttackType(int type)
    {
        NPCID.Sets.AttackType[thisNPC.Type] = type;
        return this;
    }

    /// <summary>
    /// 攻击类型 <see cref="NPCID.Sets.AttackType"/>
    /// </summary>
    public NPCUtil AttackType(AttackTypes type)
    {
        NPCID.Sets.AttackType[thisNPC.Type] = (int)type;
        return this;
    }

    /// <summary>
    /// 攻击间隔 <see cref="NPCID.Sets.AttackTime"/>
    /// </summary>
    public NPCUtil AttackTime(int time)
    {
        NPCID.Sets.AttackTime[thisNPC.Type] = time;
        return this;
    }

    /// <summary>
    /// 攻击概率 <see cref="NPCID.Sets.AttackAverageChance"/>
    /// </summary>
    public NPCUtil AttackAverageChance(int time)
    {
        NPCID.Sets.AttackAverageChance[thisNPC.Type] = time;
        return this;
    }

    /// <summary>
    /// 威胁半径 <see cref="NPCID.Sets.DangerDetectRange"/>
    /// </summary>
    public NPCUtil DangerDetectRange(int range)
    {
        NPCID.Sets.DangerDetectRange[thisNPC.Type] = range;
        return this;
    }

    /// <summary>
    /// 帽子Y偏移 <see cref="NPCID.Sets.HatOffsetY"/>
    /// </summary>
    public NPCUtil HatOffsetY(int offset)
    {
        NPCID.Sets.HatOffsetY[thisNPC.Type] = offset;
        return this;
    }

    /// <summary>
    /// 法术攻击光芒颜色 <see cref="NPCID.Sets.MagicAuraColor"/>
    /// </summary>
    public NPCUtil MagicAuraColor(Color color)
    {
        NPCID.Sets.MagicAuraColor[thisNPC.Type] = color;
        return this;
    }

    /// <summary>
    /// 添加到TownNPC图鉴 <see cref="NPCID.Sets.TownNPCBestiaryPriority"/>
    /// </summary>
    /// <returns></returns>
    public NPCUtil AddTownNPC()
    {
        NPCID.Sets.TownNPCBestiaryPriority.Add(thisNPC.Type);
        return this;
    }

    /// <summary>
    /// 设置NPC图鉴绘制状态 <see cref="NPCID.Sets.NPCBestiaryDrawOffset"/>
    /// </summary>
    public NPCUtil NPCBestiaryDrawOffset(NPCID.Sets.NPCBestiaryDrawModifiers nbm)
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(thisNPC.Type, nbm);
        return this;
    }

    /// <summary>
    /// 设置环境爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// </summary>
    public NPCUtil SetBiomeAffection<T>(AffectionLevel level) where T : class, IShoppingBiome, ILoadable
    {
        thisNPC.NPC.Happiness.SetBiomeAffection<T>(level);
        return this;
    }

    /// <summary>
    /// 设置环境爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// </summary>
    public NPCUtil SetBiomeAffection(IShoppingBiome isb, AffectionLevel level)
    {
        thisNPC.NPC.Happiness.SetBiomeAffection(isb, level);
        return this;
    }

    /// <summary>
    /// 设置NPC爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// <para> 模组NPC请使用这个 </para>
    /// </summary>
    public NPCUtil SetNPCAffection<T>(AffectionLevel level) where T : ModNPC
    {
        thisNPC.NPC.Happiness.SetNPCAffection<T>(level);
        return this;
    }

    /// <summary>
    /// 设置NPC爱好 <see cref="ModNPC.NPC.Happoncess"/>
    /// <para> <see cref="NPCID"/> </para>
    /// </summary>
    public NPCUtil SetNPCAffection(int npcid, AffectionLevel level)
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
