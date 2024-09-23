namespace Homework.Enums
{
  public enum HomeworkExercisesTypes
  {
    Unknown,

    [TextRepresentations("addition")]
    Additions,

    [TextRepresentations("multiplication")]
    Multiplications,

    [TextRepresentations("soustraction", "substraction")]
    Substractions,

    [TextRepresentations("division")]
    Divisions,

    [TextRepresentations("dictée")]
    Dictation
  }

  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  sealed class TextRepresentationsAttribute : Attribute
  {
    public string[] StringValue { get; }

    public TextRepresentationsAttribute(params string[] stringValue)
    {
      StringValue = stringValue;
    }
  }
}