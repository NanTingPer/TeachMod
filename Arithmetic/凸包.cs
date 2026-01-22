using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace TeachMod.Arithmetic;

/// <summary>
/// 本System用于绘制凸包，设置<see cref="凸包.drawArithmentic"/>为true则会绘制，绘制执行在<see cref="PostDoDraw(SpriteBatch, Main, GameTime)"/>
/// </summary>
public class 凸包 : TeachModSystem
{
    public static bool drawArithmentic = false; // 是否启用本绘制
    public static List<EntityPoint> pointList = [];

    public 凸包()
    {
        #region 定义点
        pointList.AddRange([
            // 不规则外围点（打破对称，错落分布，决定凸包的不规则轮廓）
            new EntityPoint() { position = new Vector2(1700, 450) },     // 右侧偏上（非中点，错落）
            new EntityPoint() { position = new Vector2(1250, 1200) },    // 右下偏左（不贴最右侧，避免规整）
            new EntityPoint() { position = new Vector2(80, 150) },       // 左上偏内（非边角对称）
            new EntityPoint() { position = new Vector2(1680, 1180) },    // 右下近极值（略偏移，不规整）
            new EntityPoint() { position = new Vector2(1300, 200) },     // 右上偏左（不贴最右侧）
            new EntityPoint() { position = new Vector2(320, 1150) },     // 左下偏右（不贴最左侧，错落）
            new EntityPoint() { position = new Vector2(480, 320) },      // 左上偏中（非规整坐标）
            new EntityPoint() { position = new Vector2(1500, 380) },     // 右上偏中（偏移右侧边缘）
            new EntityPoint() { position = new Vector2(1000, 950) },     // 右下偏中（非中点，打破均匀）
            new EntityPoint() { position = new Vector2(280, 1020) },     // 左下偏中（错落分布，不规整）
            // 不规则内部点（填充中间，无规律排布，避免辅助凸包规整）
            new EntityPoint() { position = new Vector2(720, 480) },      // 中间偏上（非规整坐标）
            new EntityPoint() { position = new Vector2(980, 620) },      // 中心附近（略偏移，无对称）
            new EntityPoint() { position = new Vector2(1150, 520) },     // 中间偏右（错落，不均匀）
            new EntityPoint() { position = new Vector2(580, 780) },      // 中间偏左（非对称填充）
            new EntityPoint() { position = new Vector2(820, 920) },      // 中间偏下（略偏移，打破规整）
            new EntityPoint() { position = new Vector2(1420, 730) },     // 右侧中间（非中点，错落）
            new EntityPoint() { position = new Vector2(350, 580) },      // 左侧中间（偏移，不规整）
            new EntityPoint() { position = new Vector2(1050, 280) }      // 上侧中间（非对称，打破均匀）
        ]);
        #endregion
    }

    /// <summary>
    /// 覆盖整个屏幕的点
    /// </summary>
    private static EntityPoint screenPoint = new EntityPoint() { position = Vector2.Zero, Height = Main.screenHeight, Width = Main.screenWidth, color = Color.Black };
    public override void PostDoDraw(SpriteBatch spriteBatch, Main main, GameTime gameTime)
    {
        if (drawArithmentic == false) return;
        lineColor ??= Texture2D.CreateTexture2D(20, 20, Color.White); // 初始化线条颜色 为白色
        screenPoint.Draw(spriteBatch, main, gameTime); // 绘制屏幕原点方便定位 （将整个屏幕变为黑色）

        foreach (var item in pointList) { // 绘制全部点的位置
            item.Draw(spriteBatch, main, gameTime);
        }

        spriteBatch.Begin();
        DrawLine(spriteBatch, [.. 凸包点], Color.Red); // 对凸包点进行连线
        spriteBatch.End();
    }

    private readonly List<EntityPoint> 凸包点 = []; // 全部凸包点
    private static Texture2D lineColor; // 凸包点的连线
    private Func<System.Collections.IEnumerator>? enumerator = null;
    public override void PreUpdate(Main main, ref GameTime gametime) // 更新目标点
    {
        if (drawArithmentic == false) return;
        foreach (var item in pointList) {
            item.Update(main, ref gametime);
        }
        if (pointList.Count < 3) return;

        //1. 计算最左边的点作为起始点 or MaxBy(p => p.Y);
        // - 随机获取一个初始点
        // - 遍历点列表，比较Y轴位置

        //2. 计算下一个凸包点
        // |- 进入循环前 设置起始点为当前点 并将起始点加入到集合中
        // |- 随机选取下一个点 pointlist[0]
        // |- 遍历点列表
        //   |- 计算三点叉积  当前点, 下一个点, 遍历到的点
        //   |- 如果nextPoint == currentPoint || 叉积 > 0 ||
        //     |- (共线判断): |叉积|  < 0.001 && Vector2.Distance(currentPoint.position, candidate.position) > Vector2.Distance(currentPoint.position, nextPoint.position)
        //     |- 下一个点就为当前遍历到的点
        // |- 设置当前点为下一个点
        // |- 如果当前点不等于起始点，加入到合集
        // do while的结束条件为 (currentPoint != startPoint && 凸包点.Count <= pointList.Count)

        enumerator ??= 携程.Add(NewOrig, TimeSpan.FromSeconds(0.25f), true);
    }

