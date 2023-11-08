using FFmpeg.NET;
using MediaEncoder.Domain;

namespace MediaEncoder.Infrastructure;

public class M4AEncoder : IMediaEncoder
{
    public bool Accept(string outputFormat)
    {
        return "m4a".Equals(outputFormat, StringComparison.OrdinalIgnoreCase);
    }

    public async Task EncodeAsync(FileInfo sourceFile, FileInfo destFile, string destFormat, string[]? args, CancellationToken cancellationToken)
    {
        InputFile inputFile = new(sourceFile);
        OutputFile outputFile = new(destFile);
        string baseDir = AppContext.BaseDirectory;
        string ffmpegPath = Path.Combine(baseDir, "ffmpeg.exe");
        Engine ffmpeg = new(ffmpegPath);
        string? errorMsg = null;
        ffmpeg.Error += (o, e) =>
        {
            errorMsg = e.Exception.Message;
        };
        await ffmpeg.ConvertAsync(inputFile, outputFile,cancellationToken).ConfigureAwait(false);
        if(errorMsg != null) throw new Exception(errorMsg);
    }
}
