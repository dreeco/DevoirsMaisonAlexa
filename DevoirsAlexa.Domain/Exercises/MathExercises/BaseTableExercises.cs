using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Helpers;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.Exercises.MathExercices;

internal enum Operations
{
  Addition = '+',
  Multiplication = '*',
  Substraction = '-',
}

/// <summary>
/// Serves as a base to deliver basic table math exercises
/// </summary>
public abstract class BaseTableExercises
{
  internal Operations Operation { get; }
  internal char OperationChar => (char)Operation;
  internal string OperationText { get; }

  internal IDictionary<Levels, ExerciceRule[]> ExercisesRulesByLevel { get; set; }

  internal BaseTableExercises(Operations operation, string operationText)
  {
    Operation = operation;
    OperationText = operationText;

    ExercisesRulesByLevel = new Dictionary<Levels, ExerciceRule[]>();
  }

  /// <summary>
  /// Get next question as left and right numbers to be associated with an operation
  /// </summary>
  /// <param name="getNewNumbers">A pointer to a func that allows to get 2 numbers</param>
  /// <param name="rules">A list of rules to respect in order to be compliant with the level</param>
  /// <param name="alreadyAsked">The list of already asked question in order to avoid duplicates</param>
  /// <returns></returns>
  internal Question NextQuestion(Func<(int left, int right)> getNewNumbers, IEnumerable<ExerciceRule> rules, IEnumerable<string> alreadyAsked)
  {
    string key;
    var n = 0;
    var isValid = false;
    do
    {
      var numbers = getNewNumbers();
      key = $"{numbers.left}{OperationChar}{numbers.right}";
      isValid = n++ >= 1000 || (rules.All(r => r.IsValid(key)) && !alreadyAsked.Contains(key));
    }
    while (!isValid);

    return new Question(key, $"Combien font {key.Replace(OperationChar.ToString(), $" {OperationText} ")} ?", QuestionType.Integer, alreadyAsked.Count() + 1);
  }

  private int? GetCorrectAnswer(string questionKey)
  {
    var numbers = MathHelper.GetNumbersInQuestion(questionKey, OperationChar);
    if (numbers.Count() < 2)
      return null;

    int previous = 0;
    var first = true;
    foreach (var current in numbers)
    {
      if (first)
      {
        first = false;
        previous = current;
        continue;
      }

      switch (Operation)
      {
        case Operations.Addition:
          previous += current;
          break;
        case Operations.Multiplication:
          previous *= current;
          break;
        case Operations.Substraction:
          previous -= current;
          break;
      }
    }
    return previous;
  }

  /// <inheritdoc/>
  public AnswerValidation ValidateAnswer(string questionKey, string answer)
  {
    var resultNumber = GetCorrectAnswer(questionKey);

    var isValid = int.TryParse(answer, out var answerNumber) && answerNumber == resultNumber;
    var correctAnswer = resultNumber?.ToString() ?? string.Empty;
    return new AnswerValidation(isValid, correctAnswer);
  }

  /// <inheritdoc/>
  public HelpResult Help(string questionKey)
  {
    var text = $"Combien font {questionKey.Replace(OperationChar.ToString(), $" {OperationText} ")} ?";
    var resultNumber = GetCorrectAnswer(questionKey);
    if (resultNumber == null)
      return new HelpResult("Impossible de calculer la bonne réponse.", text, QuestionType.Integer);

    int n = resultNumber.Value;
    return new HelpResult($"La bonne réponse est entre {MathHelper.GetRandomNumberBetween(n - 10, n)} et {MathHelper.GetRandomNumberBetween(n, n + 10)}.", text, QuestionType.Integer);
  }

}
