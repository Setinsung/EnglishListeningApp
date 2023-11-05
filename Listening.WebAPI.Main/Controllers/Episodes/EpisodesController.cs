using Commons.ASPNETCore;
using Commons.Helpers.Validators;
using Listening.Domain;
using Listening.WebAPI.Main.Controllers.Episodes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Listening.WebAPI.Main.Controllers.Episodes;

[Route("api/[controller]")]
[ApiController]
public class EpisodesController : ControllerBase
{
    private readonly IListeningRepository _listeningRepository;
    private readonly IMemoryCache _memoryCache;

    public EpisodesController(IListeningRepository listeningRepository, IMemoryCache memoryCache)
    {
        this._listeningRepository = listeningRepository;
        this._memoryCache = memoryCache;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EpisodeVM>> FindById([RequiredGuid] Guid id)
    {
        var episode = await _memoryCache.GetOrCreateWithRandomExpiryAsync(
            $"EpisodeController.FindById.{id}",
            async (e) => EpisodeVM.Create(await _listeningRepository.GetEpisodeByIdAsync(id), true)
        );
        if(episode == null) return NotFound("音频不存在");
        return Ok(episode);
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<EpisodeVM?>>> FindAllByAlbumId([RequiredGuid] [FromQuery] Guid albumId)
    {
        var episodes = await _memoryCache.GetOrCreateWithRandomExpiryAsync(
            $"EpisodeController.FindByAlbumId.{albumId}",
            async (e) => EpisodeVM.Create(await _listeningRepository.GetEpisodesByAlbumIdAsync(albumId), false)
        );
        if (episodes == null) return NotFound("音频不存在");
        return Ok(episodes);
    }
}
