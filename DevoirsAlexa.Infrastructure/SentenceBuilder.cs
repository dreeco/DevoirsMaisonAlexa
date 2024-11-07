using Alexa.NET.Response;
using DevoirsAlexa.Domain;
using System.Text;
using System.Text.RegularExpressions;

namespace DevoirsAlexa.Infrastructure;

/// <inheritdoc/>
public class SentenceBuilder : ISentenceBuilder
{
  private StringBuilder CurrentSentence { get; set; }
  private bool IsSSML = false;
  
  /// <summary>
  /// Get a new instance. Text is empty
  /// </summary>
  public SentenceBuilder()
  {
    CurrentSentence = new StringBuilder();
  }

  /// <inheritdoc/>
  public void AppendSimpleText(string text)
  {
    if (text.Contains("<") || text.Contains(">"))
      throw new ArgumentException(nameof(text));

    CurrentSentence.Append(text);
  }

  /// <inheritdoc/>
  public void AppendSpelling(string text)
  {
    SayAs(text, "spell-out");
  }

  private void SayAs(string text, string method)
  {
    IsSSML = true;
    CurrentSentence.Append($"<say-as interpret-as='{method}'>{text}</say-as>");
  }

  /// <inheritdoc/>
  public void AppendInterjection(string text)
  {
    SayAs(text, "interjection");
  }

  /// <inheritdoc/>
  public void AppendPause(TimeSpan? timespan = null)
  {
    IsSSML = true;
    CurrentSentence.Append($"<break time='{timespan?.TotalMilliseconds ?? 300}ms'/>");
  }

  /// <inheritdoc/>
  public void AppendPossiblePlural(string text, int nb)
  {
    AppendSimpleText(nb > 1 ? text + "s" : text);
  }

  /// <summary>
  /// Check if the output text is currently empty
  /// </summary>
  /// <returns></returns>
  public bool IsEmpty()
  {
    return string.IsNullOrEmpty(CurrentSentence.ToString());
  }

  /// <summary>
  /// Transforms sentence as SSML or PlainText according to the case
  /// </summary>
  /// <returns></returns>
  public IOutputSpeech GetSpeech()
  {
    var text = CurrentSentence.ToString().Trim();
    string output = Regex.Replace(text, @"\s+", " ");

    if (IsSSML)
      return new SsmlOutputSpeech() { Ssml = $"<speak>{text}</speak>" };

    return new PlainTextOutputSpeech(text);
  }
}
