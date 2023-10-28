using Commons.ASPNETCore;
using Commons.EventBus;
using Commons.Helpers.Validators;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Listening.WebAPI.Admin.Controllers.Episodes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Listening.WebAPI.Admin.Controllers.Episodes;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
[UnitOfWorkFilter(typeof(ListeningDbContext))]
public class EpisodesController : ControllerBase
{
    private readonly ListeningDomainService _listeningDomainService;
    private readonly IListeningRepository _listeningRepository;
    private readonly IEventBus _eventBus;
    private readonly EncodingEpisodeHelper _encodingEpisodeHelper;

    public EpisodesController(ListeningDomainService listeningDomainService, IListeningRepository listeningRepository, IEventBus eventBus, EncodingEpisodeHelper encodingEpisodeHelper)
    {
        this._listeningDomainService = listeningDomainService;
        this._listeningRepository = listeningRepository;
        this._eventBus = eventBus;
        this._encodingEpisodeHelper = encodingEpisodeHelper;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Add(EpisodeAddRequest req)
    {
        if (req.AudioUrl.ToString().EndsWith("m4a", StringComparison.OrdinalIgnoreCase))
        {   // m4a文件直接存到数据库
            Episode? episode = await _listeningDomainService.AddEpisodeAsync(req.Name, req.AlbumId, req.AudioUrl, req.DurationInSecond, req.SubtitleType, req.Subtitle);
            if (episode == null) return NotFound("音频所在专辑未找到");
            return episode.Id;
        }
        else
        {
            // 非m4a文件先转码，避免非法数据污染业务数据增加业务逻辑麻烦。按照DDD原则不完整的Episode不能插入数据库，先临时插入Redis，转码完成再插入数据库
            Guid episodeId = Guid.NewGuid();
            EncodingEpisodeInfo encodingEpisodeInfo = new(episodeId, req.Name, req.AlbumId, req.DurationInSecond, req.Subtitle, req.SubtitleType, "Created");
            await _encodingEpisodeHelper.AddEncodingEpisodeAsync(episodeId, encodingEpisodeInfo);

            // 通知转码
            _eventBus.Publish("MediaEncoding.Created", new { MediaId = episodeId, MediaUrl = req.AudioUrl, OutputFormat = "m4a", SourceSystem = "Listening" });
            return episodeId;
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([RequiredGuid] Guid id, EpisodeUpdateRequest req)
    {
        var episode = await _listeningRepository.GetEpisodeByIdAsync(id);
        if (episode == null) return NotFound("音频未找到");
        episode.ChangeName(req.Name);
        episode.ChangeSubtitle(req.SubtitleType, req.Subtitle);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteById([RequiredGuid] Guid id)
    {
        var album = await _listeningRepository.GetEpisodeByIdAsync(id);
        if (album == null) return NotFound("音频未找到");
        album.SoftDelete();
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Episode>> FindById([RequiredGuid] Guid id)
    {
        var episode = await _listeningRepository.GetEpisodeByIdAsync(id);
        if (episode == null) return NotFound("音频未找到");
        return episode;
    }

    [HttpGet("/list")]
    public async Task<ActionResult<Episode[]>> FindByAlbumId([RequiredGuid] [FromQuery] Guid albumId)
    {
        var episodes = await _listeningRepository.GetEpisodesByAlbumIdAsync(albumId);
        return episodes;
    }

    [HttpGet("/encodings")]
    public async Task<ActionResult<IEnumerable<EncodingEpisodeInfo>>> FindEncodingEpisodesByAlbumId([RequiredGuid][FromQuery] Guid albumId)
    {
        List<EncodingEpisodeInfo> list = new();
        var episodeIds = await _encodingEpisodeHelper.GetEncodingEpisodeIdsAsync(albumId);
        foreach (var episodeId in episodeIds)
        {
            var encodingEpisode = await _encodingEpisodeHelper.GetEncodingEpisodeAsync(episodeId);
            if(encodingEpisode != null && !string.Equals(encodingEpisode.Status, "Completed", StringComparison.OrdinalIgnoreCase)) // 不显示已经完成的
                list.Add(encodingEpisode);
        }
        return list.ToArray();
    }

    [HttpPut("{id}/hide")]
    public async Task<ActionResult> Hide([RequiredGuid] Guid id)
    {
        var episode = await _listeningRepository.GetEpisodeByIdAsync(id);
        if (episode == null) return NotFound($"未查询到音频");
        episode.Hide();
        return NoContent();
    }

    [HttpPut("{id}/show")]
    public async Task<ActionResult> Show([RequiredGuid] Guid id)
    {
        var episode = await _listeningRepository.GetEpisodeByIdAsync(id);
        if (episode == null) return NotFound($"未查询到音频");
        episode.Show();
        return NoContent();
    }

    [HttpPut("/sort/{albumId}")]
    public async Task<ActionResult> Sort([RequiredGuid] Guid albumId, EpisodesSortRequest req)
    {
        await _listeningDomainService.SortEpisodesAsync(albumId, req.SortedEpisodeIds);
        return Ok();
    }

}
