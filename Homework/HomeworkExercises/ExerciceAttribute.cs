using Homework.Enums;

namespace Homework.HomeworkExercises;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class ExerciceAttribute : Attribute
{
    public HomeworkExercisesTypes ExerciceType { get; }

    public ExerciceAttribute(HomeworkExercisesTypes exerciceType)
    {
        ExerciceType = exerciceType;
    }
}
