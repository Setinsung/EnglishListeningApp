using Listening.Domain.ValueObjects;
using System.Text.Json;

namespace Listening.Domain.Helpers;

/// <summary>
/// 提供将 JSON 字幕解析为句子的功能。
/// </summary>
public class JsonParser : ISubtitleParser
{
    public bool Accept(string typeName)
    {
        return typeName.Equals("json", StringComparison.OrdinalIgnoreCase);
    }

    public IEnumerable<Sentence> Parse(string subtitle)
    {
        return JsonSerializer.Deserialize<IEnumerable<Sentence>>(subtitle)
            ?? throw new ApplicationException("json解析失败");
    }
}
