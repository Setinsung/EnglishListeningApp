using System.ComponentModel.DataAnnotations;

namespace Commons.Helpers.Validators;

/// <summary>
/// 用于验证属性、字段或参数是否为非空的 Guid 值。
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredGuidAttribute : ValidationAttribute
{
    public const string DefaultErrorMessage = "The {0} field is requird and not Guid.Empty";
    public RequiredGuidAttribute() : base(DefaultErrorMessage) { }

    public override bool IsValid(object? value) => value switch
    {
        null => false,
        Guid guid => guid != Guid.Empty,
        _ => false
    };
}
