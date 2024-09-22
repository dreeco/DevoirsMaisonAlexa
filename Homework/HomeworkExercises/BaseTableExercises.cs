using Homework.Enums;
using Homework.HomeworkExercisesRunner;
using Homework.Models;

namespace Homework.HomeworkExercises;

public abstract class BaseTableExercises
{
  public Question NextQuestion(int min, int max, char operation, string operationText, IEnumerable<string> alreadyAsked)
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
      key = $"{x}{operation}{y}";
    }
    while (n++ < 100 && alreadyAsked.Contains(key));

    return new Question(key, $"Combien font {key.Replace(operation.ToString(), $" {operationText} ")} ?");
  }

  public int? GetCorrectAnswer(string questionKey, char operation)
  {
    var parts = questionKey.Split(operation);
    int? previous = null;

    foreach (var current in parts.Select(p => int.Parse(p)))
    {
      if (previous == null)
      {
        previous = current;
        continue;
      }

      switch (operation)
      {
        case '+':
          previous += current;
          break;
        case '*':
          previous *= current;
          break;
        case '/':
          previous /= current;
          break;
        case '-':
          previous -= current;
          break;
      }
    }
    return previous;
  }

  public AnswerValidation ValidateAnswer(string questionKey, string answer, char operation)
  {
    var resultNumber = GetCorrectAnswer(questionKey, operation);
    if (resultNumber == null)
      return new AnswerValidation(false, "Impossible de calculer la bonne réponse");

    if (!int.TryParse(answer, out var answerNumber))
#pragma warning disable CS8604 // The if null prevents null.
      return new AnswerValidation(false, resultNumber.ToString());
#pragma warning restore CS8604

    return new AnswerValidation(resultNumber == answerNumber, string.Empty);
  }

}
