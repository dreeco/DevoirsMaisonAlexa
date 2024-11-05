using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using System.Reflection;

namespace DevoirsAlexa.Domain.HomeworkExercisesRunner;

public class ExerciceDispatcher
{
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
