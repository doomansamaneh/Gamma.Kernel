using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Gamma.Kernel.Enums;
using Ganss.Xss;

namespace Gamma.Kernel.Extensions;

public static class StringExtensions
{
    private static readonly char NonNormalKaf = (char)1603;
    private static readonly char NormalKaf = (char)1705;

    private static readonly char NonNormalYah = (char)1610;
    private static readonly char NormalYah = (char)1740;

    private static readonly char[] EasternDigits = [(char)1776, (char)1777, (char)1778, (char)1779, (char)1780, (char)1781, (char)1782, (char)1783, (char)1784, (char)1785];
    private static readonly char[] WesternDigits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    public static string ToNormalString(this string strInput, bool sanitize = true)
    {
        if (string.IsNullOrEmpty(strInput))
            return strInput;

        var result = strInput.Replace(NonNormalKaf, NormalKaf)
                             .Replace(NonNormalYah, NormalYah);

        for (int i = 0; i < EasternDigits.Length; i++)
        {
            result = result.Replace(EasternDigits[i], WesternDigits[i]);
        }

        if (sanitize)
        {
            result = result.Sanitize(SanitizerType.HtmlFragment);
        }

        return result;
    }

    public static string Sanitize(this string str, SanitizerType sanitizerType = SanitizerType.Html)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        string result = str;

        switch (sanitizerType)
        {
            case SanitizerType.HtmlFragment:
                {
                    var sanitizer = new HtmlSanitizer();
                    result = sanitizer.Sanitize(str);
                    break;
                }
            case SanitizerType.Html:
                result = WebUtility.HtmlEncode(str);
                break;
            case SanitizerType.Xml:
                result = WebUtility.HtmlEncode(str);
                break;
            case SanitizerType.XmlAttribute:
                result = WebUtility.HtmlEncode(str);
                break;
            case SanitizerType.URL:
                result = Uri.EscapeDataString(str);
                break;
            case SanitizerType.HtmlFormUrl:
                result = Uri.EscapeDataString(str);
                break;
        }

