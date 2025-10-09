#pragma warning disable CA2255
using Microsoft.Xna.Framework;
using ReLogic.OS.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
namespace TeachMod.Nets;

public static class Ext
{
    /// <summary>
    /// <see cref="BinaryReader"/> 的读取方法，Key是方法对应的读取类型
    /// </summary>
    public readonly static Dictionary<Type, MethodInfo> ReadMethod = [];
    /// <summary>
    /// <see cref="BinaryReader"/> 的写入方法，Key是方法对应的写入类型
    /// </summary>
    public readonly static Dictionary<Type, MethodInfo> WriteMethod = [];

    /// <summary>
    /// 获取全部流读取与写入方法
    /// </summary>
    [ModuleInitializer]
    internal static void RegionStreamMethod()
    {
        var writeMethods = typeof(BinaryWriter).GetMethods().Where(m => m.Name == "Write" && m.GetParameters().Length == 1);
        foreach (var methodInfo in writeMethods) {
            var type = methodInfo.GetParameters()[0].ParameterType;
            if (!WriteMethod.TryGetValue(type, out _)) {
                WriteMethod.Add(type, methodInfo);
            }
        }

        var readMethods = typeof(BinaryReader).GetMethods()
            .Where(m =>
                m.Name.StartsWith("Read") &&
                m.Name != "Read" &&
                m.Name != "Read7BitEncodedInt" &&
                m.Name != "Read7BitEncodedInt64" &&
                m.GetParameters().Length == 0
            );

        foreach (var method in readMethods) {
            var type = method.ReturnParameter.ParameterType;
            if (!ReadMethod.TryGetValue(type, out _)) {
                ReadMethod.Add(type, method);
            }
        }

        #region Utils
        var utype = typeof(Utils);
        var readVector2 = utype.GetMethod("ReadVector2", BindingFlags.Static | BindingFlags.Public);
        var readRGB = utype.GetMethod("ReadRGB", BindingFlags.Static | BindingFlags.Public);
        ReadMethod[typeof(Vector2)] = readVector2;
        ReadMethod[typeof(Color)] = readRGB;

        var writeRGB = utype.GetMethod("WriteRGB", BindingFlags.Static | BindingFlags.Public);
        var writeVector2 = utype.GetMethod("WriteVector2", BindingFlags.Static | BindingFlags.Public);
        WriteMethod[typeof(Vector2)] = writeVector2;
        WriteMethod[typeof(Color)] = writeRGB;
        #endregion

    }

    /// <summary>
    /// 创建此属性的赋值表达式
    /// </summary>
    /// <exception cref="Exception">属性不可写</exception>
    public static Action<object, object> CreateSetPropExp(this PropertyInfo prop)
    {
        if (prop.CanWrite == false)
            throw new Exception(prop.DeclaringType.FullName + "   " + prop.Name + "不可写");
        return CreateSetAction(prop, prop.DeclaringType, prop.PropertyType);
    }

    /// <summary>
    /// 创建此字段的赋值表达式
    /// </summary>
    public static Action<object, object> CreateSetFieldExp(this FieldInfo field)
    {
        return CreateSetAction(field, field.DeclaringType, field.FieldType);
    }
    
    /// <summary>
    /// 创建Set表达式树
    /// </summary>
    /// <param name="info"> 字段或属性 </param>
    /// <param name="typeType"> 此字段所属的类型 </param>
    /// <param name="valueType"> 此字段的类型  </param>
    /// <returns></returns>
    private static Action<object, object> CreateSetAction(MemberInfo info, Type typeType, Type valueType)
    {
        var leftExp = Expression.Parameter(typeof(object), "obj");
        var rightExp = Expression.Parameter(typeof(object), "value");

        //转换type
        var objExp = typeType.IsValueType
            ? Expression.Unbox(leftExp, typeType)
            : Expression.TypeAs(leftExp, typeType);

        //转换value
        var valueExp = valueType.IsValueType
            ? Expression.Unbox(leftExp, valueType)
            : Expression.TypeAs(leftExp, valueType);

        var fieldExp = Expression.Field(leftExp, info.Name);
        var enenExp = Expression.Assign(fieldExp, valueExp);

        var finExp = Expression.Lambda<Action<object, object>>(enenExp, leftExp, rightExp);
        return finExp.Compile();
    }



