using Commons.Domain.Models;
using FluentValidation;
using Listening.Infrastructure;

namespace Listening.WebAPI.Admin.Controllers.Albums.ViewModels;

public record AlbumAddRequest(MultilingualString Name, Guid CategoryId);
public class AlbumAddRequestValidator : AbstractValidator<AlbumAddRequest>
{
    public AlbumAddRequestValidator(ListeningDbContext dbCtx)
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Name.Chinese).NotNull().Length(1, 200);
        RuleFor(x => x.Name.English).NotNull().Length(1, 200);
    }
}
