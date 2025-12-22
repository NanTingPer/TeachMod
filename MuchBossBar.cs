using Mono.Cecil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static TeachMod.MuchBossBarSystem;

namespace TeachMod;

public class MuchBossBar : TeachModSystem
{
}

public class MuchBossBarInfo
{
    public int MaxLife { get; set; }
    public int Life {get; set; }
    public static MuchBossBarInfo IfNotKeyThenCreate<TKey>(IDictionary<TKey, MuchBossBarInfo> dic, TKey tarKey)
    {
        if(dic.TryGetValue(tarKey, out var value)) {
            return value;
        } else {
            var info = new MuchBossBarInfo();
            dic[tarKey] = info;
            return info;
        }
    }
}
public class MuchBossBarSystem : ModSystem
{
    public readonly static Dictionary<int, MuchBossBarInfo> whoAmIAndLife = [];
    public readonly static int[] wroldTheEating = [NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail];
}
public class MuchBossBarGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public NPC RootNPC;
    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        source.BossSpawn(npc, BossSpawn);
        source.NPCParent(npc, NPCParent);

        base.OnSpawn(npc, source);
    }

    
    private static void BossSpawn(NPC npc, EntitySource_BossSpawn spawn)
    {
        var info = MuchBossBarInfo.IfNotKeyThenCreate(whoAmIAndLife, npc.whoAmI);
        info.MaxLife = npc.lifeMax;
    }

    private static void NPCParent(NPC npc, EntitySource_Parent spawn, NPC parentNPC)
    {
        var son = npc.GetGlobalNPC<MuchBossBarGlobalNPC>();
        var parent = parentNPC.GetGlobalNPC<MuchBossBarGlobalNPC>();
        if (son.RootNPC is null && parent.RootNPC is null) { // 如果子NPC没有父，并且父NPC没有父，那么父NPC就是顶级NPC
            son.RootNPC = parentNPC;
        } else {
            son.RootNPC = parent.RootNPC; // 否则父NPC就是子NPC
        }

        if (wroldTheEating.Contains(npc.type)) {
            var info = whoAmIAndLife[parent.RootNPC?.whoAmI ?? parentNPC.whoAmI];
            info.MaxLife += npc.lifeMax;
            info.Life += npc.life;
            Main.NewText(info.MaxLife);
        }
    }
}


public static class MuchBossBarEntitySourceExtension
{
    extension(IEntitySource source)
    {
        /// <param name="npc">传入NPC对应实体</param>
        /// <param name="call">你传入的NPC</param>
        public void BossSpawn(NPC npc, Action<NPC, EntitySource_BossSpawn> call)
        {
            if(source is EntitySource_BossSpawn spawnSource) {
                call(npc, spawnSource);
            }
        }

        /// <param name="sourceNPC"> NPC实体 </param>
        /// <param name="call"> 第一个NPC是你传入的NPC实体，第二个NPC是父实体 </param>

        public void NPCParent(NPC sourceNPC, Action<NPC, EntitySource_Parent, NPC> call)
        {
            if (source is EntitySource_Parent spawnSource && spawnSource.Entity is NPC npc) {
                call(sourceNPC, spawnSource, npc);
            }
        }
    }

    extension(NPC npc)
    {
        /// <param name="call">你传入的NPC</param>
        public void BossSpawn(IEntitySource source, Action<NPC, EntitySource_BossSpawn> call)
        {
            if (source is EntitySource_BossSpawn spawnSource) {
                call(npc, spawnSource);
            }
        }

        /// <param name="sourceNPC"> NPC实体 </param>
        /// <param name="call"> 第一个NPC是你传入的NPC实体，第二个NPC是父实体 </param>

        public void NPCParent(IEntitySource source, Action<NPC, EntitySource_Parent, NPC> call)
        {
            if (source is EntitySource_Parent spawnSource && spawnSource.Entity is NPC parentNPC) {
                call(npc, spawnSource, parentNPC);
            }
        }
    }
}