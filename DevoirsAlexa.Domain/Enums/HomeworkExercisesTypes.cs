namespace DevoirsAlexa.Domain.Enums;

public enum HomeworkExercisesTypes
{
  Unknown,

  [TextRepresentations("addition")]
  Additions,

  [TextRepresentations("multiplication")]
  Multiplications,

  [TextRepresentations("soustraction", "substraction", "soustractions", "substractions")]
  Substractions,

  [TextRepresentations("dictée")]
  Dictation
}
