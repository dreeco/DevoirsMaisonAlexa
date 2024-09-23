using Homework.Enums;
using Homework.HomeworkExercises;
using System.Reflection;

namespace Homework.HomeworkExercisesRunner
{
    public class ExerciceDispatcher
  {
    public IExerciceQuestionsRunner? GetExerciceQuestionsRunner(HomeworkExercisesTypes exercice)
    {
      var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.GetInterface(typeof(IExerciceQuestionsRunner).Name) != null && t.GetCustomAttribute<ExerciceAttribute>()?.ExerciceType == exercice);

      if (type == null)
        return null;

      return Activator.CreateInstance(type) as IExerciceQuestionsRunner;
    }
  }
}
