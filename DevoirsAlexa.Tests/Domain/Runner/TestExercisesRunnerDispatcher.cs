using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercisesRunner;
using DevoirsAlexa.Domain.MathExercices;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Infrastructure.Models;
using Xunit;

namespace DevoirsAlexa.Tests.Domain
{
  public class ExerciceRunnerTests
  {
    public IHomeworkSession? _currentSession { get; private set; }

    [Theory]
    [InlineData(HomeworkExercisesTypes.Unknown, null)]
    [InlineData(HomeworkExercisesTypes.Additions, typeof(AdditionsExercises))]
    [InlineData(HomeworkExercisesTypes.Substractions, typeof(SubstractionsExercises))]
    [InlineData(HomeworkExercisesTypes.Multiplications, typeof(MultiplicationsExercises))]
    [InlineData(HomeworkExercisesTypes.Divisions, typeof(DivisionsExercises))]
    public void ShouldReturnType_GivenSpecificExercice(HomeworkExercisesTypes exercice, Type? expectedType)
    {
      var dispatcher = new ExerciceDispatcher();
      var exerciceInstance = dispatcher.GetExerciceQuestionsRunner(exercice);

      if (expectedType == null)
        Assert.Null(exerciceInstance);
      else
        Assert.Equal(expectedType, exerciceInstance?.GetType());
    }

    [Theory]
    [InlineData("FirstName=Alix,Level=CP,Exercice=Additions,NbExercice=", 5, @"\d+\+\d+", @"Combien font \d+ plus \d+ ?")]
    [InlineData("FirstName=Elio,Level=CE1,Exercice=Multiplications,NbExercice=", 5, @"\d+\*\d+", @"Combien font \d+ multiplié par \d+ ?")]
    [InlineData("FirstName=Jonathan,Level=CE2,Exercice=Soustractions,NbExercice=", 5, @"\d+\-\d+", @"Combien font \d+ moins \d+ ?")]
    public void ShouldReturnNextQuestionAfterExercice_GivenCompleteSessionData(string session, int nbExercice, string questionKeyPattern, string questionTextPattern)
    {
      session += nbExercice.ToString();
      _currentSession = new HomeworkSession(session);

      var runner = new ExerciceRunner(_currentSession);

      for (var questionAskedLoopBegin = 0; questionAskedLoopBegin <= nbExercice; questionAskedLoopBegin++)
      {
        ThereWasNQuestionAskedAndAnswered(questionAskedLoopBegin);
        var result = runner.ValidateAnswerAndGetNext(false);
        var questionAsked = questionAskedLoopBegin + 1;

        var isLastRun = questionAskedLoopBegin == nbExercice;
        if (!isLastRun)
        {
          ThenIHaveANewCorrectAnswer(questionAskedLoopBegin);
          Assert.NotNull(result.Question);
          ThenTheQuestionAskedMatchesTheExpectedPatterns(result.Question.Key, questionKeyPattern, questionTextPattern, runner, result.Question.Text);
          _currentSession.LastAnswer = runner.GetCorrectAnswer(result.Question.Key);
        }
        else
        {
          Assert.Null(result.Question);
          ThenTheDataAreCleanedForNextExercice();
        }
      }
    }

    private void ThenIHaveANewCorrectAnswer(int n)
    {
      Assert.NotNull(_currentSession);
      Assert.Equal(n, _currentSession.CorrectAnswers);
    }

    private void ThereWasNQuestionAskedAndAnswered(int n)
    {
      Assert.NotNull(_currentSession);
      Assert.Equal(n, _currentSession.QuestionAsked);
      Assert.Equal(Math.Max(0, n - 1), _currentSession.CorrectAnswers);
    }


    private static string ThenTheQuestionAskedMatchesTheExpectedPatterns(string questionKey, string questionKeyPattern, string questionTextPattern, ExerciceRunner runner, string outputText)
    {
      Assert.Matches(questionKeyPattern, questionKey);
      Assert.Matches(questionTextPattern, outputText);
      return questionKey;
    }

    private void ThenTheDataAreCleanedForNextExercice()
    {
      Assert.NotNull(_currentSession);
      Assert.Empty(_currentSession.AlreadyAsked);
      Assert.Null(_currentSession.Exercice);
      Assert.Equal(0, _currentSession.CorrectAnswers);
      Assert.Equal(0, _currentSession.QuestionAsked);
    }
  }
}
