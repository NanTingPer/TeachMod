using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace TeachMod.Teachs.Items.Accessorys;
public class ImmempAccessory : ModItem
{
    public override void SetDefaults()
    {
        Item.accessory = true;
        base.SetDefaults();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        var modPlayer = player.GetModPlayer<ImmempAccessoryPlayer>();
        modPlayer.ImmempAccessory = true;
        base.UpdateAccessory(player, hideVisual);
    }
}

public class ImmempAccessoryPlayer : ModPlayer
{
    public bool ImmempAccessory = false;
    public List<Action> PostUpdateExter = [];

    public override void ResetInfoAccessories()
    {
        ImmempAccessory = false;
        base.ResetInfoAccessories();
    }

    public override void PostUpdate()
    {
        PostUpdateExter.ForEach(ac => ac?.Invoke());
        PostUpdateExter.Clear();
        base.PostUpdate();
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        //带着饰品 无敌帧 +1s 由于在无敌帧计算前执行
        //所以给予PostUpdate去实际上加成
        if (ImmempAccessory) {
            PostUpdateExter.Add(() => Player.immuneTime += 60);
        }
        base.ModifyHitByNPC(npc, ref modifiers);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (ImmempAccessory) {
            var modNPC = target.GetGlobalNPC<ImmempAccessoryNPC>();
            var iahm = modNPC.immempAccessoryTime.FirstOrDefault(iahm => iahm.PlayerIndex == Player.whoAmI);
            
            //本NPC第一次被携带饰品的玩家击中
            //为其添加一个用于计算的模型
            if (iahm == null) {
                iahm = new ImmempAccessoryHitNPCModel()
                {
                    PlayerIndex = Player.whoAmI,
                    StartTime = DateTime.Now,
                    NewTime = DateTime.Now,
                };
                modNPC.immempAccessoryTime.Add(iahm);
            } else {
                iahm.NewTime = DateTime.Now; //更新时间
            }

            //如果时间差值达到1s (已经一秒没击中这个NPC了) 重置时间
            if (iahm.NewSubNow > 1f) {
                iahm.Reset();
            }

            //如果时间差能来到5秒，那么本次(源)伤害 * 100%
            //并且重置时间计算
            if (iahm.StartSubNew > 5f) {
                modifiers.SourceDamage *= 1f + 1f; //伤害源增加100%
                iahm.Reset();
            }
        }

        base.ModifyHitNPC(target, ref modifiers);
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        base.OnHitByNPC(npc, hurtInfo);
    }
}

public class ImmempAccessoryNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public List<ImmempAccessoryHitNPCModel> immempAccessoryTime = [];
}

public class ImmempAccessoryHitNPCModel
{
    public int PlayerIndex;
    public DateTime StartTime;
    public DateTime NewTime;

    /// <summary>
    /// 第一次击中的时间，和最后一次击中时间的差值
    /// </summary>
    public double StartSubNew { get => Math.Abs((NewTime - StartTime).TotalSeconds); }
    
    /// <summary>
    /// 新时间和现在时间的差值
    /// </summary>
    public double NewSubNow { get => Math.Abs((DateTime.Now - NewTime).TotalSeconds); }

    /// <summary>
    /// 重置时间
    /// </summary>
    public void Reset()
    {
        StartTime = DateTime.Now;
        NewTime = DateTime.Now;
    }
}
