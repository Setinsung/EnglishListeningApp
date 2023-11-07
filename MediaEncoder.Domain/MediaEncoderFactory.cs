namespace MediaEncoder.Domain;

public class MediaEncoderFactory
{
    private readonly IEnumerable<IMediaEncoder> _encoders;

    public MediaEncoderFactory(IEnumerable<IMediaEncoder> encoders)
    {
        this._encoders = encoders;
    }

    /// <summary>
    /// 创建符合指定转码目标文件格式的编码器
    /// </summary>
    /// <param name="outputFormat">转码目标文件格式</param>
    /// <returns>符合转码目标文件格式的编码器，如果未找到则返回null</returns>
    public IMediaEncoder? Create(string outputFormat)
    {
        foreach (var encoder in _encoders)
        {
            if (encoder.Accept(outputFormat)) return encoder;
        }
        return null;
    }
}
