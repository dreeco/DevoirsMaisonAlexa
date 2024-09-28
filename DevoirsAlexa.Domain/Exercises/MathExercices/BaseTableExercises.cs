using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

public enum Operations
{
  Addition = '+',
  Multiplication = '*',
  Substraction = '-',
  //Division = '/',
}

public abstract class BaseTableExercises
{
  public Operations Operation { get; }
  public char OperationChar => (char)Operation;
  public string OperationText { get; }

  protected static Random _RandomGenerator = new Random();
  public IDictionary<Levels, ExerciceRule[]> ExercisesRulesByLevel { get; set; }

  protected BaseTableExercises(Operations operation, string operationText)
  {
    Operation = operation;

    if (string.IsNullOrWhiteSpace(operationText))
      throw new ArgumentNullException(nameof(operationText));

    OperationText = operationText;

    ExercisesRulesByLevel = new Dictionary<Levels, ExerciceRule[]>();
  }

  [Obsolete("Should use the one with rules")]
  public Question NextQuestion(int min, int max, IEnumerable<string> alreadyAsked)
  {
    int x;
    int y;
    string key;
    var n = 0;
    do
    {
      x = _RandomGenerator.Next(min, max);
      y = _RandomGenerator.Next(min, max);
      key = $"{x}{OperationChar}{y}";
    }
    while (n++ < 100 && alreadyAsked.Contains(key));

    return new Question(key, $"Combien font {key.Replace(OperationChar.ToString(), $" {OperationText} ")} ?");
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
      isValid = n++ >= 100 || (rules.All(r => r.IsValid(key)) && !alreadyAsked.Contains(key));
    }
    while (!isValid);

    return new Question(key, $"Combien font {key.Replace(OperationChar.ToString(), $" {OperationText} ")} ?");
  }
  protected IEnumerable<int> GetNumbersInQuestion(string questionKey)
  {
    return questionKey.Split(OperationChar).Where(p => int.TryParse(p, out var d)).Select(int.Parse);
  }
  private int? GetCorrectAnswer(string questionKey)
  {
    int? previous = null;
    var numbers = GetNumbersInQuestion(questionKey);

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
        //case Operations.Division:
        //  previous /= current;
        //  break;
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

  protected (int left, int right) GetRandomSimpleNumbersWithSubBetween(int lower, int higher)
  {
    if (lower < 0 && _RandomGenerator.Next(0, 4) == 1)
    {
      var result = _RandomGenerator.Next(lower * 5, 0) / 5;
      var right = _RandomGenerator.Next(0, higher / 5) * 5;
      return (right - result, right);
    }
    else
    {
      var numbers = GetRandomSimpleNumbersWithSumUpTo(higher);
      return numbers.left > numbers.right ? numbers : (numbers.right, numbers.left);
    }
  }

  protected (int left, int right) GetRandomSimpleNumbersWithSumUpTo(int maxSum)
  {
    var left = _RandomGenerator.Next(1, maxSum / 5) * 5;
    var right = maxSum - left;
    return (left, right);
  }

  protected (int left, int right) GetRandomNumbersWithMaxSumOf(int minSum, int maxSum)
  {
    var sum = _RandomGenerator.Next(minSum, maxSum);
    var left = _RandomGenerator.Next(0, sum);
    var right = sum - left;
    return (left, right);
  }

  protected int GetRandomNumberBetween(int min, int max)
  {
    return _RandomGenerator.Next(min, max);
  }

  protected (int left, int right) GetRandomNumbersBothBetween(int min, int max)
  {
    return (GetRandomNumberBetween(min, max), GetRandomNumberBetween(min, max));
  }

  protected ExerciceRule GetRuleForMinSubOf(int sum)
  {
    return new ExerciceRule($"Sub higher than {sum}", (string key) =>
    {
      var numbers = GetNumbersInQuestion(key);
      var result = numbers.First() - numbers.Skip(1).Sum();
      return result > sum;
    });
  }
  protected ExerciceRule GetRuleForMaxSumOf(int sum)
  {
    return new ExerciceRule($"Sum lower than {sum}", (string key) => GetNumbersInQuestion(key).Sum() <= sum);
  }
  protected ExerciceRule GetRuleForNoComplicatedNumberAbove(int max)
  {
    return new ExerciceRule($"No complicated number above {max}", (string key) => GetNumbersInQuestion(key).All(n => n % 5 == 0 || n < max));
  }

  protected ExerciceRule GetRuleForNoSimpleNumberAbove(int max)
  {
    return new ExerciceRule($"No simple number above {max}", (string key) => GetNumbersInQuestion(key).All(n => n % 5 != 0 || n < max));
  }

  protected ExerciceRule GetRuleForRespectModulo(int[] accptedModulos)
  {
    return new ExerciceRule($"Modulo respected [{string.Join(", ", accptedModulos)}]", (string key) =>
    {
      var numbers = GetNumbersInQuestion(key);
      var f = numbers.First();
      foreach (var number in numbers)
        f /= number;
      return accptedModulos.Any(m => f % m == 0);
    });
  }

}
