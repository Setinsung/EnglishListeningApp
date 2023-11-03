using Commons.Domain.Models;
using Listening.Domain.Entities;

namespace Listening.WebAPI.Main.Controllers.Categories.ViewModels;

public record CategoryVM(Guid Id, MultilingualString Name, Uri CoverUrl)
{
    public static CategoryVM? Create(Category? category)
    {
        if (category == null) return null;
        return new CategoryVM(category.Id, category.Name, category.CoverUrl);
    }

    public static CategoryVM?[] Create(Category[] categories)
    {
        return categories.Select(Create).ToArray();
    }
}
