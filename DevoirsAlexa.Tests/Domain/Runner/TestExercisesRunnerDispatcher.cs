using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.HomeworkExercisesRunner;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Infrastructure.Models;
using System.Reflection;
using Xunit;

namespace DevoirsAlexa.Tests.Domain
{
  public class ExerciceRunnerTests
  {
    public IHomeworkSession? _currentSession { get; private set; }


    [Fact]
    public void ShouldHaveAProperExerciceTypeListConfigured()
    {
      var exercises = typeof(IExerciceQuestionsRunner).Assembly
      .GetTypes()
      .Where(type => typeof(IExerciceQuestionsRunner).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
      .Select(t => Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null) as IExerciceQuestionsRunner);
      Assert.True(exercises.Select(e => e?.Type).Distinct().Count() == exercises.Count(), $"There should be exactly one implementation of IExerciceQuestionRunner by ExerciceType");
      Assert.True(exercises.All(e => e != null), $"All classes implementing IExerciceQuestionRunner should be able to create an instance");

      foreach (HomeworkExercisesTypes e in Enum.GetValues(typeof(HomeworkExercisesTypes)))
      {
        if (e == HomeworkExercisesTypes.Unknown)
          continue;

        Assert.NotNull(exercises.SingleOrDefault(t => t?.Type == e));
      }
    }

    [Theory]
    [InlineData("FirstName=Alix,Level=CP,Exercice=Additions,NbExercice=", 5, @"\d+\+\d+", @"Combien font \d+ plus \d+ ?")]
    [InlineData("FirstName=Elio,Level=CE1,Exercice=Multiplications,NbExercice=", 5, @"\d+\*\d+", @"Combien font \d+ multiplié par \d+ ?")]
    [InlineData("FirstName=Jonathan,Level=CE2,Exercice=Soustractions,NbExercice=", 5, @"\d+\-\d+", @"Combien font \d+ moins \d+ ?")]
    [InlineData("FirstName=Jonathan,Level=CE2,Exercice=Soustractions,AlreadyAsked=2-2,NbExercice=", 5, @"\d+\-\d+", @"Combien font \d+ moins \d+ ?")]
    [InlineData("FirstName=Jonathan,Level=CE2,Exercice=SortNumbers,AlreadyAsked=2>2,NbExercice=", 5, @"\d+[<>]\d+", @"\d+ est plus (grand|petit) que \d+ ?")]
    public void ShouldReturnNextQuestionAfterExercice_GivenCompleteSessionData(string session, int nbExercice, string questionKeyPattern, string questionTextPattern)
    {
      session += nbExercice.ToString();
      _currentSession = new HomeworkSession(session);
      Assert.NotNull(_currentSession.Exercice);

      var runner = new ExerciceRunner(Extensions.GetRunner, _currentSession);

      for (var questionAskedLoopBegin = 0; questionAskedLoopBegin <= nbExercice; questionAskedLoopBegin++)
      {
        ThereWasNQuestionAskedAndAnswered(questionAskedLoopBegin);
        var result = runner.ValidateAnswerAndGetNext(false);
        Assert.False(result.CouldNotStart);

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

    [Fact]
    public void ShouldReturnIsValidFalse_WhenCallingGetCorrectAnswer_GivenNoAnswer()
    {
      var session = new HomeworkSession("FirstName=Lucie,Level=CE2,Exercice=Additions,AlreadyAsked=2+2,NbExercice=2");
      var runner = new ExerciceRunner(Extensions.GetRunner, session);
      var answer = runner.ValidateAnswerAndGetNext(false);
      Assert.NotNull(answer);
      Assert.NotNull(answer.Validation);
      Assert.False(answer.Validation.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("FirstName=Alix")]
    [InlineData("FirstName=Alix,Level=CE2")]
    [InlineData("FirstName=Alix,Level=CE2,Exercice=Additions")]
    [InlineData("FirstName=Alix,Level=CE2,NbExercices=5")]
    [InlineData("FirstName=Alix,Level=WTF,Exercice=Additions,NbExercices=5")]
    [InlineData("FirstName=Alix,Level=CE2,Exercice=Unknown,NbExercices=5")]
    public void ShouldReturnCouldNotStart_GivenIncompleteSessionData(string sessionData)
    {
      _currentSession = new HomeworkSession(sessionData);

      var runner = new ExerciceRunner(Extensions.GetRunner, _currentSession);

      Assert.True(runner.ValidateAnswerAndGetNext(false).CouldNotStart);
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
