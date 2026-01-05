using System.Dynamic;
using System.Reflection;
using Gamma.Kernel.Models;

namespace Gamma.Kernel.Extensions;

public static class ObjectExtensions
{
    public static void LogCreatedBy(this object entity, Models.LogActorModel actor, System.DateTime logTime)
    {
        var createdBy = entity.GetPropertyValue(Constants.LOG_CREATED_BY_FIELD) as string;
        if (string.IsNullOrWhiteSpace(createdBy))
        {
            entity.SetPropertyValue(Constants.LOG_CREATED_BY_FIELD, actor.Serialize());
            entity.SetPropertyValue(Constants.LOG_DATE_CREATED_FIELD, logTime);
        }
    }

    public static void LogModifiedBy(this object entity, Models.LogActorModel actor, System.DateTime logTime)
    {
        entity.SetPropertyValue(Constants.LOG_MODIFIED_BY_FIELD, actor.Serialize());
        entity.SetPropertyValue(Constants.LOG_DATE_MODIFIED_FIELD, logTime);
    }

    public static IDictionary<string, object>? ToDictionary(this object? source)
    {
        if (source is null)
            return null;

        // Exact match
        if (source is IDictionary<string, object> dict)
            return dict;

        // ExpandoObject (nullable-safe)
        if (source is ExpandoObject expando)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in expando) result[kv.Key] = kv.Value!;
            return result;
        }

        // Other IDictionary types
        if (source is System.Collections.IDictionary genericDict)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in genericDict.Keys)
            {
                if (key != null)
                    result[key.ToString()!] = genericDict[key]!;
            }
            return result;
        }

        // POCO / anonymous object
        return source
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
            .ToDictionary(
                p => p.Name,
                p => p.GetValue(source)!,
                StringComparer.OrdinalIgnoreCase
            );
    }

    public static T? MapToType<T>(this object source)
    {
        var str = source.Serialize();
        var tp = str.Deserialize<T>();
        return tp;
    }

    public static string Serialize(this object source)
    {
        if (source == null) return "";
        var options = JsonOptions.GetJsonSerializerOptions();
        return System.Text.Json.JsonSerializer.Serialize(source, options);
    }

    public static string SerializeDynamic(dynamic source)
    {
        if (source == null) return "";
        var options = JsonOptions.GetJsonSerializerOptions();
        return System.Text.Json.JsonSerializer.Serialize(source, options);
    }

    public static bool IsNullOrEmpty(this object value)
    {
        var result = false;
        if (value == null)
        {
            result = true;
        }

        if (value is string v)
        {
            result = string.IsNullOrEmpty(v);
        }
        return result;
    }

    public static List<string> GetFieldList(this Type modelType, Type attributeType)
    {
        return modelType.GetProperties().Where(c => c.GetCustomAttributes(false).Any(attr => attr.GetType() == attributeType)).Select(f => f.Name).ToList();
    }

    public static List<string> GetFieldList(this Type modelType)
    {
        return modelType.GetProperties().Select(f => f.Name).ToList();
    }

    public static Result<EmptyUnit> ToEmptyUnit<T>(this Result<T> result)
    {
        return result.Success
            ? Result<EmptyUnit>.Ok(default)
            : Result<EmptyUnit>.Fail(result.Errors, result.Message);
    }
}