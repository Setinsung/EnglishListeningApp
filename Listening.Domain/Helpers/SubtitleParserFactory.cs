namespace Listening.Domain.Helpers;

/// <summary>
/// 字幕解析器工厂，用于创建和获取字幕解析器。
/// </summary>
public class SubtitleParserFactory
{
    private static readonly List<ISubtitleParser> parsers = new();


    /// <summary>
    /// 静态构造函数，用于扫描程序集中的所有实现了 ISubtitleParser 接口的类并创建对应的对象。
    /// </summary>
    static SubtitleParserFactory()
    {
        // 扫描当前程序集中的所有实现了ISubtitleParser接口的类
        IEnumerable<Type> parserTypes = typeof(SubtitleParserFactory).Assembly.GetTypes()
            .Where(t => typeof(ISubtitleParser).IsAssignableFrom(t) && !t.IsAbstract);

        // 创建这些对象添加到parsers
        foreach (Type parserType in parserTypes)
        {
            ISubtitleParser? parser = Activator.CreateInstance(parserType) as ISubtitleParser;
            if(parser != null) parsers.Add(parser);
        }
    }

    /// <summary>
    /// 根据给定的类型名称获取对应的字幕解析器。
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <returns>字幕解析器实例，如果找不到则返回 null。</returns>
    public static ISubtitleParser? GetParser(string typeName)
    {
        foreach (var parser in parsers)
        {
            if(parser.Accept(typeName)) return parser;
        }
        return null;
    }

}
