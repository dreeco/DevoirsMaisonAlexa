using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Domain.Models;

public class Question
{
  public string Key { get; set; }

  public Question(string key, string text, QuestionType type)
  {
    if (string.IsNullOrEmpty(key))
      throw new ArgumentNullException(nameof(key));
    if (string.IsNullOrEmpty(text))
      throw new ArgumentNullException(nameof(text));

    Key = key;
    Text = text;
    Type = type;
  }

  public string Text { get; set; }

  public int Index { get; set; }

  public QuestionType Type { get; set; }
}