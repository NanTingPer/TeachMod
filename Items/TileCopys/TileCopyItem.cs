using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

            //门需要额外处理 还有很多特殊物块都是这样
            //傀儡等需要依附的方块需要从下往上生成
            //藤蔓等需要从上往下生成

            //全部物块
            List<(TileTypeData tileType, TileWallWireStateData data, Point16 offset)> tiles = TileDataSave
                .Where(f => f.TileType.Type != 0) //不等于0说明存在实际物块
                .Select(f => (f.TileType, f.Data, f.Point))  //解构
                .Where(tileType =>
                    //GetTileData == null 说明是地形图格
                    TileObjectData.GetTileData(tileType.TileType.Type, 0) == null)
                .ToList() //ToList其实是为了更好调试，本来是IEnumerable
                ;

            //全部墙壁 同上
            List<(WallTypeData wallType, TileWallWireStateData data, Point16 offset)> walls = TileDataSave
                .Where(f => f.WallType.Type != 0)
                .Select(f => (f.WallType, f.Data, f.Point))
                .ToList()
                ;

            //全部Object
            //如门等多方块结构都无法正常使用普通的PlaceTile放置
            List<(TileTypeData tileType, TileWallWireStateData data, Point16 offset)> objes = TileDataSave
                .Where(f => f.TileType.Type != 0)
                .Select(f => (f.TileType, f.Data, f.Point))
                .Except(tiles)
                .ToList()
                ;

            var orig = Main.MouseWorld;
            _ = Task.Run(async () => {
                await PlaceWall(walls, orig);
                await PlaceTile(tiles, orig);
                //await PlaceDoor(door, sourcePoint);
                await PlaceObject(objes, orig);
            });
            
            

            static async Task PlaceTile(IEnumerable<(TileTypeData tileType, TileWallWireStateData data, Point16 offset)> tiles, Vector2 orig)
            {
                var sourcePoint = orig.ToTileCoordinates16();
                foreach (var (tileType, data, offset) in tiles) { //遍历方块，使用结构元组
                    await Task.Delay(100);
                    var createPos = sourcePoint + offset;   //实际创建方块的位置
                    if (WorldGen.InWorld(createPos.X, createPos.Y)) {   //如果此位置在世界之内，会覆盖现有物块
                        TileGetData.GetTileType(createPos).Type = tileType.Type; //将此位置的图格类型直接覆盖 也可以使用WorldGen.PlaceTile, 他不会覆盖图格
                        TileGetData.SetTileWallData(createPos, data);   //直接覆盖图格数据，因为上面是直接覆盖了图格类型，所以要这一步 使用WorldGen的方法可以不用
                        WorldGen.SquareTileFrame(createPos.X, createPos.Y);
                    }
                }
            }


            static async Task PlaceWall(IEnumerable<(WallTypeData tileType, TileWallWireStateData data, Point16 offset)> tiles, Vector2 orig)
            {
                var sourcePoint = orig.ToTileCoordinates16();
                foreach (var (wallType, data, offset) in tiles) {
                    await Task.Delay(100);
                    var createPos = sourcePoint + offset;
                    if (WorldGen.InWorld(createPos.X, createPos.Y)) {
                        TileGetData.GetWallType(createPos).Type = wallType.Type;
                        var newData = data;
                        //其他同上，需要这一步是因为，如果这个墙对应的Tile实际存在，
                        //我们又直接覆盖图格数据，他会变成土块，因为我们没有提供对应的物块类型
                        //使用WorldGen.PlaceWall可以不用，他也不会覆盖
                        newData.HasTile = false;   
                        TileGetData.SetTileWallData(createPos, newData);
                        WorldGen.SquareWallFrame(createPos.X, createPos.Y);
                    }
                }
            }

            static async Task PlaceObject(IEnumerable<(TileTypeData tileType, TileWallWireStateData data, Point16 offset)> objes, Vector2 orig)
            {
                //从下往上放，虽然CanPlace也不会同意
                objes = objes.OrderBy(f => f.offset.Y);
                var sourcePoint = orig.ToTileCoordinates16();
                foreach (var (tileType, data, offset) in objes) {
                    await Task.Delay(100);
                    var createPos = sourcePoint + offset;

                    //拷贝图格对象数据以获取图格样式，然后才能得到准确的TileObject
                    var toj = new TileObjectData();
                    toj.FullCopyFrom(tileType.Type);
                    if (WorldGen.InWorld(createPos.X, createPos.Y)
                    && !Main.tile[createPos].HasTile //有物块再放会挤掉 但是下面的CanPlace也不会同意
                    && TileObject.CanPlace(createPos.X, createPos.Y, tileType.Type, toj.Style, 1, out var obj)
                        ) {
                        TileObject.Place(obj);
                        WorldGen.SquareTileFrame(createPos.X, createPos.Y);
                    }
                }
            }
            #endregion
        }
        _step += 1;
        if(_step == 4) {
            _step = 0;
        }
        //Main.NewText(_step);
        return base.UseItem(player);
    }

    public Rectangle _drawRectangle;
    private static List<SaveTileData> GetTiles(Vector2 leftUpPointWorld, Vector2 rightDownPointWorld)
    {
        return TileGetData.GetTiles(leftUpPointWorld, rightDownPointWorld);
    }
}


