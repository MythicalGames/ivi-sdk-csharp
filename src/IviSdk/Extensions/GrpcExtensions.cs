using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mythical.Game.IviSdkCSharp;

public static class GrpcExtensions
{
    internal static decimal ToDecimal(this string value)
        => decimal.TryParse(value, out var result) ? result : 0m;

    internal static Struct ToProtoStruct(this IDictionary<string, object> value)
    {
        var result = new Struct();
        if (value != null)
        {
            foreach (var (k, v) in value)
            {
                result.Fields.Add(k, v.ToProtoValue());
            }
        }
        return result;
    }

    internal static Dictionary<string, object> ToDictionary(this Struct value)
    {
        var result = new Dictionary<string, object>();
        if (value != null)
        {
            foreach (var (k, v) in value.Fields)
            {
                result.Add(k, v.ToObject()!);
            }
        }
        return result;
    }

    internal static object? ToObject(this Value value)
    {
        switch (value.KindCase)
        {
            case Value.KindOneofCase.BoolValue: return value.BoolValue;
            case Value.KindOneofCase.ListValue: return value.ListValue.Values.Select(v => v.ToObject()).ToList();
            case Value.KindOneofCase.None:
            case Value.KindOneofCase.NullValue: return null;
            case Value.KindOneofCase.NumberValue: return value.NumberValue;
            case Value.KindOneofCase.StringValue: return value.StringValue;
            case Value.KindOneofCase.StructValue: return value.StructValue.ToDictionary();
        }
        throw new InvalidOperationException($"Unhandled value type {value.KindCase}");
    }

    internal static Value ToProtoValue(this object value)
    {
        switch (value)
        {
            case string strVal: return Value.ForString(strVal);
            case bool boolVal: return Value.ForBool(boolVal);
            case long longVal: return Value.ForNumber(longVal);
            case double dblVal: return Value.ForNumber(dblVal);
            case decimal decVal: return Value.ForNumber(Convert.ToDouble(decVal));
            case int numVal: return Value.ForNumber(numVal);
            case IEnumerable listVal: return Value.ForList(listVal.Cast<object>().Select(v => v.ToProtoValue()).ToArray());
            default:
                if (value == null)
                {
                    return Value.ForNull();
                }
                break;
        }
        throw new InvalidOperationException();
    }
}