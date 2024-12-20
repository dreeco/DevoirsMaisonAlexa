﻿using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Exercises.MathExercices;
using DevoirsAlexa.Domain.Models;
using Xunit;

namespace DevoirsAlexa.Tests.Domain.Maths;
public class TestSortExercises
{
  private SortExercises _exercice = new();

  [Theory]
  [InlineData(Levels.CP, 0, 50)]
  [InlineData(Levels.CE1, 0, 100)]
  [InlineData(Levels.CE2, 0, 1000)]
  [InlineData(Levels.CM1, 0, 10000)]
  [InlineData(Levels.CM2, 0, 100000)]
  public void ShouldAskProperQuestionsAndAcceptOrRefuseAnswers_WhenUsingSortExercice(Levels level, int min, int max)
  {
    var alreadyAsked = new List<string>();

    for (var i = 0; i <= 50; i++)
    {
      var question = _exercice.NextQuestion(level, alreadyAsked);
      ThenIDontHaveDuplicateQuestions(alreadyAsked, question);
      ThenTheQuestionTextIsProperlyFormatted(question, !alreadyAsked.Any());
      alreadyAsked.Add(question.Key);
      var operation = GetOperationChar(question);
      var parts = question.Key.Split(operation).Select(int.Parse);

      ThenTheNumbersAreDifferent(parts);
      ThenTheOperationCharForKeyIsGreaterOrLower(operation);
      ThenTheboundariesForLevelShouldBeRespected(min, max, parts);
      ThenTheValidationShouldBeCorrect(question, parts, operation);

      ThenTheHelpProvidedIsCorrect(question);
    }
  }

  [Fact]
  public void ShouldReturnFalse_WhenValidatingAnswer_GivenWrongKey()
  {
    Assert.False(_exercice.ValidateAnswer("2-5", BooleanAnswer.True.ToString()).IsValid);
    Assert.False(_exercice.ValidateAnswer("2-5", BooleanAnswer.False.ToString()).IsValid);
  }

  [Fact]
  public void ShouldStillAskQuestion_WhenGettingNextQuestion_GivenImpossibleRule()
  {
    var rule = new ExerciceRule("AlwaysFalse", (string str) => false);
    var question = _exercice.NextQuestion(() => (1, 2), [rule], []);
    Assert.NotNull(question);
    ThenTheQuestionTextIsProperlyFormatted(question, isFirstQuestion: true);
    var operation = GetOperationChar(question);
    var parts = question.Key.Split(operation).Select(int.Parse);

    ThenTheNumbersAreDifferent(parts);
    ThenTheOperationCharForKeyIsGreaterOrLower(operation);
    ThenTheValidationShouldBeCorrect(question, parts, operation);
    ThenTheHelpProvidedIsCorrect(question);
  }

  [Fact]
  public void ShouldStillAskQuestion_WhenGettingNextQuestion_GivenAlreadyAsked()
  {
    var rule = new ExerciceRule("AlwaysTrue", (string str) => true);
    var question = _exercice.NextQuestion(() => (1, 2), [rule], ["1<2","1>2"]);
    Assert.NotNull(question);
    ThenTheQuestionTextIsProperlyFormatted(question, isFirstQuestion: false);
    var operation = GetOperationChar(question);
    var parts = question.Key.Split(operation).Select(int.Parse);

    ThenTheNumbersAreDifferent(parts);
    ThenTheOperationCharForKeyIsGreaterOrLower(operation);
    ThenTheValidationShouldBeCorrect(question, parts, operation);
    ThenTheHelpProvidedIsCorrect(question);
  }

  private void ThenTheHelpProvidedIsCorrect(Question question)
  {
    var help = _exercice.Help(question.Key);
    var insideText = GetQuestionKeyFormattedAsText(question);
    Assert.Equal($"Tu dois indiquer par \"vrai\" ou \"faux\" si le chiffre " + insideText + ".", help.Text);
    Assert.Equal(insideText + " ?", help.QuestionText);
    Assert.Equal(QuestionType.Boolean, help.QuestionType);
  }

  private static void ThenTheQuestionTextIsProperlyFormatted(Question question, bool isFirstQuestion)
  {
    var help = isFirstQuestion ? "Réponds par vrai ou faux. " : string.Empty;
    Assert.Equal(help + GetQuestionKeyFormattedAsText(question) + " ?", question.Text);
  }

  private static string GetQuestionKeyFormattedAsText(Question question)
  {
    return question.Key.Replace("<", " est plus petit que ").Replace(">", " est plus grand que ");
  }

  private void ThenTheNumbersAreDifferent(IEnumerable<int> parts)
  {
    Assert.True(parts.Distinct().Count() > 1, "There must be at least 2 different numbers in the list");
  }

  private static void ThenIDontHaveDuplicateQuestions(List<string> alreadyAsked, Question question)
  {
    Assert.DoesNotContain(question.Key, alreadyAsked);
  }

  private static char GetOperationChar(Question question)
  {
    return question.Key.Skip(1).First(c => new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Contains(c) == false);
  }

  private static void ThenTheOperationCharForKeyIsGreaterOrLower(char operation)
  {
    Assert.Contains(operation, new[] { '<', '>' });
  }

  private static void ThenTheboundariesForLevelShouldBeRespected(int min, int max, IEnumerable<int> parts)
  {
    ThenTheNumberShouldBeGreaterThan(min, parts.First());
    ThenTheNumberShouldBeLowerThan(max, parts.First());
    ThenTheNumberShouldBeGreaterThan(min, parts.Last());
    ThenTheNumberShouldBeLowerThan(max, parts.Last());
  }

  private void ThenTheValidationShouldBeCorrect(Question question, IEnumerable<int> parts, char operation)
  {
    var answer = parts.First() - parts.Last() > 0 == (operation == '>');
    var validation = _exercice.ValidateAnswer(question.Key, answer ? "true" : "false");
    Assert.True(validation.IsValid, "The good answer should be accepted");
    Assert.Equal(answer ? "vrai" : "faux", validation.CorrectAnswer);


    var oppositeValidation = _exercice.ValidateAnswer(question.Key, answer ? "false" : "true");
    ThenAWrongAnswerShouldBeRefused(oppositeValidation);
    Assert.Equal(answer ? "vrai" : "faux", oppositeValidation.CorrectAnswer);

    ThenAWrongAnswerShouldBeRefused(_exercice.ValidateAnswer(question.Key, string.Empty));
    ThenAWrongAnswerShouldBeRefused(_exercice.ValidateAnswer(question.Key, "sdjkfsdkfh"));
  }

  private static void ThenAWrongAnswerShouldBeRefused(AnswerValidation oppositeValidation)
  {
    Assert.False(oppositeValidation.IsValid, "The wrong answer should be refused");
  }

  private static void ThenTheNumberShouldBeGreaterThan(int min, int nb)
  {
    Assert.True(nb >= min, $"The number should be over {min}");
  }
  private static void ThenTheNumberShouldBeLowerThan(int max, int nb)
  {
    Assert.True(nb <= max, $"The number should be under {max}");
  }
}
