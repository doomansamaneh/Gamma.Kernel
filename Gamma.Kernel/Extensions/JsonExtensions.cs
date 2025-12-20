using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gamma.Kernel.Extensions;

public static class JsonOptions
{
    public static JsonSerializerOptions GetJsonSerializerOptions(bool useCurrentCulture = false)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        options.Converters.Add(new CustomGuidConverter());
        options.Converters.Add(new CustomIntConverter());
        options.Converters.Add(new CustomIntNullConverter());
        options.Converters.Add(new CustomDecimalConverter());
        options.Converters.Add(new CustomDecimalNullConverter());
        options.Converters.Add(new CustomBoolConverter());
        options.Converters.Add(new CustomStringConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        if (useCurrentCulture)
        {
            options.Converters.Add(new CustomDateTimeConverter());
            options.Converters.Add(new CustomDateTimeNullConverter());
        }
        else
        {
            options.Converters.Add(new ADDateTimeConverter());
            options.Converters.Add(new ADDateTimeNullConverter());
        }

        return options;
    }
}

public class CustomGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        return val.HasValue() ? val.ToGuid() : null;
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteStringValue(value.Value.ToString());
        else writer.WriteNullValue();
    }
}

public class CustomIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : reader.GetString().ToInt();

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CustomIntNullConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Number ? reader.GetInt32() : reader.GetString().ToInt();

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteNumberValue(value.Value);
        else writer.WriteNullValue();
    }
}

public class CustomDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Number ? reader.GetDecimal() : reader.GetString().ToDecimal();

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CustomDecimalNullConverter : JsonConverter<decimal?>
{
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Number ? reader.GetDecimal() : reader.GetString().ToDecimal();

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteNumberValue(value.Value);
        else writer.WriteNullValue();
    }
}

public class CustomBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            _ => reader.GetString().ConvertToBoolean()
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
}

public sealed class CustomStringConverter : JsonConverter<string>
{
    public override string Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt64().ToString(),
            JsonTokenType.String => reader.GetString() ?? string.Empty,
            JsonTokenType.Null => string.Empty,
            _ => throw new JsonException(
                    $"Unexpected token {reader.TokenType} when parsing string")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        string value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(value);
}


public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] Formats = ["yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd", "yyyy-MM-dd"];
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        if (!string.IsNullOrWhiteSpace(val) && DateTime.TryParseExact(val, Formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt))
            return dt;
        return DateTime.Now;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (value != DateTime.MinValue)
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        else
            writer.WriteNullValue();
    }
}

public class CustomDateTimeNullConverter : JsonConverter<DateTime?>
{
    private static readonly string[] Formats = ["yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd", "yyyy-MM-dd"];
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var val = reader.GetString();
        if (!string.IsNullOrWhiteSpace(val) && DateTime.TryParseExact(val, Formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt))
            return dt;
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        else
            writer.WriteNullValue();
    }
}

internal class ADDateTimeConverter : CustomDateTimeConverter { }
internal class ADDateTimeNullConverter : CustomDateTimeNullConverter { }