        return result;
    }

    public static string GetPhrase(this string mainString, string startPhrase, string endPhrase)
    {
        if (string.IsNullOrEmpty(mainString)) return string.Empty;

        try
        {
            var startIndex = mainString.IndexOf(startPhrase, StringComparison.OrdinalIgnoreCase);
            var endIndex = mainString.IndexOf(endPhrase, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1 || endIndex == -1 || endIndex <= startIndex) return string.Empty;

            return mainString.Substring(startIndex + startPhrase.Length, endIndex - startIndex - startPhrase.Length);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static bool HasValue(this string? value) => !string.IsNullOrEmpty(value);

    public static int ToInt(this string? value) => int.TryParse(value, out var result) ? result : 0;

    public static decimal ToDecimal(this string? value) => decimal.TryParse(value, out var result) ? result : 0;

    public static string ToShortTime(this TimeSpan time) => $"{time.Hours:D2}:{time.Minutes:D2}";

    public static string ToCamelCase(this string? strInput) =>
        !string.IsNullOrEmpty(strInput) ? $"{char.ToLowerInvariant(strInput[0])}{strInput[1..]}" : strInput ?? string.Empty;

    public static string ToPascalCase(this string? strInput) =>
        !string.IsNullOrEmpty(strInput) ? $"{char.ToUpperInvariant(strInput[0])}{strInput[1..]}" : strInput ?? string.Empty;

    public static Guid ToGuid(this string? id) => id.IsValidGuid() ? Guid.Parse(id!) : Guid.Empty;

    public static bool IsValidGuid(this string? str) => Guid.TryParse(str, out _);

    public static bool IsWebUrl(this string? str)
    {
        if (string.IsNullOrEmpty(str)) return false;
        string pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
        return Regex.IsMatch(str, pattern);
    }

    public static string ReplaceWithIgnoreCase(this string? str, string find, string to) =>
        str is null ? string.Empty : Regex.Replace(str, find, to, RegexOptions.IgnoreCase);

    public static string ReadyForTextArea(this string? str) => str ?? string.Empty;

    public static string ReadyForHtml(this string? str) => str?.ReplaceLineBreaks("<br/>") ?? string.Empty;

    public static string ReplaceLineBreaks(this string? lines, string replacement)
    {
        if (string.IsNullOrEmpty(lines)) return string.Empty;
        return lines.Replace("\r\n", replacement)
                    .Replace("\r", replacement)
                    .Replace("\n", replacement)
                    .Replace(Environment.NewLine, replacement);
    }

    public static bool In(this string? value, params string[] stringValues)
    {
        if (value == null) return false;
        return stringValues.Any(v => string.Equals(value, v, StringComparison.Ordinal));
    }

    public static T ToEnum<T>(this string? value) where T : struct
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        return Enum.Parse<T>(value, true);
    }

    public static string Right(this string? value, int length) =>
        value != null && value.Length > length ? value[^length..] : value ?? string.Empty;

    public static string Left(this string? value, int length) =>
        value != null && value.Length > length ? value[..length] : value ?? string.Empty;

    public static string Format(this string value, params object[] args) => string.Format(value, args);

    public static byte[] HexToBytes(this string hexString)
    {
        if (hexString.Length % 2 != 0)
            throw new ArgumentException($"HexString cannot be in odd number: {hexString}");

        var retVal = new byte[hexString.Length / 2];
        for (int i = 0; i < hexString.Length; i += 2)
            retVal[i / 2] = byte.Parse(hexString.AsSpan(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        return retVal;
    }

    public static bool ConvertToBoolean(this string? str)
    {
        if (string.IsNullOrEmpty(str)) return false;
        var normalized = str.Trim().ToLowerInvariant();
        return normalized == "true" || normalized == "1";
    }

    public static string ToNotNullString(this object? obj, string defaultValue = "") => obj?.ToString() ?? defaultValue;

    public static string ToUrlTitle(this string? str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        string result = str;
        string[] invalidItems = ["http://", "https://", "www."];
        foreach (var item in invalidItems)
            result = result.ReplaceWithIgnoreCase(item, string.Empty);
        return result;
    }

    public static string ToDictKey(this object? obj) =>
        string.IsNullOrEmpty(obj?.ToString()) ? string.Empty : obj.ToString()!.ToLowerInvariant().Trim();

    public static bool IsValidIp(this string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip) || !ip.Contains('.')) return false;
        return IPAddress.TryParse(ip, out var address) &&
               (address.AddressFamily == AddressFamily.InterNetwork ||
                address.AddressFamily == AddressFamily.InterNetworkV6);
    }

    public static bool IsValidIranianNationalCode(this string? nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode)) return false;
        if (!Regex.IsMatch(nationalCode, @"^\d{10}$")) return false;

        int check = Convert.ToInt32(nationalCode![9].ToString());
        int sum = Enumerable.Range(0, 9)
                .Sum(i => Convert.ToInt32(nationalCode[i].ToString()) * (10 - i)) % 11;

        return (sum < 2 && check == sum) || (sum >= 2 && check + sum == 11);
    }

    public static int CountStringOccurrences(this string? text, string pattern)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) return 0;
        int count = 0, i = 0;
        while ((i = text.IndexOf(pattern, i, StringComparison.Ordinal)) != -1)
        {
            i += pattern.Length;
            count++;
        }
        return count;
    }

    public static bool IsMultiExtension(this string fileName) => fileName.CountStringOccurrences(".") >= 2;

    public static string RemoveHtmlCssXss(this string? htmlIn, string? baseUrl = null)
    {
        if (htmlIn == null) return string.Empty;
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        return sanitizer.Sanitize(htmlIn, baseUrl ?? "");
    }

    public static string RemoveHtmlXss(this string? htmlIn, string? baseUrl = null)
    {
        if (htmlIn == null) return string.Empty;

        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Remove("style");
        sanitizer.AllowedAttributes.Remove("src");
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("alt");
        sanitizer.AllowedAttributes.Add("title");
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("mailto");
        sanitizer.AllowedSchemes.Add("data");
        sanitizer.AllowedTags.Remove("style");

        sanitizer.RemovingAttribute += (s, e) =>
        {
            if (e.Tag.TagName == "IMG" && e.Attribute.Name == "src")
            {
                var allowedPrefixes = new[]
                {
                    "data:image/gif",
                    "data:image/jpeg",
                    "data:image/png",
                    "data:image/jpg",
                    "http://",
                    "https://"
                };
                if (allowedPrefixes.Any(x => e.Attribute.Value.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                    e.Cancel = true;
            }
        };

        return sanitizer.Sanitize(htmlIn, baseUrl ?? "");
    }

    public static bool IsWhiteSpace(this char ch) =>
        char.IsWhiteSpace(ch);

    public static string NormalizeWhiteSpace(this string? str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        var sb = new StringBuilder();
        bool lastWasWS = false;
        foreach (var ch in str)
        {
            if (ch.IsWhiteSpace())
            {
                if (!lastWasWS)
                {
                    sb.Append(' ');
                    lastWasWS = true;
                }
            }
            else
            {
                sb.Append(ch);
                lastWasWS = false;
            }
        }
        return sb.ToString();
    }

    public static DateTime ToDateTime(this string? s, string format, CultureInfo culture)
    {
        if (!string.IsNullOrEmpty(s) && DateTime.TryParseExact(s, format, culture, DateTimeStyles.None, out var dt))
            return dt;
        return DateTime.Now;
    }

    public static TimeSpan ToTime(this string? s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            try
            {
                var parts = s.Split(':');
                int hour = parts[0].ToInt();
                int minute = parts.Length > 1 ? parts[1].ToInt() : 0;
                int second = parts.Length > 2 ? parts[2].ToInt() : 0;
                return new TimeSpan(hour, minute, second);
            }
            catch { }
        }
        return DateTime.Now.TimeOfDay;
    }

    public static string ReplaceFirst(this string? text, string search, string replace)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(search)) return text ?? string.Empty;
        int pos = text!.IndexOf(search, StringComparison.CurrentCultureIgnoreCase);
        return pos < 0 ? text : $"{text[..pos]}{replace}{text[(pos + search.Length)..]}";
    }

    public static List<int> UtfStringToIntList(this string str)
    {
        var arr = new List<int>();
        foreach (var c in str)
            arr.Add(Convert.ToInt32(c));
        return arr;
    }

    public static System.IO.Stream? ToStream(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        writer.Write(text);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static string ToSafeSqlField(this string strInput)
    {
        if (string.IsNullOrEmpty(strInput))
            return strInput;

        // Remove dangerous characters
        var result = strInput.Replace(";", "")
                             .Replace(",", "")
                             .Replace("--", "")
                             .Replace("/*", "")
                             .Replace("*/", "")
                             .Replace("xp_", "", StringComparison.OrdinalIgnoreCase)
                             .Replace(" ", "")
                             .Replace("[", "")
                             .Replace("]", "");

        // Handle comma-separated fields
        // var fields = result.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                    .Select(GetSafeSqlField);
        //return string.Join(",", fields);
        return result;
    }

    public static string ToTimeString(this int number) => $"{number / 60}:{number % 60}";
    public static string ToTimeString(this long number) => $"{number / 60}:{number % 60}";

    public static T? Deserialize<T>(this string? str, bool useCurrentCulture = false)
    {
        if (string.IsNullOrWhiteSpace(str) || str == "{}")
            return default;

        var options = JsonOptions.GetJsonSerializerOptions(useCurrentCulture);
        return System.Text.Json.JsonSerializer.Deserialize<T>(str, options);
    }
}