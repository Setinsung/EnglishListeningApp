using Commons.Helpers.Validators;
using FluentValidation;

namespace Listening.WebAPI.Admin.Controllers.Categories.ViewModels;

public record CategoriesSortRequest(Guid[] SortedCategoryIds);

public class CategoriesSortRequestValidator : AbstractValidator<CategoriesSortRequest>
{
    public CategoriesSortRequestValidator()
    {
        RuleFor(r => r.SortedCategoryIds).NotNull().NotEmpty().NotContains(Guid.Empty)
            .NotDuplicated();
    }
}
