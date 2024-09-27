namespace DevoirsAlexa.Domain.Enums;

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
