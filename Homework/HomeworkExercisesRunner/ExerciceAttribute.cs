using Homework.Enums;

namespace Homework.HomeworkExercisesRunner;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class ExerciceAttribute : Attribute
{
  public HomeworkExercises ExerciceType { get; }

  public ExerciceAttribute(HomeworkExercises exerciceType)
  {
    ExerciceType = exerciceType;
  }
}
