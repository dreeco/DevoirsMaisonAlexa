namespace DevoirsAlexa.Domain.Models;

public class HelpResult
{
  public string Text { get; set; }
  public string QuestionText { get; set; }

  public HelpResult(string text, string questionText)
  {
    Text = text;
    QuestionText = questionText;
  }
}
