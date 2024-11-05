using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Exercises.MathExercices;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

public class SubstractionsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
  public HomeworkExercisesTypes Type => HomeworkExercisesTypes.Substractions;
  private IDictionary<Levels, (int sumAtLeast, int numbersUpTo, int simpleNumbersUpTo)> LevelsBoundaries { get; set; }

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
