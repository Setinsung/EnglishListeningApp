namespace Commons.Helpers;

public static class StringExtensions
{
    /// <summary>
    /// 比较两个字符串是否相等，忽略大小写。
    /// </summary>
    /// <param name="s1">要比较的第一个字符串。</param>
    /// <param name="s2">要比较的第二个字符串。</param>
    /// <returns>如果两个字符串相等，则为true；否则为false。</returns>
    public static bool EqualsIgnoreCase(this string s1, string s2)
    {
        return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
    }

    ///////
    /// <summary>
    /// 截取字符串的一部分。
    /// </summary>
    /// <param name="s1">要截取的原始字符串。</param>
    /// <param name="maxLen">要截取的最大长度。</param>
    /// <returns>截取后的字符串。</returns>
    public static string Cut(this string s1, int maxLen)
    {
        if (s1 == null)
        {
            return string.Empty;
        }
        int len = s1.Length <= maxLen ? s1.Length : maxLen; // 不能超过字符串的最大大小
        return s1[0..len];
    }
}
