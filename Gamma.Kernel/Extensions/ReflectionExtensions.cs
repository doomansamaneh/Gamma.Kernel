using System;
using System.Globalization;
using System.Reflection;

namespace Gamma.Kernel.Extensions;

public static class ReflectionExtensions
{
    public static void NormalizeStringProperties(this object entity)
    {
        if (entity == null) return;

        var stringProperties = entity.GetType()
                                     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(p => p.CanRead)
                                     .Where(p => p.CanWrite)
                                     .Where(p => p.PropertyType == typeof(string));

        foreach (var prop in stringProperties)
        {
            var val = (string?)prop.GetValue(entity);
            if (!string.IsNullOrWhiteSpace(val))
            {
                prop.SetValue(entity, val.ToNormalString().Trim());
            }
        }
    }

    public static object? GetPropertyValue(
        this object? obj,
        string propertyName,
        object? defaultValue = null,
        bool enforceNullStringToDefaultValue = false)
    {
        if (obj is null || string.IsNullOrWhiteSpace(propertyName))
            return defaultValue;

        var property = obj.GetType()
                          .GetProperty(propertyName,
                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (property == null || !property.CanRead)
            return defaultValue;

        var result = property.GetValue(obj);

        if (enforceNullStringToDefaultValue)
        {
            if (result == null)
                return defaultValue;

            if (result is string s && string.IsNullOrWhiteSpace(s))
                return defaultValue;
        }

        return result ?? defaultValue;
    }

    public static bool SetPropertyValue(
        this object obj,
        string propertyName,
        object? value)
    {
        if (obj == null || string.IsNullOrWhiteSpace(propertyName))
            return false;

        var property = obj.GetType()
                          .GetProperty(propertyName,
                              BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (property == null || !property.CanWrite)
            return false;

        try
        {
            // Handle nullable and type conversion
            var targetType = Nullable.GetUnderlyingType(property.PropertyType)
                             ?? property.PropertyType;

            if (value != null && !targetType.IsAssignableFrom(value.GetType()))
            {
                value = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }

            property.SetValue(obj, value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
