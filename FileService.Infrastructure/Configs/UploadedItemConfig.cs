using FileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.Infrastructure.Configs;

public class UploadedItemConfig : IEntityTypeConfiguration<UploadedItem>
{
    public void Configure(EntityTypeBuilder<UploadedItem> builder)
    {
        builder.ToTable("T_UploadedItems");
        builder.HasKey(e => e.Id).IsClustered(false); // 取消Guid聚集索引
        builder.Property(e => e.FileName).IsUnicode(true).HasMaxLength(1024);
        builder.Property(e => e.FileSHA256Hash).IsUnicode(false).HasMaxLength(64);
        builder.HasIndex(e => new { e.FileSHA256Hash, e.FileSizeBytes }); // 配置复合索引
    }
}
