using Listening.Domain.ValueObjects;

namespace Listening.Domain.Helpers;

public interface ISubtitleParser
{
    /// <summary>
    /// 判断解析器是否能够解析指定格式类型的字幕
    /// </summary>
    /// <param name="typeName">格式类型名称</param>
    /// <returns>如果解析器能够解析指定格式类型的字幕，则返回 true；否则返回 false。</returns>
    bool Accept(string typeName);


    /// <summary>
    /// 解析给定的字幕内容，并返回句子的集合
    /// </summary>
    /// <param name="subtitle">要解析的字幕内容</param>
    /// <returns>从字幕中解析出的句子的集合</returns>
    IEnumerable<Sentence> Parse(string subtitle);
}
