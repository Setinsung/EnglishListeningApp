using Commons.Domain.Models;
using Commons.Helpers.Validators;
using FluentValidation;

namespace Listening.WebAPI.Admin.Controllers.Categories.ViewModels;

public record CategoryAddRequest(MultilingualString Name, Uri CoverUri);

public class CategoryAddRequestValidator : AbstractValidator<CategoryAddRequest>
{
    public CategoryAddRequestValidator()
    {
        RuleFor(x =>x.Name).NotEmpty();
        RuleFor(x => x.Name.Chinese).NotNull().Length(1, 200);
        RuleFor(x => x.Name.English).NotNull().Length(1, 200);
        RuleFor(x => x.CoverUri).Length(5, 500);
    }
}
