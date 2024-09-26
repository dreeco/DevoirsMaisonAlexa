using DevoirsAlexa.Domain.ToRemove;
using System.Text;
using System.Text.RegularExpressions;

namespace DevoirsAlexa.Application;

public class SentenceBuilder : ISentenceBuilder
{
  private StringBuilder CurrentSentence { get; set; }

  public SentenceBuilder()
  {
    CurrentSentence = new StringBuilder();
  }

  public void AppendSimpleText(string text)
  {
    CurrentSentence.Append(text);
  }

  public void AppendInterjection(string text) {
    CurrentSentence.Append($"<say-as interpret-as=\"interjection\">{text}</say-as>");
  }

  public void AppendPause(TimeSpan? timespan = null)
  {
    CurrentSentence.Append($"<break time='{timespan?.TotalMilliseconds ?? 300}ms'/>");
  }

  public void AppendPossiblePlural(string text, int nb) {
    AppendSimpleText(nb > 1 ? text + "s" : text);
  }

  public override string ToString()
  {
    return $"<speak>{CurrentSentence.ToString()}</speak>";
  }

  public string GetSimpleText() {
    var regex = new Regex(@"<[^>]*>");
    var text = regex.Replace(CurrentSentence.ToString(), string.Empty, int.MaxValue);
    return text;
  }
}
