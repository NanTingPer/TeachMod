using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using TeachMod.Items.TileCopys;
using Terraria.DataStructures;

namespace TeachMod.Items;
public class TileCopyItemType : ModItem
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Ale;
    public override void SetDefaults()
    {
        int a = 0;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.autoReuse = false;
        Item.useTime = 1;
        Item.useAnimation = 1;
        base.SetDefaults();
    }

    private int statu = 0;
    private Vector2 left = default;
    private Vector2 right = default;
    private List<MyTileData> _tiles = [];
    public override bool? UseItem(Player player)
    {
        if(statu == 0) {
            left = Main.MouseWorld;
        }else if(statu == 1) {
            right = Main.MouseWorld;
        } else if (statu == 2) {
            _tiles = GetTiles(left, right);
        } else if(statu == 3) {
            PlaceTiles(Main.MouseWorld, _tiles);
        }

        statu += 1;
        if(statu >= 4) {
            statu = 0;
        }
        return base.UseItem(player);
    }


    public static void PlaceTiles(Vector2 sourcePos, IEnumerable<MyTileData> tiles)
    {
        var soureTilePos = sourcePos.ToTileCoordinates16();
        foreach (var tile in tiles) {
            var tarPos = soureTilePos + tile.Offset;
            //WorldGen.PlaceTile(tarPos.X, tarPos.Y, tile.TileType.Type);
            var tileTypeDatas = Main.tile.GetData<TileTypeData>();
            ref var typedata = ref tileTypeDatas[(uint)(tarPos.Y + (tarPos.X * Main.tile.Height))];
            typedata.Type = tile.TileType.Type;
            WorldGen.SquareTileFrame(tarPos.X, tarPos.Y);
            WorldGen.PlaceWall(tarPos.X, tarPos.Y, tile.WallType.Type);
        }
    }

    public static List<MyTileData> GetTiles(Vector2 leftUpPointWorld, Vector2 rightDownPointWorld)
    {
        var leftPonit = leftUpPointWorld.ToTileCoordinates16();
        var rightPoint = rightDownPointWorld.ToTileCoordinates16();
        var newLeftPoint = new Point16(
            X: Math.Min(leftPonit.X, rightPoint.X),
            Y: Math.Min(leftPonit.Y, rightPoint.Y));

        var newRightPoint = new Point16(
            X: Math.Max(leftPonit.X, rightPoint.X),
            Y: Math.Max(leftPonit.Y, rightPoint.Y));

        Point16 tarPoint = newRightPoint - newLeftPoint;


        List<MyTileData> tiles = [];
        for (int x = 0; x < tarPoint.X; x++) {
            for(int y = 0; y < tarPoint.Y; y++) {
                Tile tile = Main.tile[leftPonit + new Point16(x, y)];

                if (!tile.HasTile)
                    continue;

                var mytiledata = new MyTileData()
                {
                    Offset = new Point16(x, y),
                    TileType = tile.Get<TileTypeData>(),
                    WallTileData = tile.Get<TileWallWireStateData>(),
                    WallType = tile.Get<WallTypeData>()
                };
                tiles.Add(mytiledata);
            }
        }

        return tiles;
    }

    public class MyTileData
    {
        public Point16 Offset { get; set; }

        public TileTypeData TileType { get; set; }

        public WallTypeData WallType { get; set; }

        public TileWallWireStateData WallTileData { get; set; }
    }
}

