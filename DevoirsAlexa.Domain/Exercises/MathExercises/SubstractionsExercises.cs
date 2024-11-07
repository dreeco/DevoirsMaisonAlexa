using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Helpers;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.Exercises.MathExercices;

/// <summary>
/// The exercice to get questions about simple substractions
/// <para>Will provide <see cref="Question">Question</see> such as key: 1-2 Text: "Combien font 1 moins 2 ?"</para>
/// </summary>
public class SubstractionsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
  /// <inheritdoc/>
  public HomeworkExercisesTypes Type => HomeworkExercisesTypes.Substractions;
  private IDictionary<Levels, (int sumAtLeast, int numbersUpTo, int simpleNumbersUpTo)> LevelsBoundaries { get; set; }

  /// <summary>
  /// Return an instance of <see cref="IExerciceQuestionsRunner"/> capable of providing substractions and checking answers
  /// </summary>
  public SubstractionsExercises() : base(Operations.Substraction, "moins")
  {
    LevelsBoundaries = new Dictionary<Levels, (int sumAtLeast, int numbersUpTo, int simpleNumbersUpTo)>() {
      { Levels.CP, (0, 10, 20)},
      { Levels.CE1, (-10, 20, 100)},
      { Levels.CE2, (-30, 30, 1000)},
      { Levels.CM1, (-100, 100, 10000)},
      { Levels.CM2, (-1000, 1000, 100000)},
    };

    ExercisesRulesByLevel = LevelsBoundaries.ToDictionary(l => l.Key, l => GetSubstractionRules(LevelsBoundaries[l.Key]));
  }

  private ExerciceRule[] GetSubstractionRules((int sumAtLeast, int numbersUpTo, int simpleNumbersUpTo) boundaries)
  {
    return [
      MathHelper.GetRuleForMinSubOf(boundaries.sumAtLeast),
      MathHelper.GetRuleForNoComplicatedNumberAbove(boundaries.numbersUpTo),
      MathHelper.GetRuleForNoSimpleNumberAbove(boundaries.simpleNumbersUpTo)
    ];
  }

  /// <inheritdoc/>
  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
  {
    var subWithSimpleNumbers = MathHelper.GetRandomBoolean();
    var boundaries = LevelsBoundaries[level];

    Func<(int left, int right)> func = subWithSimpleNumbers ?
      () => MathHelper.GetRandomSimpleNumbersWithSubBetween(boundaries.sumAtLeast, boundaries.simpleNumbersUpTo) :
      () =>
      {
        var numbers = MathHelper.GetRandomNumbersBothBetween(0, boundaries.numbersUpTo);
        return numbers.left > numbers.right ? numbers : (numbers.right, numbers.left);
      };

    return NextQuestion(func, ExercisesRulesByLevel[level], alreadyAsked);
  }
}
