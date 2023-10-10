namespace Commons.Helpers;

public static class FormattableStringHelper
{
    /// <summary>
    /// 根据给定的 <paramref name="urlFormat"/> 和参数构建URL，自动转义其中的特殊字符
    /// </summary>
    /// <param name="urlFormat">带有参数占位符的URL格式字符串。</param>
    /// <returns>格式化后的URL字符串。</returns>
    public static string BuildUrl(FormattableString urlFormat)
    {
        // 从urlFormat中获取参数，并将其转换为不受语言环境影响的字符串
        var invariantParameters = urlFormat.GetArguments()
            .Select(e => FormattableString.Invariant($"{e}"));
        // 对参数进行URI编码将特殊字符进行转义
        object[] escapedParameters = invariantParameters
            .Select(e => (object)Uri.EscapeDataString(e)).ToArray();
        // 使用参数替换urlFormat中的占位符
        return string.Format(urlFormat.Format, escapedParameters);
    }
}
