using Presentation;
using Xunit;
using Homework.ExercisesRunner;

namespace DevoirsAlexa.Tests.ExercisesTests.SentenceBuilding;

public class ExercisesSentenceBuilderTests
{
  private SentenceBuilder sentenceBuilder = new SentenceBuilder();

  [Theory]
  [InlineData(1, "en moins de 1 seconde.")]
  [InlineData(10, "en moins de 10 secondes.")]
  [InlineData(60, "en moins de 1 minute.")]
  [InlineData(61, "en moins de 1 minute et 1 seconde.")]
  [InlineData(90, "en moins de 1 minute et 30 secondes.")]
  [InlineData(120, "en moins de 2 minutes.")]
  [InlineData(121, "en moins de 2 minutes et 1 seconde.")]
  [InlineData(122, "en moins de 2 minutes et 2 secondes.")]
  public void ShouldIncludeTime_WhenAskingForExerciceCompletionSentence(int seconds, string expectedText) {

    ExerciceSentenceBuilder.GetEndOfExerciceCompletionSentence(sentenceBuilder, 0, 5, TimeSpan.FromSeconds(seconds));

    Assert.Contains(expectedText, sentenceBuilder.ToString());
  }
}
