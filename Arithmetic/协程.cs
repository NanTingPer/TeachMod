using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;

namespace TeachMod.Arithmetic;

public class 协程 : TeachModSystem
{
    /// <summary>
    /// 携程池
    /// </summary>
    public readonly static Dictionary<Guid, Stack<协程模型>> pool = [];
    private readonly static Queue<Guid> clearQueue = [];
    public static 携程管理 Add(Func<IEnumerator> enumerator, bool isLoop = true)
    {
        return Add(enumerator, TimeSpan.FromSeconds(0), isLoop);
    }
    public static 携程管理 Add(Func<IEnumerator> enumerator, TimeSpan intervalTime, bool isLoop = true)
    {
        var code = Guid.NewGuid();
        var obj = new 协程模型()
        {
            Code = code,
            Run = enumerator.Invoke(),
            IsLoop = isLoop,
            Orig = enumerator,
            IntervalTime = intervalTime
        };

        var stack = new Stack<协程模型>();
        stack.Push(obj);
        pool.Add(code, stack);
        return new(){ Guid = code, StopAction = () => Remove(code) };
    }

    private readonly Dictionary<Guid, AwaitFrames> awaitPool = [];
    /// <summary>
    /// 支持等待和嵌套
    /// </summary>
    public override void PreUpdate(Main main, ref GameTime gametime)
    {
        foreach (var stack in pool.Values) {
            var run = stack.Peek();
            #region 等待AwaitFrames
            {
                if (awaitPool.TryGetValue(run.Code, out var awaitF)) {
                    awaitF.Frames -= 1;
                    if (awaitF.Frames <= 0) {
                        awaitPool.Remove(run.Code);
                    }
                    continue;
                }
            }
            #endregion

            run.WaitTime += TimeSpan.FromSeconds(1f / 60f);
            if (run.CheckWaitTime_If_True_Then_Reset() && !run.MoveNext()) {
                if (run.IsSub) { // 子直接出栈丢弃
                    stack.Pop();
                } else if (run.IsLoop) { // 根 且循环，不出栈
                    run.Reset();
                } else {
                    clearQueue.Enqueue(run.Code); // 根， 不循环 删除
                }
            }

            #region 处理子协程和AwaitFrames
            if (run.Run.Current != null) {
                if (run.Run.Current is 子协程 sub) {
                    stack.Push(new 协程模型()
                    {
                        Code = Guid.NewGuid(),
                        IntervalTime = sub.IntervalTime,
                        IsLoop = false,
                        Orig = sub.EnumeratorFactory,
                        Run = sub.EnumeratorFactory.Invoke(),
                        IsSub = true
                    });
                } else if(run.Run.Current is AwaitFrames awaitF) { // 等待帧
                    awaitPool[run.Code] = awaitF;
                }
            }
            #endregion
        }
        while (clearQueue.Count > 0) {
            pool.Remove(clearQueue.Dequeue());
        }
    }

    public static void Remove(Guid id)
    {
        pool.Remove(id);
    }
}

public class 协程模型
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public Guid Code { get; set; }
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
    public bool IsLoop { get; set; } = false;
    /// <summary>
    /// 是否是子协程
    /// </summary>
    public bool IsSub { get; set; } = false;
    /// <summary>
    /// 间隔
    /// </summary>
    public TimeSpan IntervalTime { get; set; } = TimeSpan.FromSeconds(1.0f);
    /// <summary>
    /// 已等待时间
    /// </summary>
    public TimeSpan WaitTime { get; set; } = TimeSpan.Zero;
    public bool MoveNext()
    {
        return Run.MoveNext();
    }

    public void Reset()
    {
        Run = Orig.Invoke();
    }

    /// <summary>
    /// 检查已等待时间
    /// </summary>
    public bool CheckWaitTime_If_True_Then_Reset()
    {
        if(IntervalTime.Ticks - WaitTime.Ticks < 0) {
            WaitTime = TimeSpan.Zero;
            return true;
        }
        return false;
    }
}

public record class 携程管理
{
    public Guid Guid { get; set; }
    public void Stop()
    {
        StopAction?.Invoke();
    }

    internal Action StopAction { get; set; }
}

public struct 子协程
{
    public 子协程()
    {
        IntervalTime = TimeSpan.Zero;
    }

    public 子协程(TimeSpan intervalTime)
    {
        IntervalTime = intervalTime;
    }
    public required Func<IEnumerator> EnumeratorFactory { get; set; }
    public TimeSpan IntervalTime { get; set; }
}

public class AwaitFrames
{
    /// <summary>
    /// 等待帧
    /// </summary>
    public uint Frames { get; set; }
}