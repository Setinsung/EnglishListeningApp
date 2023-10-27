using Commons.Helpers.JsonConverters;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Commons.Helpers;

public static class JsonExtentions
{
    // 如果不设置，中文会保存为"\u96C5"
    /// <summary>
    /// 将Unicode字符编码为JSON字符串的编码器
    /// </summary>
    public readonly static JavaScriptEncoder Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

    /// <summary>
    /// 创建用于JSON序列化的JsonSerializerOptions对象。
    /// </summary>
    /// <param name="camelCase">指定是否使用驼峰命名法。</param>
    /// <returns>JsonSerializerOptions对象。</returns>
    public static JsonSerializerOptions CreateJsonSerializerOptions(bool camelCase = false)
    {
        JsonSerializerOptions opt = new JsonSerializerOptions { Encoder = Encoder };
        if (camelCase)
        {
            opt.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            opt.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }
        opt.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
        return opt;
    }

    /// <summary>
    /// 将对象转换为JSON字符串。
    /// </summary>
    /// <param name="value">要转换为JSON的对象。</param>
    /// <param name="camelCase">指定是否使用驼峰命名法。</param>
    /// <returns>转换后的JSON字符串。</returns>
    public static string ToJsonString(this object value, bool camelCase = false)
    {
        JsonSerializerOptions opt = CreateJsonSerializerOptions(camelCase);
        return JsonSerializer.Serialize(value,value.GetType(), opt);
    }

    /// <summary>
    /// 将JSON字符串解析为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">要解析的目标类型。</typeparam>
    /// <param name="value">要解析的JSON字符串。</param>
    /// <returns>解析后的对象。</returns>
    public static T? ParseJson<T>(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;
        var opt = CreateJsonSerializerOptions();
        return JsonSerializer.Deserialize<T>(value, opt);
    }

    /// <summary>
    /// 尝试将JSON字符串解析为指定类型的对象。
    /// </summary>
    /// <typeparam name="T">要解析的目标类型。</typeparam>
    /// <param name="value">要解析的JSON字符串。</param>
    /// <param name="result">解析后的对象。</param>
    /// <returns>如果解析成功，则为 true；否则为 false。</returns>
    public static bool TryParseJson<T>(this string value, out T? result)
    {
        result = default;
        try
        {
            result = ParseJson<T>(value);
        }
        catch (JsonException)
        {
            return false;
        }
        if(result == null) return false;
        return true;
    }
}
