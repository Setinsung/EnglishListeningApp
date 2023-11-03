using Commons.ASPNETCore;
using Commons.Helpers.Validators;
using Listening.Domain;
using Listening.WebAPI.Main.Controllers.Albums.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Listening.WebAPI.Main.Controllers.Albums;

[Route("api/[controller]")]
[ApiController]
public class AlbumsController : ControllerBase
{
    private readonly IListeningRepository _listeningRepository;
    private readonly IMemoryCache _memoryCache;

    public AlbumsController(IListeningRepository listeningRepository, IMemoryCache memoryCache)
    {
        this._listeningRepository = listeningRepository;
        this._memoryCache = memoryCache;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlbumVM>> FindById([RequiredGuid] Guid id)
    {
        var album = await _memoryCache.GetOrCreateWithRandomExpiryAsync(
            $"AlbumsController.FindById.{id}",
            async (e) => AlbumVM.Create(await _listeningRepository.GetAlbumByIdAsync(id))
        );
        if(album == null) return NotFound("专辑不存在");
        return Ok(album);
    }

    [HttpGet("/list")]
    public async Task<ActionResult<IEnumerable<AlbumVM?>>> FindAllByCategoryId([RequiredGuid] [FromQuery] Guid categoryId)
    {
        var albums = await _memoryCache.GetOrCreateWithRandomExpiryAsync(
            $"AlbumsController.FindByCategoryId.{categoryId}",
            async (e) => AlbumVM.Create(await _listeningRepository.GetAlbumsByCategoryIdAsync(categoryId))
        );
        if (albums == null) return NotFound("专辑不存在");
        return Ok(albums);
    }
}
