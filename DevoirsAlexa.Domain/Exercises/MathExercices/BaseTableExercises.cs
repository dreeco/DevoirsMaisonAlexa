using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

public enum Operations
{
  Addition = '+',
  Multiplication = '*',
  Substraction = '-',
  Division = '/',
}

public abstract class BaseTableExercises
{
  private Operations Operation { get; }
  protected char OperationChar => (char)Operation;
  private string OperationText { get; }

  protected BaseTableExercises(Operations operation, string operationText)
  {
    Operation = operation;

    if (string.IsNullOrWhiteSpace(operationText))
      throw new ArgumentNullException(nameof(operationText));

    OperationText = operationText;
  }

  public Question NextQuestion(int min, int max, IEnumerable<string> alreadyAsked)
  {
    var random = new Random();
    int x;
    int y;
    string key;
    var n = 0;
    do
    {
      x = random.Next(min, max);
      y = random.Next(min, max);
      key = $"{x}{OperationChar}{y}";
    }
    while (n++ < 100 && alreadyAsked.Contains(key));

    return new Question(key, $"Combien font {key.Replace(OperationChar.ToString(), $" {OperationText} ")} ?");
  }

  public int? GetCorrectAnswer(string questionKey)
  {
    var parts = questionKey.Split(OperationChar);
    int? previous = null;

    var numbers = parts.Where(p => int.TryParse(p, out var d)).Select(p => int.Parse(p));
    foreach (var current in numbers)
    {
      if (previous == null)
      {
        previous = current;
        continue;
      }

      switch (Operation)
      {
        case Operations.Addition:
          previous += current;
          break;
        case Operations.Multiplication:
          previous *= current;
          break;
        case Operations.Division:
          previous /= current;
          break;
        case Operations.Substraction:
          previous -= current;
          break;
      }
    }
    return previous;
  }

  public AnswerValidation ValidateAnswer(string questionKey, string answer)
  {
    var resultNumber = GetCorrectAnswer(questionKey);

    if (!int.TryParse(answer, out var answerNumber))
      return new AnswerValidation(false, resultNumber?.ToString() ?? string.Empty);

    return new AnswerValidation(resultNumber == answerNumber, resultNumber?.ToString() ?? string.Empty);
  }

}
