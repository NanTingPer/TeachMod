using System;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace TeachMod.Teachs;

public static partial class TeachModUtils
{
    public static IEnumerable<T> Where<T>(this ActiveEntityIterator<T> iter, Func<T, bool> func)
        where T : Entity
    {
        var enumerator = iter.GetEnumerator();
        List<T> v = [];
        while (enumerator.MoveNext()) {
            if (func(enumerator.Current))
                v.Add(enumerator.Current);
        }
        return v;
    }

    public static IEnumerable<TOut> Select<TIn, TOut>(this ActiveEntityIterator<TIn> iter, Func<TIn, TOut> func)
        where TIn : Entity
    {
        var enumerator = iter.GetEnumerator();
        List<TOut> v = [];
        while (enumerator.MoveNext()) {
            v.Add(func(enumerator.Current));
        }
        return v;
    }

    public static IEnumerable<T> AsIEnumerable<T>(this ActiveEntityIterator<T> iter)
        where T : Entity
    {
        var enumerator = iter.GetEnumerator();
        List<T> v = [];
        while (enumerator.MoveNext()) {
            v.Add(enumerator.Current);
        }
        return v;
    }
}
