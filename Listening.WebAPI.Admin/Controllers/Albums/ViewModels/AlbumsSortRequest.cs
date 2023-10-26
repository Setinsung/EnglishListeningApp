using Commons.Helpers.Validators;
using FluentValidation;

namespace Listening.WebAPI.Admin.Controllers.Albums.ViewModels;

public record AlbumsSortRequest(Guid[] SortedAlbumIds);

public class AlbumsSortRequestValidator : AbstractValidator<AlbumsSortRequest>
{
    public AlbumsSortRequestValidator()
    {
        RuleFor(r => r.SortedAlbumIds).NotNull().NotEmpty().NotContains(Guid.Empty)
            .NotDuplicated();
    }
}