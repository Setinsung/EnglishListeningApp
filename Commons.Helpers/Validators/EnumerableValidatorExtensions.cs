using FluentValidation;

namespace Commons.Helpers.Validators;

public static class EnumerableValidatorExtensions
{
    /// <summary>
    /// 验证可枚举对象是否不包含重复的元素。
    /// </summary>
    /// <typeparam name="T">验证对象类型。</typeparam>
    /// <typeparam name="TItem">可枚举对象中元素的类型。</typeparam>
    /// <param name="ruleBuilder">规则构建器。</param>
    /// <returns>规则构建器选项。</returns>
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> NotDuplicated<T, TItem>(this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder)
        => ruleBuilder.Must(p => p == null || p.Distinct().Count() == p.Count());

    /// <summary>
    /// 验证可枚举对象是否不包含指定的比较值。
    /// </summary>
    /// <typeparam name="T">验证对象类型。</typeparam>
    /// <typeparam name="TItem">可枚举对象中元素的类型。</typeparam>
    /// <param name="ruleBuilder">规则构建器。</param>
    /// <param name="comparedValue">要比较的值。</param>
    /// <returns>规则构建器选项。</returns>
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> NotContains<T, TItem>(this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder, TItem comparedValue)
        => ruleBuilder.Must(p => p == null || !p.Contains(comparedValue));

    /// <summary>
    /// 验证可枚举对象是否包含指定的比较值。
    /// </summary>
    /// <typeparam name="T">验证对象类型。</typeparam>
    /// <typeparam name="TItem">可枚举对象中元素的类型。</typeparam>
    /// <param name="ruleBuilder">规则构建器。</param>
    /// <param name="comparedValue">要比较的值。</param>
    /// <returns>规则构建器选项。</returns>
    public static IRuleBuilderOptions<T, IEnumerable<TItem>> Contains<T, TItem>(this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder, TItem comparedValue)
    => ruleBuilder.Must(p => p == null || p.Contains(comparedValue));

}
