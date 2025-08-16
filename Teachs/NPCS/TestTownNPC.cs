using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TeachMod.Teachs.NPCS;

[AutoloadHead]
public class TestTownNPC : ModNPC
{
    private AIStatus Status = AIStatus.Wandering;
    public override LocalizedText DisplayName => base.DisplayName;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 3; //贴图有3帧
        NPCID.Sets.ExtraFramesCount[Type] = 0;

        NPCID.Sets.AttackFrameCount[Type] = 4; //攻击帧
        NPCID.Sets.AttackType[Type] = (int)AttackType.Ranged;
        NPCID.Sets.AttackTime[Type] = 30;           //攻击时间
        NPCID.Sets.AttackAverageChance[Type] = 10;  //攻击概率
        NPCID.Sets.DangerDetectRange[Type] = 300;   //威胁半径

        NPCID.Sets.HatOffsetY[Type] = 0; //帽子偏移
        NPCID.Sets.MagicAuraColor[Type] = Color.Black;

        //NPC图鉴
        NPCID.Sets.TownNPCBestiaryPriority.Add(Type); //添加城镇NPC图鉴
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Velocity = 1f
        })
        ;


        NPC.Happiness
            .SetBiomeAffection<JungleBiome>(AffectionLevel.Like)  //生物群落喜好
            .SetNPCAffection(NPCID.Guide, AffectionLevel.Like)   //NPC喜好
            ;
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.lifeMax = 666;
        //AnimationType = NPCID.Guide;

        base.SetDefaults();
    }

    private Func<int, bool> FrameAI;
    private Func<bool> SuitAI;
    private int statusMaxTime = 300;
    private int currentStatusTime = 0;
    private int frameTime = 20;
    private int statusMax = Enum.GetValues<AIStatus>().Select(f => (int)f).Max();
    public override void AI()
    {
        if(currentStatusTime >= statusMaxTime) {
            currentStatusTime = 0;
            var newStatus = Main.rand.Next(0, statusMax);
            if(newStatus == (int)AIStatus.Wandering) {
                ref Vector2 velocity = ref NPC.velocity;
                var velocityX = Main.rand.NextFloat(-3f, 3f);
                velocity.X = velocityX == 0f ? 3f : (int)velocityX * 3f;

                FrameAI = (frameHeight) => {
                    var npcTexture = TextureAssets.Npc[Type];
                    var countFPS = Main.npcFrameCount[Type];
                    var currentFPS = NPC.frame.Y / frameHeight; //当前帧
                    ref Rectangle npcFrame = ref NPC.frame;
                    if (currentFPS >= countFPS) {
                        npcFrame.Y = 0;
                    }
                    if (currentStatusTime % frameTime == 0) {
                        npcFrame.Y += frameHeight;
                    }
                    //Main.NewText("NPC Frame = " + NPC.frame + "  FrameHeight = " + frameHeight);
                    return true;
                };
            } else {
                FrameAI = (f) => true;
                ref Vector2 velocity = ref NPC.velocity;
                velocity.X = 0f;
            }
        }
        currentStatusTime++;
        base.AI();
    }

    /// <summary>
    /// frameHeight的高度是单个帧的高度
    /// </summary>
    public override void FindFrame(int frameHeight)
    {
        FrameAI?.Invoke(frameHeight);

        base.FindFrame(frameHeight);
    }

    public override void AddShops()
    {
        var 新手商店 = new NPCShop(Type, "one");
        var condition = new Condition(Language.GetOrRegister("NullKey", () => "空"), () => true);
        新手商店.Add(new NPCShop.Entry(ItemID.WaffleIron, condition));
        新手商店.Register();


        var 中期商店 = new NPCShop(Type, "two");
        var shopItem = new Item();
        shopItem.SetDefaults(ItemID.WoodenArrow);
        shopItem.value = Item.buyPrice(1,1,1,1);
        shopItem.stack = 20;
        var entry = new NPCShop.Entry(shopItem, condition);
        entry.AddShopOpenedCallback((item, npc) => {
            item.stack = 20;
        });
        中期商店.Add(entry);
        中期商店.Register();
        base.AddShops();
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        switch (cushop) {
            case 1:
                button = "新手商店";
                break;
            case 2:
                button = "中期商店";
                break;
        }
        button2 = "切换商店";
        base.SetChatButtons(ref button, ref button2);
    }

    private int cushop = 1;
    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton) {
            cushop++;
            if (cushop >= 3)
                cushop = 1;
        }
        if (firstButton) {
            if (cushop == 1) {
                shopName = "one";
            }
            if (cushop == 2) {
                shopName = "two";
            }
        }
        base.OnChatButtonClicked(firstButton, ref shopName);
    }

    private enum AIStatus
    {
        Wandering,
        StayPut
    }

    public enum AttackType
    {
        Default = -1,
        Ranged = 1,
        Magic = 2,
        Melee = 3,
    }
}
