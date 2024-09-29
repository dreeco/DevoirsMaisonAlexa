using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using System.Reflection;

namespace DevoirsAlexa.Domain.HomeworkExercisesRunner;

public class ExerciceDispatcher
{
  public IExerciceQuestionsRunner? GetExerciceQuestionsRunner(HomeworkExercisesTypes exercice)
  {
    var type = Assembly.GetExecutingAssembly().GetTypes()
      .FirstOrDefault(t => t.GetCustomAttribute<ExerciceAttribute>()?.ExerciceType == exercice);

    if (type == null)
      return null;

    return Activator.CreateInstance(type) as IExerciceQuestionsRunner;
  }
}
