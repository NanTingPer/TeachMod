using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;

namespace TeachMod.Arithmetic;

public class 携程 : TeachModSystem
{
    public static Dictionary<int, 携程模型> pool = [];
    public static Func<IEnumerator> Add(Func<IEnumerator> enumerator, bool isLoop = true)
    {
        return Add(enumerator, TimeSpan.FromSeconds(0), isLoop);
    }
    public static Func<IEnumerator> Add(Func<IEnumerator> enumerator, TimeSpan updateTime, bool isLoop = true)
    {

        if (pool.ContainsKey(enumerator.GetHashCode())) {
            return enumerator;
        }

        var code = enumerator.GetHashCode();
        var obj = new 携程模型()
        {
            Code = code,
            Run = enumerator.Invoke(),
            IsLoop = isLoop,
            Orig = enumerator,
            UpdateTime = updateTime
        };

        pool.Add(code, obj);
        return enumerator;
    }
    public override void PreUpdate(Main main, ref GameTime gametime)
    {
        List<int> removeKeys = [];
        foreach (var item in pool.Values) {
            item.WaitTime += TimeSpan.FromSeconds(1f / 60f);
            if (item.CheckWaitTime_If_True_Then_Reset_WaitTime() && !item.MoveNext()) {
                if (item.IsLoop) {
                    item.Reset(); // 循环
                } else {
                    removeKeys.Add(item.Code); // 不循环
                }
            }
        }
        foreach (var item in removeKeys) {
            pool.Remove(item);
        }
        base.PreUpdate(main, ref gametime);
    }
    public static void Remove(int code)
    {
        pool.Remove(code);
    }
}

public class 携程模型
{
    /// <summary>
    /// <see cref="Orig"/>的<see cref="object.GetHashCode"/>
    /// </summary>
    public int Code { get; set; }
    /// <summary>
    /// 获取<see cref="Run"/>的方法
    /// </summary>
    public Func<IEnumerator> Orig { get; set; }
    /// <summary>
    /// 实际的迭代器
    /// </summary>
    public IEnumerator Run { get; set; }
    /// <summary>
    /// 是否循环
    /// </summary>
    public bool IsLoop { get; set; }
    public TimeSpan UpdateTime { get; set; } = TimeSpan.FromSeconds(1.0f);
    public TimeSpan WaitTime { get; set; } = TimeSpan.Zero;
    public bool MoveNext()
    {
        return Run.MoveNext();
    }

    public void Reset()
    {
        Run = Orig.Invoke();
    }

    public bool CheckWaitTime_If_True_Then_Reset_WaitTime()
    {
        if(UpdateTime.Ticks - WaitTime.Ticks < 0) {
            WaitTime = TimeSpan.Zero;
            return true;
        }
        return false;
    }
}
