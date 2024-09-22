﻿using Homework.Enums;
using Homework.HomeworkExercises;
using Homework.HomeworkExercisesRunner;
using Homework.Models;
using Xunit;

namespace DevoirsAlexa.Tests.HomeworkTests
{
    public class TestExercisesRunnerDispatcher
  {
    public HomeworkSession? _currentSession { get; private set; }

    [Theory]
    [InlineData(HomeworkExercisesTypes.Unknown, null)]
    [InlineData(HomeworkExercisesTypes.Additions, typeof(AdditionsExercises))]
    public void ShouldReturnType_GivenSpecificExercice(HomeworkExercisesTypes exercice, Type? expectedType)
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
    [InlineData("FirstName=Alix,Age=6,Exercice=Additions,NbExercice=", 5, @"\d+\+\d+", @"Combien font \d+ plus \d+ ?")]
    [InlineData("FirstName=Elio,Age=4,Exercice=Multiplications,NbExercice=", 5, @"\d+\*\d+", @"Combien font \d+ multiplié par \d+ ?")]
    public void ShouldReturnNextQuestionAfterExercice_GivenCompleteSessionData(string session, int nbExercice, string questionKeyPattern, string questionTextPattern) {
      session += nbExercice.ToString();
      _currentSession = new HomeworkSession(session);
      
      var runner = new HomeworkExerciceRunner(_currentSession);

      for (var n = 0; n <= nbExercice; n++)
      {
        ThereWasNQuestionAsked(n);

        var outputText = runner.NextQuestion();
        var lastQuestionKey = runner.LastQuestionKey;

        var isLastRun = n == nbExercice;
        if (!isLastRun)
        {
          Assert.NotNull(lastQuestionKey);
          ThenTheQuestionAskedMatchesTheExpectedPatterns(lastQuestionKey, questionKeyPattern, questionTextPattern, runner, outputText);
          _currentSession["LastAnswer"] = runner.GetCorrectAnswer(lastQuestionKey);
        }
        else
        {
          Assert.Null(lastQuestionKey);

          ThenTheQuestionDoesNotMatchExerciceQuestionPattern(questionTextPattern, outputText);
          ThenTheDataAreCleanedForNextExercice();
        }
      }
    }

    private void ThereWasNQuestionAsked(int n)
    {
      Assert.NotNull(_currentSession);
      Assert.Equal(n, _currentSession.QuestionAsked);
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
