using Commons.ASPNETCore;
using Commons.Helpers.Validators;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Listening.WebAPI.Admin.Controllers.Categories.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Listening.WebAPI.Admin.Controllers.Categories;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWorkFilter(typeof(ListeningDbContext))]
public class CategoriesController : ControllerBase
{
    private readonly ListeningDomainService _listeningDomainService;
    private readonly IListeningRepository _listeningRepository;

    public CategoriesController(ListeningDomainService listeningDomainService, IListeningRepository listeningRepository)
    {
        this._listeningDomainService = listeningDomainService;
        this._listeningRepository = listeningRepository;
    }

    [HttpGet]
    public async Task<ActionResult<Category[]>> FindAll()
    {
        Category[] categories = await _listeningRepository.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> FindById([RequiredGuid] Guid id)
    {
        Category? category = await _listeningRepository.GetCategoryByIdAsync(id);
        if(category == null) return NotFound("类别未找到");
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Add(CategoryAddRequest req)
    {
        Category category = await _listeningDomainService.AddCategoryAsync(req.Name, req.CoverUri);
        return Ok(category);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update([RequiredGuid] Guid id, CategoryUpdateRequest req)
    {
        Category? category = await _listeningRepository.GetCategoryByIdAsync(id);
        if (category == null) return NotFound("类别不存在");
        category.ChangeName(req.Name);
        category.ChangeCoverUrl(req.CoverUrl);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById([RequiredGuid] Guid id)
    {
        Category? category = await _listeningRepository.GetCategoryByIdAsync(id);
        if (category == null) return NotFound("类别不存在");
        category.SoftDelete();
        return Ok();
    }

    [HttpPut("sort")]
    public async Task<ActionResult> Sort(CategoriesSortRequest req)
    {
        await _listeningDomainService.SortCategoriesAsync(req.SortedCategoryIds);
        return Ok();
    }

}
