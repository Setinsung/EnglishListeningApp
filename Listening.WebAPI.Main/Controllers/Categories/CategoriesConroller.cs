using Commons.ASPNETCore;
using Commons.Helpers.Validators;
using Listening.Domain;
using Listening.WebAPI.Main.Controllers.Categories.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Listening.WebAPI.Main.Controllers.Categories;

[Route("api/[controller]")]
[ApiController]
public class CategoriesConroller : ControllerBase
{
    private readonly IListeningRepository _listeningRepository;
    private readonly IMemoryCache _memoryCache;

    public CategoriesConroller(IListeningRepository listeningRepository, IMemoryCache memoryCache)
    {
        this._listeningRepository = listeningRepository;
        this._memoryCache = memoryCache;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryVM>> FindById([RequiredGuid] Guid id)
    {
        var category = _memoryCache.GetOrCreateWithRandomExpiryAsync(
            $"CategoryController.FindById.{id}",
            async (e) => CategoryVM.Create(await _listeningRepository.GetCategoryByIdAsync(id))
        );
        if(category == null) return NotFound("类别不存在");
        return Ok(category);
    }

    [HttpGet("/list")]
    public async Task<ActionResult<IEnumerable<CategoryVM?>>> FindAll()
    {
        var categories = _memoryCache.GetOrCreateWithRandomExpiryAsync(
            $"CategoriesController.FindAll",
            async (e) => CategoryVM.Create(await _listeningRepository.GetCategoriesAsync())
        );
        if (categories == null) return NotFound("类别不存在");
        return Ok(categories);
    }
}
