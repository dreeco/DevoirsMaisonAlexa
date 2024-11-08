namespace DevoirsAlexa.Application.Text;

/// <summary>
/// Provide some static functions designed to help format text
/// </summary>
public static class TextHelpers
{
  /// <summary>
  /// Transforms a timespan to a french text (minutes and seconds)
  /// </summary>
  /// <param name="timeSpan"></param>
  /// <returns></returns>
  public static string GetTimeAsText(this TimeSpan timeSpan)
  {
    int minutes = timeSpan.Minutes;
    int seconds = timeSpan.Seconds;

    string minuteText = minutes > 0 ? $"{minutes} minute{(minutes > 1 ? "s" : "")}" : "";
    string secondText = seconds > 0 ? $"{seconds} seconde{(seconds > 1 ? "s" : "")}" : "";
    string inBetween = minutes > 0 && seconds > 0 ? " et " : " ";
    return $"{minuteText}{inBetween}{secondText}".Trim();
  }
}
