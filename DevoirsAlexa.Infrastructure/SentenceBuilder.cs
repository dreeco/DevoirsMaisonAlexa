using Alexa.NET.Response;
using DevoirsAlexa.Domain;
using System.Text;
using System.Text.RegularExpressions;

namespace DevoirsAlexa.Infrastructure;

public class SentenceBuilder : ISentenceBuilder
{
  private StringBuilder CurrentSentence { get; set; }
  private bool IsSSML = false;
  public SentenceBuilder()
  {
    CurrentSentence = new StringBuilder();
  }

  public void AppendSimpleText(string text)
  {
    if (text.Contains("<") || text.Contains(">"))
      throw new ArgumentException(nameof(text));

    CurrentSentence.Append(text);
  }

  public void AppendSpelling(string text)
  {
    SayAs(text, "spell-out");
  }

  private void SayAs(string text, string method)
  {
    IsSSML = true;
    CurrentSentence.Append($"<say-as interpret-as='{method}'>{text}</say-as>");
  }

  public void AppendInterjection(string text)
  {
    SayAs(text, "interjection");
  }

  public void AppendPause(TimeSpan? timespan = null)
  {
    IsSSML = true;
    CurrentSentence.Append($"<break time='{timespan?.TotalMilliseconds ?? 300}ms'/>");
  }

  public void AppendPossiblePlural(string text, int nb)
  {
    AppendSimpleText(nb > 1 ? text + "s" : text);
  }

  public bool IsEmpty()
  {
    return string.IsNullOrEmpty(CurrentSentence.ToString());
  }

  public IOutputSpeech GetSpeech()
  {
    var text = CurrentSentence.ToString().Trim();
    string output = Regex.Replace(text, @"\s+", " ");

    if (IsSSML)
      return new SsmlOutputSpeech() { Ssml = $"<speak>{text}</speak>" };

    return new PlainTextOutputSpeech(text);
  }
}
