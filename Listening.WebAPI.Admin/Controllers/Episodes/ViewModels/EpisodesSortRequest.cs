using Commons.Helpers.Validators;
using FluentValidation;

namespace Listening.WebAPI.Admin.Controllers.Episodes.ViewModels;

public record EpisodesSortRequest(Guid[] SortedEpisodeIds);

public class EpisodesSortRequestValidator : AbstractValidator<EpisodesSortRequest>
{
    public EpisodesSortRequestValidator()
    {
        RuleFor(r => r.SortedEpisodeIds).NotNull().NotEmpty()
            .NotContains(Guid.Empty).NotDuplicated();
    }
}
