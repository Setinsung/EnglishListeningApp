using Commons.Domain.Models;
using Listening.Domain.Events;
using Listening.Domain.Helpers;
using Listening.Domain.ValueObjects;

namespace Listening.Domain.Entities;

public record Episode : AggregateRootEntity
{
    private Episode() { }

    /// <summary>
    /// 显示序号，靠前优先
    /// </summary>
    public int SequenceNumber { get; private set; }

    /// <summary>
    /// 标题
    /// </summary>
    public MultilingualString Name { get; private set; }

    /// <summary>
    /// //专辑Id
    /// </summary>
    public Guid AlbumId { get; private set; }

    /// <summary>
    /// 音频路径
    /// </summary>
    public Uri AudioUrl { get; private set; }

    /// <summary>
    /// 音频时长（秒数）
    /// 是实际长度，用于直接返回给浏览器
    /// </summary>
    public double DurationInSecond { get; private set; }

    /// <summary>
    /// 原文字幕内容
    /// </summary>
    public string Subtitle { get; private set; }

    /// <summary>
    /// 原文字幕格式
    /// </summary>
    public string SubtitleType { get; private set; }

    /// <summary>
    /// 用户是否可见
    /// </summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// 更改序号并返回当前 Episode 实例。
    /// </summary>
    /// <param name="value">要更改的序号。</param>
    /// <returns>当前 Episode 实例。</returns>
    public Episode ChangeSequenceNumber(int value)
    {
        this.SequenceNumber = value;
        this.AddDomainEventIfAbsent(new EpisodeUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 更改名称并返回当前 Episode 实例。
    /// </summary>
    /// <param name="value">要更改的名称。</param>
    /// <returns>当前 Episode 实例。</returns>
    public Episode ChangeName(MultilingualString value)
    {
        this.Name = value;
        this.AddDomainEventIfAbsent(new EpisodeUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 更改字幕类型和字幕内容并返回当前 Episode 实例。
    /// </summary>
    /// <param name="subtitleType">字幕类型。</param>
    /// <param name="subtitle">字幕内容。</param>
    /// <returns>当前 Episode 实例。</returns>
    public Episode ChangeSubtitle(string subtitleType, string subtitle)
    {
        _ = SubtitleParserFactory.GetParser(subtitleType)
            ?? throw new ArgumentOutOfRangeException(nameof(subtitleType), $"subtitleType={subtitleType} is not supported.");
        this.SubtitleType = subtitleType;
        this.Subtitle = subtitle;
        this.AddDomainEventIfAbsent(new EpisodeUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 隐藏当前 Episode 实例。
    /// </summary>
    /// <returns>当前 Episode 实例。</returns>
    public Episode Hide()
    {
        this.IsVisible = false;
        this.AddDomainEventIfAbsent(new EpisodeUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 显示当前 Episode 实例。
    /// </summary>
    /// <returns>当前 Episode 实例。</returns>
    public Episode Show()
    {
        this.IsVisible = true;
        this.AddDomainEventIfAbsent(new EpisodeUpdatedEvent(this));
        return this;
    }

    /// <summary>
    /// 软删除当前 Episode 实例。
    /// </summary>
    public override void SoftDelete()
    {
        base.SoftDelete();
        this.AddDomainEvent(new EpisodeDeletedEvent(this.Id));
    }

    /// <summary>
    /// 解析字幕并返回句子的集合。
    /// </summary>
    /// <returns>句子的集合。</returns>
    public IEnumerable<Sentence> ParseSubtitle()
    {
        ISubtitleParser? parser = SubtitleParserFactory.GetParser(this.SubtitleType);
        return parser.Parse(this.Subtitle);
    }

    /// <summary>
    /// 用于构建Episode对象
    /// </summary>
    public class Builder
    {
        private Guid id;
        private int sequenceNumber;
        private MultilingualString? name;
        private Guid albumId;
        private Uri? audioUrl;
        private double durationInSecond;
        private string? subtitle;
        private string? subtitleType;

        /// <summary>
        /// 设置构建器的唯一标识符。
        /// </summary>
        /// <param name="value">唯一标识符的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder Id(Guid value)
        {
            this.id = value;
            return this;
        }

        /// <summary>
        /// 设置构建器的序列号。
        /// </summary>
        /// <param name="value">序列号的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder SequenceNumber(int value)
        {
            this.sequenceNumber = value;
            return this;
        }

        /// <summary>
        /// 设置构建器的名称。
        /// </summary>
        /// <param name="value">名称的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder Name(MultilingualString value)
        {
            this.name = value;
            return this;
        }

        /// <summary>
        /// 设置构建器所属的专辑标识符。
        /// </summary>
        /// <param name="value">专辑标识符的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder AlbumId(Guid value)
        {
            this.albumId = value;
            return this;
        }

        /// <summary>
        /// 设置构建器的音频URL。
        /// </summary>
        /// <param name="value">音频URL的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder AudioUrl(Uri value)
        {
            this.audioUrl = value;
            return this;
        }

        /// <summary>
        /// 设置构建器的时长（以秒为单位）。
        /// </summary>
        /// <param name="value">时长的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder DurationInSecond(double value)
        {
            this.durationInSecond = value;
            return this;
        }

        /// <summary>
        /// 设置构建器的字幕。
        /// </summary>
        /// <param name="value">字幕的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder Subtitle(string value)
        {
            this.subtitle = value;
            return this;
        }

        /// <summary>
        /// 设置构建器的字幕类型。
        /// </summary>
        /// <param name="value">字幕类型的值。</param>
        /// <returns>构建器实例。</returns>
        public Builder SubtitleType(string value)
        {
            this.subtitleType = value;
            return this;
        }

        /// <summary>
        /// 构建并返回一个 Episode 实例。
        /// </summary>
        /// <returns>构建的 Episode 实例。</returns>
        public Episode Build()
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (albumId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(albumId));
            }
            if (audioUrl == null)
            {
                throw new ArgumentNullException(nameof(audioUrl));
            }
            if (durationInSecond <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationInSecond));
            }
            if (subtitle == null)
            {
                throw new ArgumentNullException(nameof(subtitle));
            }
            if (subtitleType == null)
            {
                throw new ArgumentNullException(nameof(subtitleType));
            }
            Episode e = new()
            {
                Id = id,
                SequenceNumber = sequenceNumber,
                Name = name,
                AlbumId = albumId,
                AudioUrl = audioUrl,
                DurationInSecond = durationInSecond,
                Subtitle = subtitle,
                SubtitleType = subtitleType,
                IsVisible = true
            };
            e.AddDomainEvent(new EpisodeCreatedEvent(e));
            return e;
        }
    }

}
