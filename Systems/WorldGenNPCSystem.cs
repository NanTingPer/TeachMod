using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.ID;

namespace TeachMod.Systems;

public class WorldGenNPCSystem : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        var task = tasks.FindIndex(f => f.Name == "Guide"); //向导
        tasks.Insert(task, new WorldGenNPCSystemGenPass("SpawnDemolitionist", 1.0));
        base.ModifyWorldGenTasks(tasks, ref totalWeight);
    }

    public override void PreWorldGen()
    {
        base.PreWorldGen();
    }

    public class WorldGenNPCSystemGenPass : GenPass
    {
        public WorldGenNPCSystemGenPass(string name, double loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int entityId = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, NPCID.Demolitionist);
            Main.npc[entityId].homeTileX = Main.spawnTileX;
            Main.npc[entityId].homeTileY = Main.spawnTileY;
            Main.npc[entityId].direction = 1;
            Main.npc[entityId].homeless = true;
        }
    }
}
