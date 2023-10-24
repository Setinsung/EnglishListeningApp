using Listening.Domain.ValueObjects;
using Opportunity.LrcParser;

namespace Listening.Domain.Helpers;

/// <summary>
/// 提供将 LRC 字幕解析为句子的功能。
/// </summary>
public class LrcParser : ISubtitleParser
{
    public bool Accept(string typeName)
    {
        return typeName.Equals("lrc", StringComparison.OrdinalIgnoreCase);
    }

    public IEnumerable<Sentence> Parse(string subtitle)
    {
        var lyrics = Lyrics.Parse(subtitle);
        if (lyrics.Exceptions.Count > 0) throw new ApplicationException("lrc解析失败");
        lyrics.Lyrics.PreApplyOffset(); // 应用如[offset:500]的偏移
        return FromLrc(lyrics.Lyrics);
    }


    /// <summary>
    /// 从 LRC 歌词对象中生成句子集合。
    /// </summary>
    /// <param name="lyrics">LRC 歌词对象。</param>
    /// <returns>包含生成的句子的可枚举集合。</returns>
    private static IEnumerable<Sentence> FromLrc(Lyrics<Line> lyrics)
    {
        var lines = lyrics.Lines;
        var sentences = new List<Sentence>();
        for (int i = 0; i < lines.Count - 1; i++)
        {
            var line = lines[i];
            var nextLine = lines[i + 1];
            Sentence sentence = new(line.Timestamp.TimeOfDay, nextLine.Timestamp.TimeOfDay, line.Content);
            sentences.Add(sentence);
        }

        var lastLine = lines.Last();
        TimeSpan lastLineStartTime = lastLine.Timestamp.TimeOfDay;
        // 假设最后一句耗时1分钟
        TimeSpan lastLineEndTime = lastLineStartTime.Add(TimeSpan.FromMinutes(1));
        var lastSentence = new Sentence(lastLineStartTime, lastLineEndTime, lastLine.Content);
        sentences.Add(lastSentence);
        return sentences;
    }
}
