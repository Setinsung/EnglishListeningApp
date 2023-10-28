using Commons.Domain.Models;
using FluentValidation;

namespace Listening.WebAPI.Admin.Controllers.Episodes.ViewModels;

public record EpisodeAddRequest(MultilingualString Name, Guid AlbumId, Uri AudioUrl, double DurationInSecond, string Subtitle, string SubtitleType);

public class EpisodeAddRequestValidator : AbstractValidator<EpisodeAddRequest>
{
    public EpisodeAddRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Name.Chinese).NotNull().Length(1, 200);
        RuleFor(x => x.Name.English).NotNull().Length(1, 200);
        RuleFor(x => x.DurationInSecond).GreaterThan(0);
        RuleFor(x => x.SubtitleType).NotEmpty().Length(1, 10);
        RuleFor(x => x.Subtitle).NotEmpty();
    }
}