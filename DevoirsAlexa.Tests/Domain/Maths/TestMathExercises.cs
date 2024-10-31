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
  private BaseTableExercises ExerciceAsBase => Exercice as BaseTableExercises ?? throw new Exception("Exercice question runner should be child of BaseTableExercises.");

  [Theory]
  [InlineData(Levels.CP, 10, 50, 45, 20)]
  [InlineData(Levels.CE1, 20, 100, 45, 30)]
  [InlineData(Levels.CE2, 30, 1000, 45)]
  [InlineData(Levels.CM1, 100, 10000, 45)]
  [InlineData(Levels.CM2, 1000, 100000, 45)]

  [InlineData(Levels.CP, 10, 50, 5)]
  [InlineData(Levels.CE1, 20, 100, 5)]
  [InlineData(Levels.CE2, 30, 1000, 5)]
  [InlineData(Levels.CM1, 100, 10000, 5)]
  [InlineData(Levels.CM2, 1000, 100000, 5)]
  public void ShouldAskForAProperAddition_WhenGettingQuestionAfterLevel(Levels level, int numberUpTo, int sumSimpleNumberUpTo, int loopSize, int? minDifferentAtLeast = null)
  {
    exercice = new AdditionsExercises();
    var checkExerciceRules = (Question question) => { ThenAdditionRulesByLevelAreRespected(numberUpTo, sumSimpleNumberUpTo, question); };
    RunLoopTestForExercice(level, checkExerciceRules, loopSize, minDifferentAtLeast);
  }

  [Fact]
  public void ShouldGiveHelp_ForSimpleAddition()
  {
    exercice = new AdditionsExercises();
    var help = exercice.Help("2+2");
    Assert.NotNull(help);
    Assert.Matches(@"La bonne réponse est entre [-\d]+ et [-\d]+.", help.Text);
    Assert.Equal(@"Combien font 2 plus 2 ?", help.QuestionText);
  }

  [Fact]
  public void ShouldNotGiveHelp_ForWrongAddition()
  {
    exercice = new AdditionsExercises();
    var help = exercice.Help("2_2");
    Assert.NotNull(help);
    Assert.Equal("Impossible de calculer la bonne réponse.", help.Text);
  }

  [Theory]
  [InlineData(Levels.CP, 4, 45)]
  [InlineData(Levels.CE1, 5, 45)]
  [InlineData(Levels.CE2, 10, 45)]
  [InlineData(Levels.CM1, 20, 45)]
  [InlineData(Levels.CM2, 50, 45)]
  [InlineData(Levels.CP, 4, 5)]
  [InlineData(Levels.CE1, 5, 5)]
  [InlineData(Levels.CE2, 10, 5)]
  [InlineData(Levels.CM1, 20, 5)]
  [InlineData(Levels.CM2, 50, 5)]

  public void ShouldAskForAProperMultiplication_WhenGettingQuestionAfterLevel(Levels level, int numberUpTo, int loopSize)
  {
    exercice = new MultiplicationsExercises();
    var nbDifferentAnswersPossible = (numberUpTo - 1) * (numberUpTo - 1);
    var checkExerciceRules = (Question question) => { ThenMultiplicationRulesByLevelAreRespected(numberUpTo, question); };
    RunLoopTestForExercice(level, checkExerciceRules, loopSize, nbDifferentAnswersPossible);
  }

  [Fact]
  public void ShouldGiveHelp_ForSimpleMultiplication()
  {
    exercice = new MultiplicationsExercises();
    var help = exercice.Help("2*2");
    Assert.NotNull(help);
    Assert.Matches(@"La bonne réponse est entre [-\d]+ et [-\d]+.", help.Text);
    Assert.Equal(@"Combien font 2 multiplié par 2 ?", help.QuestionText);
  }

  [Fact]
  public void ShouldNotGiveHelp_ForWrongMultiplication()
  {
    exercice = new MultiplicationsExercises();
    var help = exercice.Help("2_2");
    Assert.NotNull(help);
    Assert.Equal("Impossible de calculer la bonne réponse.", help.Text);
  }


  [Theory]
  [InlineData(Levels.CP, 0, 10, 50, 45, 18)]
  [InlineData(Levels.CE1, -10, 20, 100, 45)]
  [InlineData(Levels.CE2, -30, 30, 1000, 45)]
  [InlineData(Levels.CM1, -100, 100, 10000, 45)]
  [InlineData(Levels.CM2, -1000, 1000, 100000, 45)]

  [InlineData(Levels.CP, 0, 10, 50, 5, 2)]
  [InlineData(Levels.CE1, -10, 20, 100, 5)]
  [InlineData(Levels.CE2, -30, 30, 1000, 5)]
  [InlineData(Levels.CM1, -100, 100, 10000, 5)]
  [InlineData(Levels.CM2, -1000, 1000, 100000, 5)]
  public void ShouldAskForAProperSubstraction_WhenGettingQuestionAfterLevel(Levels level, int sumAtLeast, int numberUpTo, int simpleNumbersUpTo, int loopSize, int? minDifferentAtLeast = null)
  {
    exercice = new SubstractionsExercises();
    var checkExerciceRules = (Question question) => { ThenSubstractionRulesByLevelAreRespected(numberUpTo, simpleNumbersUpTo, sumAtLeast, question); };
    RunLoopTestForExercice(level, checkExerciceRules, loopSize, minDifferentAtLeast ?? loopSize);
  }

  [Fact]
  public void ShouldGiveHelp_ForSimpleSubstraction()
  {
    exercice = new SubstractionsExercises();
    var help = exercice.Help("2-2");
    Assert.NotNull(help);
    Assert.Matches(@"La bonne réponse est entre [-\d]+ et [-\d]+.", help.Text);
    Assert.Equal(@"Combien font 2 moins 2 ?", help.QuestionText);
  }

  [Fact]
  public void ShouldNotGiveHelp_ForWrongSubstraction()
  {
    exercice = new SubstractionsExercises();
    var help = exercice.Help("2_2");
    Assert.NotNull(help);
    Assert.Equal("Impossible de calculer la bonne réponse.", help.Text);
  }


  [Theory]
  [InlineData("", "", "")]
  [InlineData("2_2", "4", "")]
  [InlineData("2-2", "4", "")]
  [InlineData("2*2", "2", "")]
  public void ShouldReturnInvalidAnswer_GivenIncorrectData(string questionKey, string answer, string expectedCorrectAnswer)
  {
    var exercice = new AdditionsExercises();
    var result = exercice.ValidateAnswer(questionKey, answer);
    Assert.False(result.IsValid);
    Assert.Equal(expectedCorrectAnswer, result.CorrectAnswer);
  }

  private void ThenIHaveAProperlyBuiltQuestionWithAPossibleAnswer(List<string> alreadyAsked, Question question)
  {
    ThenIHaveAQuestion(question);
    ThenTheQuestionKeyIsProperlyFormatted(question);
    ThenTheQuestionIsProperlyFormattedWithSameInfoAsKey(question);
    alreadyAsked.Add(question.Key);
    ThenTheAnswerValidationIsCorrect(question);
  }

  private Question WhenIGetTheNextQuestion(Levels level, List<string> alreadyAsked)
  {
    return Exercice.NextQuestion(level, alreadyAsked);
  }

  private static void ThenIHaveAtLeast75PercentDifferentQuestions(List<string> alreadyAsked, int expectedDifferentKeys)
  {
    var foundDifferentQuestions = alreadyAsked.Distinct().Count();
    Assert.True(expectedDifferentKeys <= foundDifferentQuestions, $"Expected a minimum of {expectedDifferentKeys} different questions but was {foundDifferentQuestions}. {string.Join(';', alreadyAsked)}");
  }

  private void RunLoopTestForExercice(Levels level, Action<Question> questionRespectExerciceRules, int loopSize, int? nbDifferentAnswersPossible)
  {
    var alreadyAsked = new List<string>();
    for (var n = 0; n < loopSize; n++)
    {
      Question question = WhenIGetTheNextQuestion(level, alreadyAsked);

      ThenIHaveAProperlyBuiltQuestionWithAPossibleAnswer(alreadyAsked, question);
      questionRespectExerciceRules(question);
    }

    var loopSize90Percent = (int)Math.Floor(loopSize * 0.9);
    nbDifferentAnswersPossible ??= loopSize90Percent;
    nbDifferentAnswersPossible = Math.Min(nbDifferentAnswersPossible.Value, loopSize90Percent);
    ThenIHaveAtLeast75PercentDifferentQuestions(alreadyAsked, nbDifferentAnswersPossible.Value);
  }

  private void ThenAdditionRulesByLevelAreRespected(int numberUpTo, int sumSimpleNumberUpTo, Question question)
  {
    var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);
    var sum = parts.Sum();

    foreach (var number in parts)
      Assert.True(
        (number >= 1 && number <= numberUpTo) || 
        (number % 5 == 0 && sum <= sumSimpleNumberUpTo), 
      $"Number {number} does not match addition rules. Should be between 1 and {numberUpTo} or divisible by 5 and < {sumSimpleNumberUpTo}");
  }

  private void ThenSubstractionRulesByLevelAreRespected(int numberUpTo, int simpleNumberUpTo, int sumAtLeast, Question question)
  {
    var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);
    var sum = parts.First() - parts.Skip(1).Sum();
    Assert.True(sum >= sumAtLeast, $"Sum {sum} should be greater than {sumAtLeast}. Operation : {question.Key}");

    foreach (var number in parts)
      Assert.True((number >= 1 && number <= numberUpTo) || (number % 5 == 0 && number <= simpleNumberUpTo), $"Number {number} does not match substraction rules");
  }

  private void ThenMultiplicationRulesByLevelAreRespected(int numberUpTo, Question question)
  {
    var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);

    foreach (var number in parts)
      Assert.True(number >= 1 && number <= numberUpTo, $"Number {number} does not match multiplication rules");
  }

  private void ThenTheAnswerValidationIsCorrect(Question question)
  {
    var operationChar = ExerciceAsBase.OperationChar;
    var parts = question.Key.Split(operationChar);
    var answer = 0;
    var first = true;
    foreach (var part in parts.Select(int.Parse))
    {
      if (first == true)
      {
        answer = part;
        first = false;
      }
      else
      {
        switch (operationChar)
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

  private void ThenTheQuestionIsProperlyFormattedWithSameInfoAsKey(Question question)
  {
    Assert.Matches($@"Combien font {question.Key.Replace(ExerciceAsBase.OperationChar.ToString(), $" {ExerciceAsBase.OperationText} ")} ?", question.Text);
  }

  private void ThenTheQuestionKeyIsProperlyFormatted(Question question)
  {
    Assert.Matches($@"\d+\{ExerciceAsBase.OperationChar}\d+", question.Key);
  }
  private static void ThenIHaveAQuestion(Question question)
  {
    Assert.NotNull(question);
  }
}
