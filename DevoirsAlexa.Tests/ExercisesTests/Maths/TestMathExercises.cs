using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.MathExercices;
using DevoirsAlexa.Domain.Models;
using Xunit;

namespace DevoirsAlexa.Tests.Domain;

public class MathExercisesTests
{
  private IExerciceQuestionsRunner? exercice;
  private IExerciceQuestionsRunner Exercice => exercice ?? throw new Exception("Exercice question runner should not be null.");


  [Theory]
  [InlineData(Levels.CP, 1, 4, 45, '+')]
  [InlineData(Levels.CE1, 0, 10, 45, '+')]
  [InlineData(Levels.CE2, 0, 30, 45, '+')]

  [InlineData(Levels.CP, 1, 4, 5, '+')]
  [InlineData(Levels.CE1, 0, 10, 5, '+')]
  [InlineData(Levels.CE2, 0, 30, 5, '+')]


  [InlineData(Levels.CP, 1, 4, 45, '*')]
  [InlineData(Levels.CE1, 0, 10, 45, '*')]
  [InlineData(Levels.CE2, 0, 30, 45, '*')]

  [InlineData(Levels.CP, 1, 4, 5, '*')]
  [InlineData(Levels.CE1, 0, 10, 5, '*')]
  [InlineData(Levels.CE2, 0, 30, 5, '*')]

  [InlineData(Levels.CP, 1, 4, 45, '-')]
  [InlineData(Levels.CE1, 0, 10, 45, '-')]
  [InlineData(Levels.CE2, 0, 30, 45, '-')]

  [InlineData(Levels.CP, 1, 4, 5, '-')]
  [InlineData(Levels.CE1, 0, 10, 5, '-')]
  [InlineData(Levels.CE2, 0, 30, 5, '-')]

  [InlineData(Levels.CP, 1, 4, 45, '/')]
  [InlineData(Levels.CE1, 0, 10, 45, '/')]
  [InlineData(Levels.CE2, 0, 30, 45, '/')]

  [InlineData(Levels.CP, 1, 4, 5, '/')]
  [InlineData(Levels.CE1, 0, 10, 5, '/')]
  [InlineData(Levels.CE2, 0, 30, 5, '/')]

  public void ShouldAskForDifferentQuestionAfterLevel_WhenGettingNewQuestions(Levels level, int min, int max, int loopSize, char operation)
  {
    switch (operation) {
      case '+':
        exercice = new AdditionsExercises();
        break;
      case '-':
        exercice = new SubstractionsExercises();
        break;
      case '*':
        exercice = new MultiplicationsExercises();
        break;
      case '/':
        exercice = new DivisionsExercises();
        break;
      default : throw new ArgumentException(nameof(operation));
    }
    var alreadyAsked = new List<string>();
    for (var n = 0; n < loopSize; n++)
    {
      Question question = WhenIGetTheNextQuestion(level, alreadyAsked);

      ThenIHaveAQuestion(question);
      ThenTheQuestionKeyIsProperlyFormatted(question, operation);
      ThenTheQuestionIsProperlyFormattedWithSameInfoAsKey(question, operation.ToString());
      ThenTheMinMaxForLevelIsRespected(min, max, question, operation);
      alreadyAsked.Add(question.Key);
      ThenTheAnswerValidationIsCorrect(question, operation);
    }

    ThenIHaveAtLeast75PercentDifferentQuestions(min, max, alreadyAsked, loopSize);
  }

  private Question WhenIGetTheNextQuestion(Levels level, List<string> alreadyAsked)
  {
    return Exercice.NextQuestion(level, alreadyAsked);
  }

  private static void ThenIHaveAtLeast75PercentDifferentQuestions(int min, int max, List<string> alreadyAsked, int loopSize)
  {
    var minDifferentQuestion = (max - min) * (max - min) * 0.75;
    if (minDifferentQuestion > loopSize)
      minDifferentQuestion = loopSize;
    var foundDifferentQuestions = alreadyAsked.Distinct().Count();
    Assert.True(minDifferentQuestion <= foundDifferentQuestions, $"Expected a minimum of {minDifferentQuestion} different questions but was {foundDifferentQuestions}");
  }

  private static void ThenTheMinMaxForLevelIsRespected(int min, int max, Question question, char operation)
  {
    var parts = question.Key.Split(operation);
    foreach (var part in parts)
      Assert.True(int.TryParse(part, out var number) && number >= min && number <= max);
  }

  private void ThenTheAnswerValidationIsCorrect(Question question, char operation)
  {
    var parts = question.Key.Split(operation);
    var answer = 0;
    var first = true;
    foreach (var part in parts.Select(p => int.Parse(p)))
    {
      if (first == true)
      {
        answer = part;
        first = false;
      }
      else
      {
        switch (operation)
        {
          case '+':
            answer += part;
            break;
          case '-':
            answer -= part;
            break;
          case '*':
            answer *= part;
            break;
          case '/':
            answer /= part;
            break;
        }
      }
    }


    var validAnswer = Exercice.ValidateAnswer(question.Key, answer.ToString());
    Assert.True(validAnswer.IsValid);

    var invalidAnswer = Exercice.ValidateAnswer(question.Key, (answer + 1).ToString());
    Assert.False(invalidAnswer.IsValid);
    Assert.Contains(answer.ToString(), invalidAnswer.CorrectAnswer);
  }

  private static void ThenTheQuestionIsProperlyFormattedWithSameInfoAsKey(Question question, string operation)
  {
    Assert.Matches($@"Combien font {question.Key.Replace(operation, @"\s[^\d]+\s")} ?", question.Text);
  }

  private static void ThenTheQuestionKeyIsProperlyFormatted(Question question, char operation)
  {
    Assert.Matches($@"\d+\{operation}\d+", question.Key);
  }

  private static void ThenIHaveAQuestion(Question question)
  {
    Assert.NotNull(question);
  }
}
