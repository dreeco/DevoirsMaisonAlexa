using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Exercises.MathExercices;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

[Exercice(HomeworkExercisesTypes.Additions)]
public class AdditionsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
  private IDictionary<Levels, (int numbersUpTo, int sumSimpleNumbersUpTo)> LevelsBoundaries { get; set; }

  public AdditionsExercises() : base(Operations.Addition, "plus")
  {
    LevelsBoundaries = new Dictionary<Levels, (int numbersUpTo, int sumSimpleNumbersUpTo)>() {
      { Levels.CP, (10, 50)},
      { Levels.CE1, (20, 100)},
      { Levels.CE2, (30, 1000)},
      { Levels.CM1, (100, 10000)},
      { Levels.CM2, (1000, 100000)},
    };

    ExercisesRulesByLevel = LevelsBoundaries.ToDictionary(l => l.Key, l => GetAdditionsRules(LevelsBoundaries[l.Key]));
  }

  private ExerciceRule[] GetAdditionsRules((int numbersUpTo, int sumUpTo) boundaries)
  {
    return [MathHelper.GetRuleForMaxSumOf(boundaries.sumUpTo), MathHelper.GetRuleForNoComplicatedNumberAbove(boundaries.numbersUpTo)];
  }

  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
  {
    var boundaries = LevelsBoundaries[level];
    
    Func<(int left, int right)> func = MathHelper.GetRandomBoolean() ? 
      () => MathHelper.GetRandomSimpleNumbersWithSumUpTo(boundaries.sumSimpleNumbersUpTo) :
      () => MathHelper.GetRandomNumbersBothBetween(1, boundaries.numbersUpTo);
    
    return NextQuestion(func, ExercisesRulesByLevel[level], alreadyAsked);
  }
}
