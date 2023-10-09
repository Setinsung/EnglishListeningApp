using System.Text.Json;
using System.Text.Json.Serialization;

namespace Commons.Helpers;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string _dateFormatString;
    public DateTimeJsonConverter()
    {
        _dateFormatString = "yyyy-MM-dd HH:mm:ss";  
    }
    public DateTimeJsonConverter(string dateFormatString)
    {
        _dateFormatString = dateFormatString;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? str = reader.GetString();
        if (str == null) return default(DateTime);
        else return DateTime.Parse(str);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // 固定服务器所在时区，前端适应用户的时区则须重新调整
        writer.WriteStringValue(value.ToString(_dateFormatString));
    }
}
