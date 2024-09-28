//using DevoirsAlexa.Domain.Enums;
//using DevoirsAlexa.Domain.Exercises;
//using DevoirsAlexa.Domain.HomeworkExercises;
//using DevoirsAlexa.Domain.Models;

//namespace DevoirsAlexa.Domain.MathExercices;

//[Exercice(HomeworkExercisesTypes.Divisions)]
//public class DivisionsExercises : BaseTableExercises, IExerciceQuestionsRunner
//{
//  private IDictionary<Levels, (int[] acceptedModulos, int numbersUpTo)> LevelsBoundaries { get; set; }

//  public DivisionsExercises() : base(Operations.Division, "divisé par")
//  {
//    LevelsBoundaries = new Dictionary<Levels, (int[] acceptedModulos, int numbersUpTo)>() {
//      { Levels.CP, ([2], 10)},
//      { Levels.CE1, ([2, 5], 20)},
//      { Levels.CE2, ([2, 5], 100)},
//      { Levels.CM1, ([2, 5], 1000)},
//      { Levels.CM2, ([2, 5], 10000)},
//    };

//    ExercisesRulesByLevel = LevelsBoundaries.ToDictionary(l => l.Key, l => GetAdditionsRules(LevelsBoundaries[l.Key]));
//  }

//  private ExerciceRule[] GetAdditionsRules((int[] acceptedModulos, int numbersUpTo) boundaries)
//  {
//    return [GetRuleForRespectModulo(boundaries.acceptedModulos), GetRuleForNoComplicatedNumberAbove(boundaries.numbersUpTo)];
//  }

//  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
//  {
//    var boundaries = LevelsBoundaries[level];
//    return NextQuestion(() =>
//    {
//      var modChosen = boundaries.acceptedModulos[_RandomGenerator.Next(0, boundaries.acceptedModulos.Length)];

//      var left = _RandomGenerator.Next(2, boundaries.numbersUpTo) / modChosen;
//      var right = _RandomGenerator.Next(2, Math.Max(1, left) * modChosen) / modChosen;
//      return (left, right);
//    }, ExercisesRulesByLevel[level], alreadyAsked);
//  }
//}
