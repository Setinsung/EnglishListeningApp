using Listening.Domain.ValueObjects;
using System.Text;

namespace Listening.Domain.Helpers;


/// <summary>
/// 提供将 SRT 字幕解析为句子的功能。
/// </summary>
public class SrtParser : ISubtitleParser
{
    public bool Accept(string typeName)
    {
        return typeName.Equals("srt", StringComparison.OrdinalIgnoreCase) ||
            typeName.Equals("vtt", StringComparison.OrdinalIgnoreCase);
    }

    public IEnumerable<Sentence> Parse(string subtitle)
    {
        var srtParser = new SubtitlesParser.Classes.Parsers.SubParser();
        using MemoryStream ms = new(Encoding.UTF8.GetBytes(subtitle));
        var items = srtParser.ParseStream(ms);
        return items.Select(s => new Sentence(TimeSpan.FromMilliseconds(s.StartTime), TimeSpan.FromMilliseconds(s.EndTime), string.Join(" ", s.Lines)));
    }
}
