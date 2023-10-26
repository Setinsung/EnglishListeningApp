using Commons.ASPNETCore;
using Commons.Helpers.Validators;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Listening.WebAPI.Admin.Controllers.Albums.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Listening.WebAPI.Admin.Controllers.Albums;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWorkFilter(typeof(ListeningDbContext))]
public class AlbumsController : ControllerBase
{
    private readonly IListeningRepository _listeningRepository;
    private readonly ListeningDomainService _listeningDomainService;

    public AlbumsController(IListeningRepository listeningRepository, ListeningDomainService listeningDomainService)
    {
        this._listeningRepository = listeningRepository;
        this._listeningDomainService = listeningDomainService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Album?>> FindById([RequiredGuid] Guid id)
    {
        var album = await _listeningRepository.GetAlbumByIdAsync(id);
        return Ok(album);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<Album[]>> FindByCategoryId([RequiredGuid] Guid categoryId)
    {
        var albums = await _listeningRepository.GetAlbumsByCategoryIdAsync(categoryId);
        return Ok(albums);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Add(AlbumAddRequest req)
    {
        Album? album = await _listeningDomainService.AddAlbumAsync(req.CategoryId, req.Name);
        if (album == null) return NotFound("类别未找到");
        return Ok(album.Id);
    }

    [HttpPut]
    public async Task<ActionResult> Update([RequiredGuid] Guid id, AlbumUpdateRequest request)
    {
        var album = await _listeningRepository.GetAlbumByIdAsync(id);
        if (album == null) return NotFound("专辑未找到");
        album.ChangeName(request.Name);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById([RequiredGuid] Guid id)
    {
        var album = await _listeningRepository.GetAlbumByIdAsync(id);
        if (album == null) return NotFound("专辑未找到");
        album.SoftDelete();
        return Ok();
    }

    [HttpPut("{id}/hide")]
    public async Task<ActionResult> Hide([RequiredGuid] Guid id)
    {
        var album = await _listeningRepository.GetAlbumByIdAsync(id);
        if (album == null) return NotFound("专辑未找到");
        album.Hide();
        return Ok();
    }

    [HttpPut("{id}/show")]
    public async Task<ActionResult> Show([RequiredGuid] Guid id)
    {
        var album = await _listeningRepository.GetAlbumByIdAsync(id);
        if (album == null) return NotFound("专辑未找到");
        album.Show();
        return Ok();
    }

    [HttpPut("/sort/{categoryId}")]
    public async Task<ActionResult> Sort([RequiredGuid] Guid categoryId, AlbumsSortRequest req)
    {
        await _listeningDomainService.SortAlbumsAsync(categoryId, req.SortedAlbumIds);
        return Ok();
    }
}
