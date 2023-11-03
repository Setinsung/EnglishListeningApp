using Listening.Domain.ValueObjects;

namespace Listening.WebAPI.Main.Controllers.Episodes.ViewModels;

public record SentenceVM(double StartTime, double EndTime, string Value)
{
    public static SentenceVM? Create(Sentence? sentence)
    {
        if (sentence == null) return null;
        return new SentenceVM(sentence.StartTime.TotalSeconds, sentence.EndTime.TotalSeconds, sentence.Value);
    }

    public static SentenceVM?[] Create(IEnumerable<Sentence> sentences)
    {
        return sentences.Select(Create).ToArray();
    }
}
