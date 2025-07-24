using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace TeachMod.Items.TileCopys;
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
    public List<SaveTileData> TileDataSave = [];
    public override bool? UseItem(Player player)
    {
        var pos = player.position;
        if (_step == 0) { //选中左上角
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 10, 10), Color.White, "已经选择左上角，请选择右下脚!");
            _leftPoint = Main.MouseWorld.ToTileCoordinates16().ToWorldCoordinates();
        } else if(_step == 1) { //选中右下角
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 10, 10), Color.White, "已经选择右下角，再次使用保存!");
            _rightPoint = Main.MouseWorld.ToTileCoordinates16().ToWorldCoordinates();
        } else if(_step == 2) { //获取选中范围内的图格
            CombatText.NewText(new Rectangle((int)pos.X, (int)pos.Y, 10, 10), Color.White, "已经保存选中项，再次使用创建!");
            TileDataSave = GetTiles(_leftPoint, _rightPoint);
        } else if(_step == 3) {
            #region 创建结构
            foreach (var tileData in TileDataSave) {
                var createPos = Main.MouseWorld.ToTileCoordinates16() + tileData.Point;
                WorldGen.PlaceTile(createPos.X, createPos.Y, tileData.TileType.Type);
                WorldGen.PlaceWall(createPos.X, createPos.Y, tileData.WallType.Type);

                #region 修改TileWallWire
                //WordGen.SlopeTile 但是不会同步
                TileGetData.SetTileWallData(createPos, tileData.Data);
                //WorldGen.SlopeTile(createPos.X, createPos.Y, (int)tile.Slope);
                #endregion
                WorldGen.SquareTileFrame(createPos.X, createPos.Y, true);
                WorldGen.SquareWallFrame(createPos.X, createPos.Y, true);
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
    private static List<SaveTileData> GetTiles(Vector2 leftUpPointWorld, Vector2 rightDownPointWorld)
    {
        return TileGetData.GetTiles(leftUpPointWorld, rightDownPointWorld);
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
                var drawVector = tileData.Point.ToWorldCoordinates() + Main.MouseWorld - Main.screenPosition;
                var value = new Rectangle(tileData.Data.TileFrameX, tileData.Data.TileFrameY, 16, 16);
                SpriteBatch.Draw(TextureAssets.Tile[tileData.TileType.Type].Value, drawVector, value, Color.White * 0.5f, 0f, default, 1f, SpriteEffects.None, 1f);
            }
            SpriteBatch.End();
            #endregion
        }
    }
}

/*
 *  更多注释
 *      使用GetData获取注释，可以查看tModLoader的ITileData接口相关实现
 *      其中TileWallWireStateData是相关图格的数据
 *      Main.tile虽然可以跟使用二维数组，但其实他只是一维的Array
 *      而一个tile数据可以同时包含 `墙` `方块` 获取相关信息 可以使用 "WallType" "TileType" 从GetData获取
 *      
 *      如何使用GetData如同使用 [x, y] 一般，可看Main.tile[] 如何实现
 */
