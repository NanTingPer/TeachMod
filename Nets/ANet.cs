#pragma warning disable CA2255
using MonoMod.RuntimeDetour;
using ReLogic.OS.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;

namespace TeachMod.Nets;

/// <summary>
/// <para> 网络自动同步的抽象 </para>
/// <para> 被TAttr标记的属性 / 字段会生效 </para>
/// </summary>
/// <typeparam name="TType"> 目标类型 </typeparam>
/// <typeparam name="TAttr"> 生效特性 </typeparam>
public abstract class ANet<TType, TAttr> where TAttr : Attribute
{
    /// <summary>
    /// 全部的字段，Key是类型，Value是这个类型的全部字段(被 <see cref="TAttr"/> 标记的)
    /// </summary>
    protected readonly static Dictionary<Type, List<FieldInfo>> _netFieldInfo = [];
    /// <summary>
    /// 全部的属性，Key是类型，Value是这个类型的全部属性(被 <see cref="TAttr"/> 标记的)
    /// </summary>
    protected readonly static Dictionary<Type, List<PropertyInfo>> _netPropInfo = [];
    /// <summary>
    /// 全部的类型，来自本程序集中，继承自<see cref="TType"/>的类型
    /// </summary>
    protected readonly static List<Type> tarType = [];
    /// <summary>
    /// object => Action<TType, TValue>
    /// <para> 开发中: Key是字段 / 属性， Value是这个字段对应的Set方法 </para>
    /// </summary>
    protected readonly static Dictionary<MemberInfo, Action<object, object>> _setValues = [];

    static ANet()
    {
        var getExpMethod = typeof(Ext).GetMethod(nameof(Ext.CreateSetFieldExp), BindingFlags.Public | BindingFlags.Static);

        tarType = typeof(GlobalNPCAutoNet)
            .Assembly
            .GetTypes()
            .Where(f => f.IsSubclassOf(typeof(TType)) && f.IsAbstract == false)
            .ToList();

        foreach (var type in tarType) {
            _netFieldInfo[type] = [];
            _netPropInfo[type] = [];
            foreach (var field in type.GetFields()) {
                if (field.GetCustomAttribute<TAttr>() != null) {
                    _netFieldInfo[type].Add(field);
                    _setValues[field] = field.CreateSetFieldExp();
                    //getExpMethod.MakeGenericMethod(field.DeclaringType, field.FieldType).Invoke(null, [field]);
                }
            }

            foreach (var propertyInfo in type.GetProperties()) {
                if (propertyInfo.GetCustomAttribute<TAttr>() != null) {
                    _netPropInfo[type].Add(propertyInfo);
                    _setValues[propertyInfo] = propertyInfo.CreateSetPropExp();
                }
            }
        }
        foreach (var keyValue in _netFieldInfo) {
            var k = keyValue.Value.ToHashSet().ToList();
            keyValue.Value.Clear();
            keyValue.Value.AddRange(k);
            keyValue.Value.Sort((a, b) => a.MetadataToken.CompareTo(b.MetadataToken));
        }

        foreach (var keyValue in _netPropInfo) {
            var k = keyValue.Value.ToHashSet().ToList();
            keyValue.Value.Clear();
            keyValue.Value.AddRange(k);
            keyValue.Value.Sort((a, b) => a.MetadataToken.CompareTo(b.MetadataToken));
        }
    }
}
