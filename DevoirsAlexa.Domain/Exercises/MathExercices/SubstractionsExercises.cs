using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

[Exercice(HomeworkExercisesTypes.Substractions)]
public class SubstractionsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
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
      GetRuleForMinSubOf(boundaries.sumAtLeast), 
      GetRuleForNoComplicatedNumberAbove(boundaries.numbersUpTo), 
      GetRuleForNoSimpleNumberAbove(boundaries.simpleNumbersUpTo)
    ];
  }

  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
  {
    var subWithSimpleNumbers = _RandomGenerator.Next(0, 1) == 1;
    var boundaries = LevelsBoundaries[level];

    Func<(int left, int right)> func = subWithSimpleNumbers ?
      () => GetRandomSimpleNumbersWithSubBetween(boundaries.sumAtLeast, boundaries.simpleNumbersUpTo) :
      () => GetRandomNumbersBothBetween(0, boundaries.numbersUpTo);

    return NextQuestion(func, ExercisesRulesByLevel[level], alreadyAsked);
  }
}


//using DevoirsAlexa.Domain.Enums;
//using DevoirsAlexa.Domain.HomeworkExercises;
//using DevoirsAlexa.Domain.Models;

//namespace DevoirsAlexa.Domain.MathExercices;

//[Exercice(HomeworkExercisesTypes.Substractions)]
//public class SubstractionsExercises : BaseTableExercises, IExerciceQuestionsRunner
//{
//  public SubstractionsExercises() : base(Operations.Substraction, "moins") { }

//  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
//  {
//    var min = 0;
//    var max = 1000;

//    switch (level)
//    {
//      case Levels.CP:
//        min = 1;
//        max = 4;
//        break;
//      case Levels.CE1:
//        min = 0;
//        max = 10;
//        break;
//      case Levels.CE2:
//        min = 0;
//        max = 30;
//        break;
//      case Levels.CM1:
//        min = 0;
//        max = 60;
//        break;
//      case Levels.CM2:
//        min = 0;
//        max = 100;
//        break;
//    }

//    return NextQuestion(min, max, alreadyAsked);
//  }
//}
