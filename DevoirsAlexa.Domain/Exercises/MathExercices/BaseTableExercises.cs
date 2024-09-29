using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Exercises.MathExercices;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

public enum Operations
{
  Addition = '+',
  Multiplication = '*',
  Substraction = '-',
}

public abstract class BaseTableExercises
{
  public Operations Operation { get; }
  public char OperationChar => (char)Operation;
  public string OperationText { get; }

  public IDictionary<Levels, ExerciceRule[]> ExercisesRulesByLevel { get; set; }

  protected BaseTableExercises(Operations operation, string operationText)
  {
    Operation = operation;

    if (string.IsNullOrWhiteSpace(operationText))
      throw new ArgumentNullException(nameof(operationText));

    OperationText = operationText;

    ExercisesRulesByLevel = new Dictionary<Levels, ExerciceRule[]>();
  }

  protected Question NextQuestion(Func<(int left, int right)> getNewNumbers, IEnumerable<ExerciceRule> rules, IEnumerable<string> alreadyAsked)
  {
    string key;
    var n = 0;
    var isValid = false;
    do
    {
      var numbers = getNewNumbers();
      key = $"{numbers.left}{OperationChar}{numbers.right}";
      isValid = n++ >= 1000 || (rules.All(r => r.IsValid(key)) && !alreadyAsked.Contains(key));
    }
    while (!isValid);

    return new Question(key, $"Combien font {key.Replace(OperationChar.ToString(), $" {OperationText} ")} ?");
  }

  private int? GetCorrectAnswer(string questionKey)
  {
    int? previous = null;
    var numbers = MathHelper.GetNumbersInQuestion(questionKey, OperationChar);

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
        case Operations.Substraction:
          previous -= current;
          break;
        default: throw new ArgumentException($"Unsupported operation {Operation}", nameof(Operation));
      }
    }
    return previous;
  }

  public AnswerValidation ValidateAnswer(string questionKey, string answer)
  {
    var resultNumber = GetCorrectAnswer(questionKey);

    var isValid = int.TryParse(answer, out var answerNumber) && answerNumber == resultNumber;
    var correctAnswer = resultNumber?.ToString() ?? string.Empty;
    return new AnswerValidation(isValid, correctAnswer);
  }

}
