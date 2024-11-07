using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Helpers;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.Exercises.MathExercices;

/// <summary>
/// The exercice to get questions about simple multiplications
/// </summary>
public class MultiplicationsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
  /// <inheritdoc/>
  public HomeworkExercisesTypes Type => HomeworkExercisesTypes.Multiplications;
  private IDictionary<Levels, int> LevelsBoundaries { get; set; }

  /// <summary>
  /// Get the question runner
  /// </summary>
  public MultiplicationsExercises() : base(Operations.Multiplication, "multiplié par")
  {
    LevelsBoundaries = new Dictionary<Levels, int>() {
      { Levels.CP, 4},
      { Levels.CE1, 5},
      { Levels.CE2, 10},
      { Levels.CM1, 20},
      { Levels.CM2, 50},
    };

    ExercisesRulesByLevel = LevelsBoundaries.ToDictionary(l => l.Key, l => GetMultiplicationRules(LevelsBoundaries[l.Key]));
  }

  private ExerciceRule[] GetMultiplicationRules(int boundary)
  {
    return [MathHelper.GetRuleForNoComplicatedNumberAbove(boundary)];
  }

  /// <inheritdoc/>
  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
  {
    return NextQuestion(() => MathHelper.GetRandomNumbersBothBetween(1, LevelsBoundaries[level]), ExercisesRulesByLevel[level], alreadyAsked);
  }
}
