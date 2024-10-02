namespace DevoirsAlexa.Application.Text;

public static class TextHelpers
{

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
