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
  [InlineData(Levels.CP, 10, 50, 45)]
  [InlineData(Levels.CE1, 20, 100, 45)]
  [InlineData(Levels.CE2, 30, 1000, 45)]
  [InlineData(Levels.CM1, 100, 10000, 45)]
  [InlineData(Levels.CM2, 1000, 100000, 45)]

  [InlineData(Levels.CP, 10, 50, 5)]
  [InlineData(Levels.CE1, 20, 100, 5)]
  [InlineData(Levels.CE2, 30, 1000, 5)]
  [InlineData(Levels.CM1, 100, 10000, 5)]
  [InlineData(Levels.CM2, 1000, 100000, 5)]
  public void ShouldAskForAProperAddition_WhenGettingQuestionAfterLevel(Levels level, int numberUpTo, int sumSimpleNumberUpTo, int loopSize)
  {
    exercice = new AdditionsExercises();
    var p = (numberUpTo - 1) + ((sumSimpleNumberUpTo - numberUpTo) / 5);
    var nbDifferentAnswersPossible = p*p;
    var checkExerciceRules = (Question question) => { ThenAdditionRulesByLevelAreRespected(numberUpTo, sumSimpleNumberUpTo, question); };
    RunLoopTestForExercice(level, checkExerciceRules, loopSize, nbDifferentAnswersPossible);
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

  [Theory]
  [InlineData(Levels.CP, 0, 10, 50, 45)]
  [InlineData(Levels.CE1, -10, 20, 100, 45)]
  [InlineData(Levels.CE2, -30, 30, 1000, 45)]
  [InlineData(Levels.CM1, -100, 100, 10000, 45)]
  [InlineData(Levels.CM2, -1000, 1000, 100000, 45)]

  [InlineData(Levels.CP, 0, 10, 50, 5)]
  [InlineData(Levels.CE1, -10, 20, 100, 5)]
  [InlineData(Levels.CE2, -30, 30, 1000, 5)]
  [InlineData(Levels.CM1, -100, 100, 10000, 5)]
  [InlineData(Levels.CM2, -1000, 1000, 100000, 5)]

  public void ShouldAskForAProperSubstraction_WhenGettingQuestionAfterLevel(Levels level, int sumAtLeast, int numberUpTo, int simpleNumbersUpTo, int loopSize)
  {
    exercice = new SubstractionsExercises();
    var p = (numberUpTo - 1) + ((simpleNumbersUpTo - numberUpTo) / 5);
    var nbDifferentAnswersPossible = p*p;
    var checkExerciceRules = (Question question) => { ThenSubstractionRulesByLevelAreRespected(numberUpTo, simpleNumbersUpTo, sumAtLeast, question); };
    RunLoopTestForExercice(level, checkExerciceRules, loopSize, nbDifferentAnswersPossible);
  }

  //[Theory]
  //[InlineData(Levels.CP, "2", 10, 45)]
  //[InlineData(Levels.CE1, "2,5", 20, 45)]
  //[InlineData(Levels.CE2, "2,5", 100, 45)]
  //[InlineData(Levels.CM1, "2,5", 1000, 45)]
  //[InlineData(Levels.CM2, "2,5", 10000, 45)]

  //[InlineData(Levels.CP, "2", 10, 5)]
  //[InlineData(Levels.CE1, "2,5", 20, 5)]
  //[InlineData(Levels.CE2, "2,5", 100, 5)]
  //[InlineData(Levels.CM1, "2,5", 1000, 5)]
  //[InlineData(Levels.CM2, "2,5", 10000, 5)]

  //public void ShouldAskForAProperDivision_WhenGettingQuestionAfterLevel(Levels level, string modulosString, int numberUpTo, int loopSize)
  //{
  //  exercice = new DivisionsExercises();
  //  var modulos = modulosString.Split(',').Select(int.Parse);
  //  var nbDifferentAnswersPossible = modulos.Sum(modulo => numberUpTo / modulo);
  //  var checkExerciceRules = (Question question) => { ThenDivisionRulesByLevelAreRespected(numberUpTo, modulos, question); };
  //  RunLoopTestForExercice(level, checkExerciceRules, loopSize, nbDifferentAnswersPossible);
  //}

  [Theory]
  [InlineData("", "", "")]
  [InlineData("2_2", "4", "")]
  [InlineData("2-2", "4", "")]
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

  private static void ThenIHaveAtLeast75PercentDifferentQuestions(int nbDifferentAnswersPossible, List<string> alreadyAsked, int loopSize)
  {
    nbDifferentAnswersPossible = Math.Min(nbDifferentAnswersPossible, loopSize);
    nbDifferentAnswersPossible = (int)Math.Floor(nbDifferentAnswersPossible * 0.9);
    var foundDifferentQuestions = alreadyAsked.Distinct().Count();
    Assert.True(nbDifferentAnswersPossible <= foundDifferentQuestions, $"Expected a minimum of {nbDifferentAnswersPossible} different questions but was {foundDifferentQuestions}. {string.Join(';', alreadyAsked)}");
  }

  private void RunLoopTestForExercice(Levels level, Action<Question> questionRespectExerciceRules, int loopSize, int nbDifferentAnswersPossible)
  {
    var alreadyAsked = new List<string>();
    for (var n = 0; n < loopSize; n++)
    {
      Question question = WhenIGetTheNextQuestion(level, alreadyAsked);

      ThenIHaveAProperlyBuiltQuestionWithAPossibleAnswer(alreadyAsked, question);
      questionRespectExerciceRules(question);
    }

    ThenIHaveAtLeast75PercentDifferentQuestions(nbDifferentAnswersPossible, alreadyAsked, loopSize);
  }

  private void ThenAdditionRulesByLevelAreRespected(int numberUpTo, int sumSimpleNumberUpTo, Question question)
  {
    var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);
    var sum = parts.Sum();

    foreach (var number in parts)
      Assert.True((number >= 1 && number <= numberUpTo) || (number % 5 == 0 && sum < sumSimpleNumberUpTo), $"Number {number} does not match addition rules");
  }

  private void ThenSubstractionRulesByLevelAreRespected(int numberUpTo, int simpleNumberUpTo, int sumAtLeast, Question question)
  {
    var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);
    var sum = parts.First() - parts.Skip(1).Sum();
    Assert.True(sum >= sumAtLeast, $"Sum {sum} should be greater than {sumAtLeast}. Operation : {question.Key}");

    foreach (var number in parts)
      Assert.True((number >= 1 && number <= numberUpTo) || (number % 5 == 0 && number < simpleNumberUpTo), $"Number {number} does not match substraction rules");
  }

  private void ThenMultiplicationRulesByLevelAreRespected(int numberUpTo, Question question)
  {
    var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);

    foreach (var number in parts)
      Assert.True(number >= 1 && number <= numberUpTo, $"Number {number} does not match multiplication rules");
  }

  //private void ThenDivisionRulesByLevelAreRespected(int numberUpTo, IEnumerable<int> modulos, Question question)
  //{
  //  var parts = question.Key.Split(ExerciceAsBase.OperationChar).Select(int.Parse);
  //  var result = parts.First();
  //  foreach (var number in parts.Skip(1))
  //    result /= number;
  //  Assert.Contains(modulos, modulo => result % modulo == 0);
  //}

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