    private System.Collections.IEnumerator NewOrig()
    {
        if (pointList.Count <= 0) yield break;
        var startPoint = pointList.MinBy(f => f.X);
        var currentPoint = startPoint;
        do {
            var nextPoint = pointList[0];
            foreach (var 候选点 in pointList) {
                if(候选点 == currentPoint) continue;
                if(nextPoint == currentPoint || Cross(currentPoint, nextPoint, 候选点) > 0) {
                    nextPoint = 候选点;
                }    
            }
            currentPoint = nextPoint;
            if (!凸包点.Contains(nextPoint)) {
                凸包点.Add(nextPoint);
            }
            yield return null;
        } while (currentPoint != startPoint);
        凸包点.Clear();
        yield break;
    }



    private void Orig()
    {
        var startPoint = pointList.MinBy(ep => ep.X);
        var currentPoint = startPoint;
        凸包点.Add(currentPoint);
        do {
            var nextPoint = pointList[0];
            foreach (var 预选 in pointList) {
                if (预选 == currentPoint) {
                    continue;
                }
                var cross = Cross(currentPoint, nextPoint, 预选);
                if (nextPoint == currentPoint || cross > 0 || (Math.Abs(cross) < 0.001f && 共线且更远(currentPoint, 预选, nextPoint))) {
                    nextPoint = 预选;
                }
            }
            currentPoint = nextPoint;
            if (currentPoint != startPoint) {
                凸包点.Add(currentPoint);
            }
        } while (currentPoint != startPoint && 凸包点.Count < pointList.Count);
    }

    private bool 共线且更远(EntityPoint ep1, EntityPoint ep2, EntityPoint ep3)
    {
        return Vector2.Distance(ep1, ep2) > Vector2.Distance(ep1, ep3);
    }

    // 计算叉积 (p1->p2) × (p1->p3)
    //叉积 > 0：b在a的逆时针方向（左转）
    //叉积 = 0：a和b共线（方向相同或相反）
    //叉积< 0：b在a的顺时针方向（右转）

    /// <summary>
    /// 计算(p1 -> p2)与(p1 -> p3)的2D叉积
    /// <br/>
    /// <code>
    /// // 当p1为(0, 0)时的公式: p2.x * p3.y - p3.x * p2.y
    /// return (p2.x - p1.x)*(p3.y - p1.y) - (p2.y - p1.y)*(p3.x - p1.x);
    /// </code>
    /// </summary>
    /// <param name="p1"> 当前线段起点 </param>
    /// <param name="p2"> 第一个向量的终点(p1 -> p2) </param>
    /// <param name="p3"> 第二个向量的终点(p1 -> p3) </param>
    /// <returns>
    /// 叉积值：
    /// <br/> +：p3在向量(p1->p2)的左侧（逆时针方向）
    /// <br/> -：p3在向量(p1->p2)的右侧（顺时针方向）
    /// <br/> 0：三点共线
    /// </returns>
    /// <remarks>
    /// <see href="https://www.bilibili.com/video/BV1ib411t7YR?p=11"/> 
    /// </remarks>
    private static float Cross(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        //想象你站在点 p1，面朝点 p2：
        //Cross(p1, p2, p3) > 0：点 p3 在你的左手边
        //Cross(p1, p2, p3) = 0：点 p3 在你正前方或正后方（共线）
        //Cross(p1, p2, p3) < 0：点 p3 在你的右手边
        return (p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X);
    }

    public static void DrawLine(SpriteBatch spriteBatch, List<EntityPoint> points, Color color)
    {
        if (points.Count < 2) return;

        for (int i = 0; i < points.Count; i++) {
            int nextIndex = (i + 1) % points.Count; // 使用模运算确保闭合
            ArithmeticUtils.DrawLine(spriteBatch, points[i].position, points[nextIndex].position, color, color, 5);
        }
    }
}