#pragma warning disable CA2255
[Autoload(Side = ModSide.Client)]
public class TileCopyItemPlayerLayer : PlayerDrawLayer
{
    private static SpriteBatch spriteBatch;
    private static Texture2D white;
    public static SpriteBatch SpriteBatch
    {
        get
        {
            if(spriteBatch == null && white == null) {
                Main.QueueMainThreadAction(() => {
                    white = new Texture2D(Main.instance.GraphicsDevice, 1, 1);
                    white.SetData([Color.White]);
                    spriteBatch = new SpriteBatch(Main.instance.GraphicsDevice);
                });
            }
            return spriteBatch;
        }
    }
    public static Texture2D White
    {
        get
        {
            if(spriteBatch == null) {
                _ = SpriteBatch;
            }
            return white;
        }
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
        if (spriteBatch == null || drawInfo.drawPlayer.HeldItem.ModItem is not TileCopyItem moditem)
            return;
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
            SpriteBatch.Draw(White, rectangle, null, Color.White * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            SpriteBatch.End();
            #endregion

        }

        if (_step == 3) {
            #region 绘制结构残影
            var tiles = moditem.TileDataSave;
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            foreach (var tileData in tiles) {
                if (tileData.WallType.Type != 0) {//不等于0说明是有的
                    var drawVector = ((tileData.Point.ToWorldCoordinates() + Main.MouseWorld) - Main.screenPosition.ToTileCoordinates16().ToWorldCoordinates() - new Vector2(8f, 8f)) - new Point16(0, 0).ToWorldCoordinates();
                    var wallValue = new Rectangle(tileData.Data.WallFrameX, tileData.Data.WallFrameY, 32, 32);
                    SpriteBatch.Draw(TextureAssets.Wall[tileData.WallType.Type].Value, drawVector, wallValue, Color.White * 0.5f, 0f, default, 1f, SpriteEffects.None, 1f);
                }

                if (tileData.TileType.Type != 0) {
                    var drawVector = ((tileData.Point.ToWorldCoordinates() + Main.MouseWorld) - Main.screenPosition.ToTileCoordinates16().ToWorldCoordinates() - new Vector2(8f, 8f)) - new Point16(0, 0).ToWorldCoordinates();
                    var tileValue = new Rectangle(tileData.Data.TileFrameX, tileData.Data.TileFrameY, 16, 16);
                    SpriteBatch.Draw(TextureAssets.Tile[tileData.TileType.Type].Value, drawVector, tileValue, Color.White * 0.5f, 0f, default, 1f, SpriteEffects.None, 1f);
                }
            }
            SpriteBatch.End();
            #endregion
        }
    }
}