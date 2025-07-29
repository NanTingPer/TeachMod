using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.Main;

namespace TeachMod.Items.TileCopys;
/// <summary>
/// 物块拷贝
/// </summary>
public class TileCopyItemTwo : ModItem
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TerraBeam;
    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.autoReuse = false;
        Item.useTime = 1;
        Item.useAnimation = 1;
        base.SetDefaults();
    }

    /// <summary>
    /// Point => 希望他是相对于放置的偏移量
    /// MouseWorld.ToTilePos + Point = TilePos
    /// </summary>
    private Dictionary<Point16, (Tile, TileWallWireStateData)> tileBuffer = [];
    private Vector2 up;
    private Vector2 down;
    private Status statu = Status.等待点击;

    public override bool? UseItem(Player player)
    {
        var tilePos = MouseWorld.ToTileCoordinates16();
        var tile = Main.tile[tilePos];
        switch (statu) {
            case Status.等待点击:
                statu = Status.等待左上角;
                Main.NewText("当前状态: 等待左上角");
                break;
            case Status.等待左上角:
                up = MouseWorld;
                statu = Status.等待右下角;
                Main.NewText("当前状态: 等待右下角");
                break;
            case Status.等待右下角:
                down = MouseWorld;
                statu = Status.保存;
                Main.NewText("当前状态: 保存");
                break;
            case Status.保存:
                GetTile(up, down);
                statu = Status.放置;
                Main.NewText("当前状态: 放置");
                break;
            case Status.放置:
                PlaceTiles(MouseWorld);
                statu = Status.等待点击;
                Main.NewText("当前状态: 等待点击");
                tileBuffer.Clear();
                break;
        }
        
        return base.UseItem(player);
    }

    private void PlaceTiles(Vector2 sourcePos)
    {
        List<(Point16, WallTypeData)> walls = [];
        List<(Point16, TileTypeData)> tiles = [];
        List<(Point16, TileWallWireStateData)> states = [];

        foreach (var kv in tileBuffer) {
            walls.Add((kv.Key, kv.Value.Item1.Get<WallTypeData>()));
            tiles.Add((kv.Key, kv.Value.Item1.Get<TileTypeData>()));
            states.Add((kv.Key, kv.Value.Item2));
        }

        List<(Point16, TileTypeData)> tileObject = tiles.Where(f => TileObjectData.GetTileData(f.Item2.Type, 0) != null).ToList();
        tiles = tiles.Except(tileObject).ToList();

        var sourceTilePos = sourcePos.ToTileCoordinates16();

        foreach (var (point, type) in walls) {
            var pos = sourceTilePos + point;
            WorldGen.PlaceWall(pos.X, pos.Y, type.Type);
            WorldGen.SquareWallFrame(pos.X, pos.Y, true);
        }

        foreach (var (point, type) in tiles) {
            var pos = sourceTilePos + point;
            if(WorldGen.PlaceTile(pos.X, pos.Y, type.Type)) {
                var state = states.Find(f => f.Item1 == point);
                var s = Main.tile.GetData<TileWallWireStateData>();
                s[(uint)(pos.Y + (pos.X * Main.tile.Height))] = state.Item2;
                WorldGen.SquareTileFrame(pos.X, pos.Y, true);
            }
        }
    }

    /// <param name="up">世界坐标</param>
    /// <param name="down">世界坐标</param>
    private void GetTile(Vector2 up, Vector2 down)
    {
        var upTile = up.ToTileCoordinates16();
        var downTile = down.ToTileCoordinates16();

        var count = downTile - upTile;

        for (int x = 0; x < count.X; x++) {
            for(int y = 0; y < count.Y; y++) {
                var tile = new Tile();
                ref var refTile = ref tile;
                var offset = new Point16(x, y);
                
                var tarTile = Main.tile[upTile + offset];
                tileBuffer[offset] = (refTile, tarTile.Get<TileWallWireStateData>());

                var tarTileTypeData = tarTile.Get<TileTypeData>();
                var tarWallTypeData = tarTile.Get<WallTypeData>();

                ref var tileTypeData =  ref refTile.Get<TileTypeData>();
                tileTypeData.Type = tarTileTypeData.Type;

                ref var tilewallTypeData = ref refTile.Get<WallTypeData>();
                tilewallTypeData.Type = tarWallTypeData.Type;
            }
        }

    }

    public enum Status
    {
        等待点击,
        等待左上角,
        等待右下角,
        保存,
        放置
    }
}