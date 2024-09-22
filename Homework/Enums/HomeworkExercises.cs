namespace Homework.Enums
{
  public enum HomeworkExercises
  {
    Unknown,

    [TextRepresentations("addition")]
    Additions,

    [TextRepresentations("multiplication")]
    Multiplications,

    [TextRepresentations("soustraction")]
    Subsctractions,

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