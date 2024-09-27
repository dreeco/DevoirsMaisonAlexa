using DevoirsAlexa.Application;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Infrastructure;
using Xunit;

namespace DevoirsAlexa.Tests.ExercisesTests.SentenceBuilding;

public class ExercisesSentenceBuilderTests
{

  [Theory]
  [InlineData(1, "en moins de 1 seconde.")]
  [InlineData(10, "en moins de 10 secondes.")]
  [InlineData(60, "en moins de 1 minute.")]
  [InlineData(61, "en moins de 1 minute et 1 seconde.")]
  [InlineData(90, "en moins de 1 minute et 30 secondes.")]
  [InlineData(120, "en moins de 2 minutes.")]
  [InlineData(121, "en moins de 2 minutes et 1 seconde.")]
  [InlineData(122, "en moins de 2 minutes et 2 secondes.")]
  public void ShouldIncludeTime_WhenAskingForExerciceCompletionSentence(int seconds, string expectedText)
  {

    var prompt = new SentenceBuilder();
    RequestHandler.GetEndOfExerciceCompletionSentence(prompt, new ExerciceResult
    {
      CorrectAnswers = 2,
      ElapsedTime = TimeSpan.FromSeconds(seconds),
      TotalQuestions = 2
    });

    Assert.Contains(expectedText, prompt.GetPromptAsText());
  }

  [Theory]
  [InlineData(1, 1, "1 bonne réponse sur 1 question,")]
  [InlineData(1, 2, "1 bonne réponse sur 2 questions,")]
  [InlineData(2, 2, "2 bonnes réponses sur 2 questions,")]
  public void ShouldIncludeGoodAnswers_WhenAskingForExerciceCompletionSentence(int correctAnswers, int totalAnswers, string expectedText)
  {

    var prompt = new SentenceBuilder();
    RequestHandler.GetEndOfExerciceCompletionSentence(prompt, new ExerciceResult
    {
      CorrectAnswers = correctAnswers,
      ElapsedTime = TimeSpan.FromSeconds(30),
      TotalQuestions = totalAnswers
    });

    Assert.Contains(expectedText, prompt.GetPromptAsText());
  }
}
