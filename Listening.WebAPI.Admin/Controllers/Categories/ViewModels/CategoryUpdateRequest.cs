using Commons.Domain.Models;
using Commons.Helpers.Validators;
using FluentValidation;

namespace Listening.WebAPI.Admin.Controllers.Categories.ViewModels;

public record CategoryUpdateRequest(MultilingualString Name, Uri CoverUrl);

public class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
{
    public CategoryUpdateRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Name.Chinese).NotNull().Length(1, 200);
        RuleFor(x => x.Name.English).NotNull().Length(1, 200);
        RuleFor(x => x.CoverUrl).Length(5, 500);
    }
}