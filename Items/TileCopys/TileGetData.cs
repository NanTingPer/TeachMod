using System.Collections.Generic;
using System;

using Terraria;
using Terraria.DataStructures;

using Microsoft.Xna.Framework;

namespace TeachMod.Items.TileCopys;

/// <summary>
/// <para> 对于<see cref="Tilemap.GetData{T}"/> 可以查看接口文档 <see href="https://docs.tmodloader.net/docs/stable/interface_i_tile_data.html">ITileData</see>  </para>
/// <para> 值得注意的是，其返回的是整个Tilemap底层维护的一维数组，直接覆盖会直接修改其维护的数据 </para>
/// <para> 如何获得给定<see cref="Point16"/>对应的数据，可以查看<see cref="Tilemap.this"/>的实现 </para>
/// </summary>
public static class TileGetData
{
    /// <summary>
    /// 以引用的方式返回此图格坐标对应的<see cref="TileTypeData"/>
    /// </summary>
    public static ref TileTypeData GetTileType(Point16 point) => ref GetTileType(point.X, point.Y);
    /// <summary>
    /// 以引用的方式返回此图格坐标对应的<see cref="TileTypeData"/>
    /// </summary>
    public static ref TileTypeData GetTileType(int x, int y)
    {
        var tileTypes = Main.tile.GetData<TileTypeData>();
        return ref tileTypes[y + x * Main.tile.Height];
    }

    /// <summary>
    /// 以引用的方式返回此图格坐标对应的<see cref="WallTypeData"/>
    /// </summary>
    public static ref WallTypeData GetWallType(Point16 point) => ref GetWallType(point.X, point.Y);
    /// <summary>
    /// 以引用的方式返回此图格坐标对应的<see cref="WallTypeData"/>
    /// </summary>
    public static ref WallTypeData GetWallType(int x, int y)
    {
        var vallTypes = Main.tile.GetData<WallTypeData>();
        return ref vallTypes[y + x * Main.tile.Height];
    }

    /// <summary>
    /// 以引用的方式返回此图格坐标对应的<see cref="TileWallWireStateData"/>
    /// </summary>
    public static ref TileWallWireStateData GetTileWallData(Point16 point) => ref GetTileWallData(point.X, point.Y);
    /// <summary>
    /// 以引用的方式返回此图格坐标对应的<see cref="TileWallWireStateData"/>
    /// </summary>
    public static ref TileWallWireStateData GetTileWallData(int x, int y)
    {
        var dataTypes = Main.tile.GetData<TileWallWireStateData>();
        return ref dataTypes[y + x * Main.tile.Height];
    }

    /// <summary>
    /// 直接覆盖图格坐标对应的<see cref="TileWallWireStateData"/>
    /// </summary>
    public static void SetTileWallData(Point16 point, TileWallWireStateData data) => SetTileWallData(point.X, point.Y, data);
    /// <summary>
    /// 直接覆盖图格坐标对应的<see cref="TileWallWireStateData"/>
    /// </summary>
    public static void SetTileWallData(int x, int y, TileWallWireStateData data)
    {
        Main.tile.GetData<TileWallWireStateData>()[y + x * Main.tile.Height] = data;
    }

    /// <summary>
    /// 获取给定矩形内的图格数据
    /// <para><see cref="TileTypeData"/></para>
    /// <para><see cref="WallTypeData"/></para>
    /// <para><see cref="TileWallWireStateData"/></para>
    /// <para><see cref="Point16"/> 基于左上角的偏移 </para>
    /// </summary>
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