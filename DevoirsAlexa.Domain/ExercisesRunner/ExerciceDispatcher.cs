using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using System.Reflection;

namespace DevoirsAlexa.Domain.HomeworkExercisesRunner;

/// <summary>
/// Match ExerciceType enum to an question runner class
/// </summary>
public class ExerciceDispatcher
{
  /// <summary>
  /// Get the Question runner that is behind the exercice type
  /// </summary>
  /// <param name="exercice">The exercice type</param>
  /// <returns>A question runner, null if non found.</returns>
  public IExerciceQuestionsRunner? GetExerciceQuestionsRunner(HomeworkExercisesTypes exercice)
  {
    return Assembly
      .GetExecutingAssembly()
      .GetTypes()
      .Where(type => typeof(IExerciceQuestionsRunner).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
      .Select(t => Activator.CreateInstance(t) as IExerciceQuestionsRunner)
      .FirstOrDefault(t => t?.Type == exercice);
  }
}
