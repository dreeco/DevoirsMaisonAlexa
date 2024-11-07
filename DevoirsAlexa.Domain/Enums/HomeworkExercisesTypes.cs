namespace DevoirsAlexa.Domain.Enums;

/// <summary>
/// Different supported exercises types
/// </summary>
public enum HomeworkExercisesTypes
{
  /// <summary>
  /// Not parsable
  /// </summary>
  Unknown,

  /// <summary>
  /// Table of additions
  /// </summary>
  [TextRepresentations("addition")]
  Additions,

  /// <summary>
  /// Table of multiplications
  /// </summary>
  [TextRepresentations("multiplication")]
  Multiplications,

  /// <summary>
  /// Table of substractions
  /// </summary>
  [TextRepresentations("soustraction", "substraction", "soustractions", "substractions")]
  Substractions,

  //[TextRepresentations("dictée")]
  //Dictation,

  /// <summary>
  /// Comparing and sorting numbers
  /// </summary>
  [TextRepresentations("tri", "tri nombres", "plus grand plus petit", "plus petit plus grand")]
  SortNumbers
}
