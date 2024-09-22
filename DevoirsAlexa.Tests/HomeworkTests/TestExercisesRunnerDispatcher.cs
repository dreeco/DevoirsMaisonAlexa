using Homework.Enums;
using Homework.HomeworkExercisesRunner;
using Homework.Models;
using Xunit;

namespace DevoirsAlexa.Tests.HomeworkTests
{
  public class TestExercisesRunnerDispatcher
  {
    [Theory]
    [InlineData(HomeworkExercises.Unknown, null)]
    [InlineData(HomeworkExercises.Additions, typeof(AdditionsExercises))]
    public void ShouldReturnType_GivenSpecificExercice(HomeworkExercises exercice, Type? expectedType)
    {
      var dispatcher = new HomeworkExerciceDispatcher();
      var exerciceInstance = dispatcher.GetExerciceQuestionsRunner(exercice);
      
      if (expectedType == null)
        Assert.Null(exerciceInstance);
      else
        Assert.Equal(expectedType, exerciceInstance?.GetType());
    }


    [Theory]
    //[InlineData(HomeworkExercises.Unknown, null)]
    [InlineData("FirstName=Alix,Age=6,Exercice=Additions,NbExercice=5", @"\d+\+\d+", @"Combien font \d+ plus \d+ ?")]
    public void ShouldReturnNextQuestionAfterExercice_GivenCompleteSessionData(string session, string questionKeyPattern, string questionTextPattern) {
      var runner = new HomeworkExerciceRunner(new HomeworkSession(session));

      string previousAnswer = string.Empty;
      var nbExercice = runner.NbExercice;

      for (var n = 0; n <= nbExercice; n++)
      {
        ThereWasNQuestionAsked(runner, n);

        var outputText = runner.NextQuestion(previousAnswer);
        var lastQuestionKey = runner.AlreadyAsked.LastOrDefault();

        var isLastRun = n == nbExercice;
        if (!isLastRun)
        {
          Assert.NotNull(lastQuestionKey);
          ThenTheQuestionAskedMatchesTheExpectedPatterns(lastQuestionKey, questionKeyPattern, questionTextPattern, runner, outputText);
          previousAnswer = runner.GetCorrectAnswer(lastQuestionKey);
        }
        else
        {
          Assert.Null(lastQuestionKey);

          ThenTheQuestionDoesNotMatchExerciceQuestionPattern(questionTextPattern, outputText);
          ThenTheDataAreCleanedForNextExercice(runner);
        }
      }
    }

    private static void ThereWasNQuestionAsked(HomeworkExerciceRunner runner, int n)
    {
      Assert.Equal(n, runner.QuestionAsked);
    }

    private static void ThenTheQuestionDoesNotMatchExerciceQuestionPattern(string questionTextPattern, string outputText)
    {
      Assert.DoesNotMatch(questionTextPattern, outputText);
    }

    private static string ThenTheQuestionAskedMatchesTheExpectedPatterns(string questionKey, string questionKeyPattern, string questionTextPattern, HomeworkExerciceRunner runner, string outputText)
    {
      Assert.Matches(questionKeyPattern, questionKey);
      Assert.Matches(questionTextPattern, outputText);
      return questionKey;
    }

    private static void ThenTheDataAreCleanedForNextExercice(HomeworkExerciceRunner runner)
    {
      Assert.Empty(runner.AlreadyAsked);
      Assert.Null(runner.Exercice);
      Assert.Equal(0, runner.CorrectAnswers);
      Assert.Equal(0, runner.QuestionAsked);
    }
  }
}
