using FluentValidation;

namespace Commons.Helpers.Validators;

public static class UriValidatorExtensions
{
    /// <summary>
    /// 验证Uri对象是否不为空。
    /// </summary>
    /// <typeparam name="T">验证对象类型。</typeparam>
    /// <param name="ruleBuilder">规则构建器。</param>
    /// <returns>规则构建器选项。</returns>
    public static IRuleBuilderOptions<T, Uri> NotEnptyUri<T>(this IRuleBuilder<T, Uri> ruleBuilder)
        => ruleBuilder.Must(
                p => p == null
                || !string.IsNullOrWhiteSpace(p.OriginalString)
           ).WithMessage("Uri不能为null或者为空");


    /// <summary>
    /// 验证Uri对象的长度是否在指定范围内。
    /// </summary>
    /// <typeparam name="T">验证对象类型。</typeparam>
    /// <param name="ruleBuilder">规则构建器。</param>
    /// <param name="min">Uri的最小长度。</param>
    /// <param name="max">Uri的最大长度。</param>
    /// <returns>规则构建器选项。</returns>
    public static IRuleBuilderOptions<T, Uri> Length<T>(this IRuleBuilder<T, Uri> ruleBuilder, int min, int max)
        => ruleBuilder.Must(
                p => string.IsNullOrEmpty(p.OriginalString)
                || p.OriginalString.Length >= min && p.OriginalString.Length <= max
           ).WithMessage($"Uri的长度必须在{min}和{max}之间");
}
