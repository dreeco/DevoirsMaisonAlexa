using Homework.HomeworkExercises.MathExercices;
using Homework.Models;
using Xunit;

namespace DevoirsAlexa.Tests.HomeworkTests
{
    public class TestMultiplications
  {
    private MultiplicationsExercises exercice;

    public TestMultiplications()
    {
      exercice = new MultiplicationsExercises();
    }

    [Theory]
    [InlineData(1, 1, 4, 45)]
    [InlineData(5, 0, 10, 45)]
    [InlineData(8, 0, 30, 45)]

    [InlineData(1, 1, 4, 5)]
    [InlineData(5, 0, 10, 5)]
    [InlineData(8, 0, 30, 5)]
    public void ShouldAskForDifferentQuestionAfterAge_WhenGettingNewQuestions(int age, int min, int max, int loopSize)
    {
      var alreadyAsked = new List<string>();
      for (var n = 0; n < loopSize; n++)
      {
        Question question = WhenIGetTheNextQuestion(age, alreadyAsked);

        ThenIHaveAQuestion(question);
        ThenTheQuestionKeyIsProperlyFormatted(question);
        ThenTheQuestionIsProperlyFormattedWithSameInfoAsKey(question);
        ThenTheMinMaxForAgeIsRespected(min, max, question);
        alreadyAsked.Add(question.Key);
        ThenTheAnswerValidationIsCorrect(question);
      }

      ThenIHaveAtLeast75PercentDifferentQuestions(min, max, alreadyAsked, loopSize);
    }

    private Question WhenIGetTheNextQuestion(int age, List<string> alreadyAsked)
    {
      return exercice.NextQuestion(age, alreadyAsked);
    }

    private static void ThenIHaveAtLeast75PercentDifferentQuestions(int min, int max, List<string> alreadyAsked, int loopSize)
    {
      var minDifferentQuestion = ((max - min) * (max - min) * 0.75);
      if (minDifferentQuestion > loopSize)
        minDifferentQuestion = loopSize;
      var foundDifferentQuestions = alreadyAsked.Distinct().Count();
      Assert.True(minDifferentQuestion <= foundDifferentQuestions, $"Expected a minimum of {minDifferentQuestion} different questions but was {foundDifferentQuestions}");
    }

    private static void ThenTheMinMaxForAgeIsRespected(int min, int max, Question question)
    {
      var parts = question.Key.Split('*');
      foreach (var part in parts)
        Assert.True(int.TryParse(part, out var number) && number >= min && number <= max);
    }

    private void ThenTheAnswerValidationIsCorrect(Question question)
    {
      var parts = question.Key.Split('*');
      var answer = 1;
      foreach (var part in parts.Select(p => int.Parse(p))) 
        answer *= part;
      
      Assert.True(exercice.ValidateAnswer(question.Key, answer.ToString()).IsValid);
      Assert.False(exercice.ValidateAnswer(question.Key, (answer + 1).ToString()).IsValid);
    }

    private static void ThenTheQuestionIsProperlyFormattedWithSameInfoAsKey(Question question)
    {
      Assert.Matches($@"Combien font {question.Key.Replace("*", @"\smultiplié par\s")} ?", question.Text);
    }

    private static void ThenTheQuestionKeyIsProperlyFormatted(Question question)
    {
      Assert.Matches(@"\d+\*\d+", question.Key);
    }

    private static void ThenIHaveAQuestion(Question question)
    {
      Assert.NotNull(question);
    }
  }
}
