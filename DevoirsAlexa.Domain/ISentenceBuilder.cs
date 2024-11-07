namespace DevoirsAlexa.Domain;

/// <summary>
/// Build sentences for Alexa
/// Used as a StringBuilder with multiple subsequent calls to complete the text
/// Maybe the domain should not be aware of that
/// </summary>
public interface ISentenceBuilder
{
  /// <summary>
  /// Simple text output with no SSML
  /// </summary>
  /// <param name="text"></param>
  public void AppendSimpleText(string text);

  /// <summary>
  /// Spell out the following string
  /// </summary>
  /// <param name="text"></param>
  public void AppendSpelling(string text);

  /// <summary>
  /// Interjection, must check within Alexa SSML
  /// </summary>
  /// <param name="text"></param>
  public void AppendInterjection(string text);

  /// <summary>
  /// Little pause to add more emphasis
  /// </summary>
  /// <param name="timespan"></param>
  public void AppendPause(TimeSpan? timespan = null);

  /// <summary>
  /// Add an s if more than one
  /// </summary>
  /// <param name="text">the text</param>
  /// <param name="nb">If more than one, add an "S"</param>
  public void AppendPossiblePlural(string text, int nb);
}
