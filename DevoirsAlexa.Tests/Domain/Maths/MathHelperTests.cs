using Alexa.NET.Response.Ssml;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Helpers;
using Xunit;

namespace DevoirsAlexa.Tests.Domain;

public class MathHelperTests
{
  [Theory]
  [InlineData(10)]
  [InlineData(100)]
  public void ShouldReturnSumLesserOrEqualToMax_WhenUsingGetRandomSimpleNumbersWithSumUpTo(int sum)
  {
    for (var i = 0; i < 50; i++)
    {
      var numbers = MathHelper.GetRandomSimpleNumbersWithSumUpTo(sum);
      NumbersSumShouldBeLowerThanMax(sum, numbers);

      ThenMyNumberIsSimple(numbers.left, "left");
      ThenMyNumberIsSimple(numbers.right, "right");
    }
  }

  [Fact]
  public void ShouldReturnTrueAndFalse_WhenUsingGetRandomBoolean()
  {
    var loops = 100;
    bool? b = null;
    for (var i = 0; i < loops; i++)
    {
      var newBool = MathHelper.GetRandomBoolean();
      b ??= newBool;
      if (newBool != b)
        break;
      if (i == loops - 1)
        throw new Exception($"Only booleans with value {b} were generated");
    }
  }

  [Theory]
  [InlineData(0, 10)]
  [InlineData(-10, 100)]
  public void ShouldReturnNumberWithinBounds_WhenUsingGetRandomNumberBetween(int min, int max)
  {
    for (var i = 0; i < 50; i++)
    {
      var number = MathHelper.GetRandomNumberBetween(min, max);

      Assert.True(number >= min, $"Number should be above {min} but was {number}.");
      Assert.True(number <= max, $"Number should be under {max} but was {number}.");
    }
  }

  [Theory]
  [InlineData(0, 10)]
  [InlineData(-10, 100)]
  public void ShouldReturnNumbersWithinBounds_WhenUsingGetRandomNumbersBetween(int min, int max)
  {
    for (var i = 0; i < 50; i++)
    {
      var numbers = MathHelper.GetRandomNumbersBothBetween(min, max);

      Assert.True(numbers.left >= min, $"Left number should be above {min} but was {numbers.left}.");
      Assert.True(numbers.left <= max, $"Left number should be under {max} but was {numbers.left}.");
      Assert.True(numbers.right >= min, $"Right number should be above {min} but was {numbers.right}.");
      Assert.True(numbers.right <= max, $"Right number should be under {max} but was {numbers.right}.");
    }
  }

  [Theory]
  [InlineData(0, 10)]
  [InlineData(0, 10000)]
  public void ShouldReturnNumbersWithinBounds_WhenUsingGetRandomNumbersWithSumInBetween(int min, int max)
  {
    for (var i = 0; i < 50; i++)
    {
      var numbers = MathHelper.GetRandomNumbersWithSumInBetween(min, max);

      NumbersSumShouldBeLowerThanMax(max, numbers);
      NumbersSumShouldBeHigherThanMin(min, numbers);
    }
  }

  [Theory]
  [InlineData(0, 10)]
  [InlineData(-100, 100)]
  public void ShouldReturnNumbersWithinBounds_WhenUsingGetRandomSimpleNumbersWithSubBetween(int min, int max)
  {
    for (var i = 0; i < 50; i++)
    {
      var numbers = MathHelper.GetRandomSimpleNumbersWithSubBetween(min, max);

      var calculatedSum = numbers.left - numbers.right;
      Assert.True(calculatedSum <= max, $"Sum should be max {max} but was {calculatedSum}. Numbers were {numbers.left} + {numbers.right}");
      Assert.True(calculatedSum >= min, $"Sum should be min {min} but was {calculatedSum}. Numbers were {numbers.left} + {numbers.right}");

      ThenMyNumberIsSimple(numbers.left, "left");
      ThenMyNumberIsSimple(numbers.right, "right");
    }
  }

  [Fact(Skip = "Need to see what we do with records and string that we dont want empty")]
  public void ShouldThrowError_WhenInstantiatingExerciceRule_GivenNull()
  {
    Assert.Throws<ArgumentNullException>(() => { new ExerciceRule(string.Empty, (string key) => !string.IsNullOrWhiteSpace(key)); });
    Assert.Throws<ArgumentNullException>(() => { new ExerciceRule(" ", (string key) => !string.IsNullOrWhiteSpace(key)); });
  }

  [Fact]
  public void ShouldCallFunc_WhenInstantiatingExerciceRule_GivenSimpleRule()
  {
    var rule = new ExerciceRule("Basic str rule", (string key) => !string.IsNullOrWhiteSpace(key));
    Assert.False(rule.IsValid(""));
    Assert.False(rule.IsValid(" "));
    Assert.True(rule.IsValid("a"));
  }

  private static void NumbersSumShouldBeLowerThanMax(int sum, (int left, int right) numbers)
  {
    var calculatedSum = numbers.left + numbers.right;
    Assert.True(calculatedSum <= sum, $"Sum should be max {sum} but was {calculatedSum}. Numbers were {numbers.left} + {numbers.right}");
  }

  private static void NumbersSumShouldBeHigherThanMin(int min, (int left, int right) numbers)
  {
    var calculatedSum = numbers.left + numbers.right;
    Assert.True(calculatedSum >= min, $"Sum should be min {min} but was {calculatedSum}. Numbers were {numbers.left} + {numbers.right}");
  }

  private static void ThenMyNumberIsSimple(int number, string name)
  {
    Assert.True(number % 5 == 0, $"{name} number should be divisible by 5 but is not. {number}");
  }
}
