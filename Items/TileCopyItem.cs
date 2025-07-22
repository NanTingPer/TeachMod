using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TeachMod.Items;
/// <summary>
/// 物块拷贝
/// </summary>
public class TileCopyItem : ModItem
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.EmpressBlade;
    public override void SetDefaults()
    {
        Item.useStyle = ItemUseStyleID.Swing;
        Item.autoReuse = false;
        Item.useTime = 1;
        Item.useAnimation = 1;
        base.SetDefaults();
    }


    public int _step = 0;
    public Vector2 _leftPoint = Vector2.Zero;
    public Vector2 _rightPoint = Vector2.Zero;
    public Dictionary<Point16, Tile> TileDataSave = [];
    public override bool? UseItem(Player player)
    {
        var pos = player.position;
        if (_step == 0) {
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 10, 10), Color.White, "已经选择左上角，请选择右下脚!");
            _leftPoint = Main.MouseWorld.ToTileCoordinates16().ToWorldCoordinates();
        } else if(_step == 1) {
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 10, 10), Color.White, "已经选择右下角，再次使用保存!");
            _rightPoint = Main.MouseWorld.ToTileCoordinates16().ToWorldCoordinates();
        } else if(_step == 2) {
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 10, 10), Color.White, "已经保存选中项，再次使用创建!");
            TileDataSave = GetTiles(_leftPoint, _rightPoint);
        } else if(_step == 3) {
            #region 创建结构
            foreach (var tileData in TileDataSave) {
                var tile = tileData.Value;
                var createPos = Main.MouseWorld.ToTileCoordinates16() + tileData.Key;
                WorldGen.PlaceTile(createPos.X, createPos.Y, tile.TileType, style: (int)tile.BlockType);
            }
            #endregion
        }
        _step += 1;
        if(_step == 4) {
            _step = 0;
        }
        Main.NewText(_step);
        return base.UseItem(player);
    }

    public Rectangle _drawRectangle;
    public static Dictionary<Point16, Tile> GetTiles(Vector2 leftUpPointWorld, Vector2 rightDownPointWorld)
    {
        Dictionary<Point16, Tile> tileDataSave = [];
        var leftPonit = leftUpPointWorld.ToTileCoordinates16();
        var rightPoint = rightDownPointWorld.ToTileCoordinates16();
        var newLeftPoint = new Point16(
            X: Math.Min(leftPonit.X, rightPoint.X),
            Y: Math.Min(leftPonit.Y, rightPoint.Y));

        var newRightPoint = new Point16(
            X: Math.Max(leftPonit.X, rightPoint.X),
            Y: Math.Max(leftPonit.Y, rightPoint.Y));

        Point16 tarPoint = newRightPoint - newLeftPoint;

        //Tile.SmoothSlope //根据相邻的方块倾斜一个方块
        //Main.tile[]
        for (int i = 0; i < tarPoint.X; i++) {
            for (int j = 0; j < tarPoint.Y; j++) {
                Point16 tilePoint = newLeftPoint + new Point16(i, j);
                var tile = Main.tile[tilePoint];
                //tile.BlockType
                //tile.Slope
                if (tile != null && tile.HasTile) {
                    Main.NewText(TileID.Search.GetName(tile.TileType));
                    tileDataSave[new Point16(i, j)] = tile;
                }

                //WorldGen.KillTile(tilePoint.X, tilePoint.Y);
                //WorldGen.PlaceTile(tilePoint.X, tilePoint.Y, 0);
                #region WorldGen
                //TileRunner是替换方块
                //WorldGen.TileRunner(tilePoint.X, tilePoint.Y, 2, 1, 30);

                //PlaceWall是放墙
                //WorldGen.PlaceWall(tilePoint.X, tilePoint.Y, 30);

                //Trap陷阱
                //WorldGen.placeTrap(tilePoint.X, tilePoint.Y, 30);

                //种树
                //WorldGen.PlaceXmasTree(tilePoint.X, tilePoint.Y, 172);
                #endregion
            }
        }

        return tileDataSave;
    }
}

public class TileCopyItemPlayerLayer : PlayerDrawLayer
{
    public static SpriteBatch SpriteBatch;
    public static Texture2D _white;
    static TileCopyItemPlayerLayer()
    {
        //绘图相关必须在主线程实例化 / 调用
        Main.QueueMainThreadAction(() => {
            _white = new Texture2D(Main.instance.GraphicsDevice, 1, 1);
            _white.SetData([Color.White]);
            SpriteBatch = new SpriteBatch(Main.instance.GraphicsDevice);
        });
    }
    public override bool IsHeadLayer => false;
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        if(drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<TileCopyItem>())
            return true;
        return false;
    }

    public override Position GetDefaultPosition()
    {
        return PlayerDrawLayers.Head.GetDefaultPosition();
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        var moditem = drawInfo.drawPlayer.HeldItem.ModItem as TileCopyItem;
        var _step = moditem._step;

        //!=0说明已经按下了一下鼠标
        if (_step != 0 && _step != 3) {
            if (_step == 1) {
                moditem._rightPoint = Main.MouseWorld.ToTileCoordinates16().ToWorldCoordinates();
            }
            ref var rectangle = ref moditem._drawRectangle;
            #region 修改绘制矩形
            //图格是16*16 坐标系X是正常，Y是下正上负
            var screenPos = moditem._leftPoint.ToTileCoordinates16().ToWorldCoordinates() + new Vector2(-8, -8) - Main.screenPosition;
            rectangle.X = (int)screenPos.X;
            rectangle.Y = (int)screenPos.Y;
            var newV2 = moditem._rightPoint - moditem._leftPoint;
            rectangle.Height = (int)newV2.Y;
            rectangle.Width = (int)newV2.X;
            #endregion

            #region 绘制矩形
            //可以使用 状态拷贝 (懒了)
            //https://github.com/stormytuna/FishUtils/blob/main/DataStructures/SpriteBatchParams.cs
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            SpriteBatch.Draw(_white, rectangle, null, Color.White * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            SpriteBatch.End();
            #endregion

        }

        if (_step == 3) {
            #region 绘制结构残影
            var tiles = moditem.TileDataSave;
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            foreach (var tileData in tiles) {
                var tile = tileData.Value;
                var drawVector = (tileData.Key.ToWorldCoordinates() + Main.MouseWorld) - Main.screenPosition;
                var value = new Rectangle(tile.TileFrameX, tile.TileFrameX, 16, 16);
                SpriteBatch.Draw(TextureAssets.Tile[tileData.Value.TileType].Value, drawVector, value, Color.White * 0.5f, 0f, default, 1f, SpriteEffects.None, 1f);
            }
            SpriteBatch.End();
            #endregion
        }
    }
}