    //public static Action<TType, TValue> CreateSetFieldExp<TType, TValue>(this FieldInfo field)
    //{
    //    var fieldType = field.FieldType;
    //    var typeType = field.DeclaringType;
    //
    //    var leftParExp = Expression.Parameter(typeof(object), "obj"); //创建左子树 接收目标对象
    //    var rightParExp = Expression.Parameter(typeof(object), "value"); //创建右子树 接收值
    //
    //    //如果使用 object object则要对目标类型(字段拥有者)进行转换 泛型不用
    //    //Expression typeAsExp;
    //    //if (typeType.IsValueType) {
    //    //    typeAsExp = Expression.Unbox(leftParExp, typeType);
    //    //} else {
    //    //    typeAsExp = Expression.TypeAs(leftParExp, typeType);
    //    //}
    //    //var covExp = Expression.Convert(rightParExp, fieldType);
    //
    //
    //    //obj.fieldName
    //    var fieldExp = Expression.Field(leftParExp, field.Name); //typeAsExp 
    //
    //    //obj.fieldName = value;
    //    var assignExp = Expression.Assign(fieldExp, rightParExp); //covExp
    //
    //    var f = Expression.Lambda<Action<TType, TValue>>(assignExp, leftParExp, rightParExp);
    //    return f.Compile();
    //}


    /// <summary>
    /// 从流中读取一个字段，并赋值
    /// </summary>
    /// <param name="br"> 流 </param>
    /// <param name="field"> 字段 </param>
    /// <param name="tarObj"> 目标对象 </param>
    public static object Read(this BinaryReader br, FieldInfo field, object tarObj)
    {
        var methodInfo = ReadMethod[field.FieldType];
        object value = null;
        if (methodInfo.GetParameters().Length == 1) {
            value = methodInfo.Invoke(null, [br]);
        } else {
            value = methodInfo.Invoke(br, []);
        }
        field.SetValue(tarObj, value);
        return value;
    }

    /// <summary>
    /// 从流中读取一个属性，并赋值
    /// </summary>
    /// <param name="br"> 流 </param>
    /// <param name="prop"> 属性 </param>
    /// <param name="tarObj"> 对象 </param>
    public static object Read(this BinaryReader br, PropertyInfo prop, object tarObj)
    {
        if (br.BaseStream.Position >= br.BaseStream.Length)
            return null;
        var methodInfo = ReadMethod[prop.PropertyType];
        object value = null;
        if (methodInfo.GetParameters().Length == 1) {
            value = methodInfo.Invoke(null, [br]);
        } else {
            value = methodInfo.Invoke(br, []);
        }
        prop.SetValue(tarObj, value);
        return value;
    }

    /// <summary>
    /// 往流写一个字段
    /// </summary>
    /// <param name="bw"> 流 </param>
    /// <param name="field"> 字段 </param>
    /// <param name="value"> 值 </param>
    public static void Write(this BinaryWriter bw, FieldInfo field, object value)
    {
        var methodinfo = WriteMethod[field.FieldType];
        if (methodinfo.GetParameters().Length == 2) {
            methodinfo.Invoke(null, [bw, value]);
            return;
        }
        methodinfo.Invoke(bw, [value]);
    }

    /// <summary>
    /// 往流中写一个属性
    /// </summary>
    /// <param name="bw"> 流 </param>
    /// <param name="prop"> 属性 </param>
    /// <param name="value"> 值 </param>
    public static void Write(this BinaryWriter bw, PropertyInfo prop, object value)
    {
        var methodinfo = WriteMethod[prop.PropertyType];
        if (methodinfo.GetParameters().Length == 2) {
            methodinfo.Invoke(null, [bw, value]);
            return;
        }
        methodinfo.Invoke(bw, [value]);
    }

    private static void Write(this BinaryWriter bw, MemberInfo info, object value)
    {
        MethodInfo methodinfo = info.MemberType switch
        {
            MemberTypes.Field => WriteMethod[(info as FieldInfo).FieldType],
            MemberTypes.Property => WriteMethod[(info as PropertyInfo).PropertyType],
            _ => throw new Exception($"不支持的类型! {info.MemberType}, 只能是属性或者字段!"),
        };

        if (methodinfo.GetParameters().Length == 2) {
            methodinfo.Invoke(null, [bw, value]);
            return;
        }
        methodinfo.Invoke(bw, [value]);
    }
}
