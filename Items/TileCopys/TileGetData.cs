using System.Collections.Generic;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace TeachMod.Items.TileCopys;

public static class TileGetData
{
    public static TileTypeData GetTileType(Point16 point) => GetTileType(point.X, point.Y);
    public static TileTypeData GetTileType(int x, int y)
    {
        var tileTypes = Main.tile.GetData<TileTypeData>();
        return tileTypes[y + x * Main.tile.Height];
    }

    public static WallTypeData GetWallType(Point16 point) => GetWallType(point.X, point.Y);
    public static WallTypeData GetWallType(int x, int y)
    {
        var vallTypes = Main.tile.GetData<WallTypeData>();
        return vallTypes[y + x * Main.tile.Height];
    }

    public static ref TileWallWireStateData GetTileWallData(Point16 point) => ref GetTileWallData(point.X, point.Y);
    public static ref TileWallWireStateData GetTileWallData(int x, int y)
    {
        var dataTypes = Main.tile.GetData<TileWallWireStateData>();
        return ref dataTypes[y + x * Main.tile.Height];
    }

    public static void SetTileWallData(Point16 point, TileWallWireStateData data) => SetTileWallData(point.X, point.Y, data);
    public static void SetTileWallData(int x, int y, TileWallWireStateData data)
    {
        Main.tile.GetData<TileWallWireStateData>()[y + x * Main.tile.Height] = data;
    }

    public static List<SaveTileData> GetTiles(Vector2 leftUpPointWorld, Vector2 rightDownPointWorld)
    {
        #region 计算给定坐标范围内包含的图格
        var leftPonit = leftUpPointWorld.ToTileCoordinates16();
        var rightPoint = rightDownPointWorld.ToTileCoordinates16();
        var newLeftPoint = new Point16(
            X: Math.Min(leftPonit.X, rightPoint.X),
            Y: Math.Min(leftPonit.Y, rightPoint.Y));

        var newRightPoint = new Point16(
            X: Math.Max(leftPonit.X, rightPoint.X),
            Y: Math.Max(leftPonit.Y, rightPoint.Y));

        Point16 tarPoint = newRightPoint - newLeftPoint;
        #endregion

        List<SaveTileData> tiles = [];
        #region 获取范围内的全部图格
        for (int i = 0; i < tarPoint.X; i++) {
            for (int j = 0; j < tarPoint.Y; j++) {
                Point16 tilePoint = newLeftPoint + new Point16(i, j);
                tiles.Add(new SaveTileData()
                {
                    TileType = GetTileType(tilePoint),
                    WallType = GetWallType(tilePoint),
                    Data = GetTileWallData(tilePoint),
                    Point = new Point16(i, j),
                });
                
            }
        }
        #endregion

        return tiles;
    }

}

public class SaveTileData
{
    public TileTypeData TileType { get; set; }
    public WallTypeData WallType { get; set; }
    public TileWallWireStateData Data { get; set; }
    public Point16 Point { get; set; }
}
#region WorldGen
//Tile.SmoothSlope //根据相邻的方块倾斜一个方块
//WorldGen.KillTile(tilePoint.X, tilePoint.Y);
//WorldGen.PlaceTile(tilePoint.X, tilePoint.Y, 0);

//TileRunner是替换方块
//WorldGen.TileRunner(tilePoint.X, tilePoint.Y, 2, 1, 30);

//PlaceWall是放墙
//WorldGen.PlaceWall(tilePoint.X, tilePoint.Y, 30);

//Trap陷阱
//WorldGen.placeTrap(tilePoint.X, tilePoint.Y, 30);

//种树
//WorldGen.PlaceXmasTree(tilePoint.X, tilePoint.Y, 172);
#endregion

