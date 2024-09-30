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
    var numbers = MathHelper.GetNumbersInQuestion(questionKey, OperationChar);
    if (numbers.Count() < 2)
      return null;

    int previous = 0;
    var first = true;
    foreach (var current in numbers)
    {
      if (first)
      {
        first = false;
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
