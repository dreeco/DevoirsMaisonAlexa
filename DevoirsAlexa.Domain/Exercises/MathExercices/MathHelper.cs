using System.Security.Cryptography;

namespace DevoirsAlexa.Domain.Exercises.MathExercices;

public class MathHelper
{
  protected static char[] Digits = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

  public static (int left, int right) GetRandomSimpleNumbersWithSubBetween(int lower, int higher)
  {
    if (lower < 0 && GetRandomNumberBetween(0, 4) == 1)
    {
      var result = GetRandomNumberBetween(lower / 5, 0) * 5;
      var right = GetRandomNumberBetween(0, higher / 5) * 5;
      return (right - result, right);
    }
    else
    {
      var numbers = GetRandomSimpleNumbersWithSumUpTo(higher);
      return numbers.left > numbers.right ? numbers : (numbers.right, numbers.left);
    }
  }

  public static (int left, int right) GetRandomSimpleNumbersWithSumUpTo(int maxSum)
  {
    var left = GetRandomNumberBetween(1, maxSum / 5) * 5;
    var right = maxSum - left;
    return (left, right);
  }

  public static (int left, int right) GetRandomNumbersWithSumInBetween(int minSum, int maxSum)
  {
    var sum = GetRandomNumberBetween(minSum, maxSum);
    var left = GetRandomNumberBetween(0, sum);
    var right = sum - left;
    return (left, right);
  }

  public static int GetRandomNumberBetween(int min, int max)
  {
    return Random.Shared.Next(min, max+1);
  }

  public static bool GetRandomBoolean()
  {
    return Random.Shared.NextDouble() > 0.5;
  }

  public static (int left, int right) GetRandomNumbersBothBetween(int min, int max)
  {
    return (GetRandomNumberBetween(min, max), GetRandomNumberBetween(min, max));
  }

  public static IEnumerable<int> GetNumbersInQuestion(string questionKey, char? operation = null)
  {
    var operationChar = operation ?? GetOperationChar(questionKey);
    return questionKey.Split(operationChar).Where(p => int.TryParse(p, out var d)).Select(int.Parse);
  }

  public static char GetOperationChar(string questionKey)
  {
    return questionKey.Skip(1).First(c => !Digits.Contains(c));
  }

  public static ExerciceRule GetRuleForMinSubOf(int sum)
  {
    return new ExerciceRule($"Sub higher than {sum}", (string key) =>
    {
      var numbers = GetNumbersInQuestion(key, '-');
      var result = numbers.First() - numbers.Skip(1).Sum();
      return result > sum;
    });
  }
  public static ExerciceRule GetRuleForMaxSumOf(int sum)
  {
    return new ExerciceRule($"Sum lower than {sum}", (string key) => GetNumbersInQuestion(key, '+').Sum() <= sum);
  }
  public static ExerciceRule GetRuleForNoComplicatedNumberAbove(int max)
  {
    return new ExerciceRule($"No complicated number above {max}", (string key) => GetNumbersInQuestion(key).All(n => n % 5 == 0 || n <= max));
  }

  public static ExerciceRule GetRuleForNoNumberUnder(int min)
  {
    return new ExerciceRule($"No number under {min}", (string key) => GetNumbersInQuestion(key).All(n => n >= min));
  }

  public static ExerciceRule GetRuleForNoNumberOver(int max)
  {
    return new ExerciceRule($"No number over {max}", (string key) => GetNumbersInQuestion(key).All(n => n <= max));
  }

  public static ExerciceRule GetRuleForDifferentNumbers()
  {
    return new ExerciceRule($"Numbers should be different", (string key) => GetNumbersInQuestion(key).Distinct().Count() == GetNumbersInQuestion(key).Count());
  }

  public static ExerciceRule GetRuleForNoSimpleNumberAbove(int max)
  {
    return new ExerciceRule($"No simple number above {max}", (string key) => GetNumbersInQuestion(key).All(n => n % 5 != 0 || n <= max));
  }
}
