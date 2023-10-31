using Commons.EventBus;
using Listening.Domain;
using Listening.Domain.Entities;
using Listening.Infrastructure;
using Listening.WebAPI.Admin.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Listening.WebAPI.Admin.EventHandlers;
[EventName("MediaEncoding.Started")]
[EventName("MediaEncoding.Failed")]
[EventName("MediaEncoding.Duplicated")]
[EventName("MediaEncoding.Completed")]
public class MediaEncodingStatusChangeIntegrationHandler : DynamicIntegrationEventHandler
{
    private readonly IListeningRepository _listeningRepository;
    private readonly EncodingEpisodeHelper _encodingEpisodeHelper;
    private readonly IHubContext<EpisodeEncodingStatusHub> _episodeEncodingStatusHubContext;
    private readonly ListeningDbContext _listeningDbContext;

    public MediaEncodingStatusChangeIntegrationHandler(IListeningRepository listeningRepository, EncodingEpisodeHelper encodingEpisodeHelper, IHubContext<EpisodeEncodingStatusHub> episodeEncodingStatusHubContext, ListeningDbContext listeningDbContext)
    {
        this._listeningRepository = listeningRepository;
        this._encodingEpisodeHelper = encodingEpisodeHelper;
        this._episodeEncodingStatusHubContext = episodeEncodingStatusHubContext;
        this._listeningDbContext = listeningDbContext;
    }
    public override async Task HandleDynamic(string eventName, dynamic eventData)
    {
        string sourceStream = eventData.SourceStream;
        if (sourceStream != "Listening") return;
        Guid id = Guid.Parse(eventData.Id);
        switch (eventName)
        {
            case "MediaEncoding.Started":
                await _encodingEpisodeHelper.UpdateEpisodeStatusAsync(id, "Started");
                await _episodeEncodingStatusHubContext.Clients.All.SendAsync("OnMediaEncodingStarted", id);
                break;

            case "MediaEncoding.Failed":
                await _encodingEpisodeHelper.UpdateEpisodeStatusAsync(id, "Failed");
                await _episodeEncodingStatusHubContext.Clients.All.SendAsync("OnMediaEncodingFailed", id);
                break;

            case "MediaEncoding.Duplicated":
                await _encodingEpisodeHelper.UpdateEpisodeStatusAsync(id, "Completed");
                await _episodeEncodingStatusHubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", id);
                break;

            case "MediaEncoding.Completed":
                await _encodingEpisodeHelper.UpdateEpisodeStatusAsync(id, "Completed");
                Uri outputUrl = new(eventData.OutputUrl);
                EncodingEpisodeInfo? encodingEpisodeInfo = await _encodingEpisodeHelper.GetEncodingEpisodeAsync(id);
                Debug.Assert(encodingEpisodeInfo != null);
                if (encodingEpisodeInfo == null) return;

                Guid albumId = encodingEpisodeInfo.AlbumId;
                int maxSeq = await _listeningRepository.GetMaxSeqOfEpisodesAsync(albumId);
                var builder = new Episode.Builder();
                builder.Id(id).SequenceNumber(maxSeq + 1).Name(encodingEpisodeInfo.Name)
                    .AlbumId(albumId).AudioUrl(outputUrl)
                    .DurationInSecond(encodingEpisodeInfo.DurationInSecond)
                    .SubtitleType(encodingEpisodeInfo.SubtitleType)
                    .Subtitle(encodingEpisodeInfo.Subtitle);
                Episode episdoe = builder.Build();
                _listeningDbContext.Add(episdoe);
                await _listeningDbContext.SaveChangesAsync();
                await _episodeEncodingStatusHubContext.Clients.All.SendAsync("OnMediaEncodingCompleted", id);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(eventName), "Invalid eventName");
        }

    }
}
