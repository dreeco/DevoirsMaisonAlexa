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
  /// <para>Should trigger the instantiationn of a <see cref="Exercises.MathExercices.AdditionsExercises"/></para>
  /// </summary>
  [TextRepresentations("addition")]
  Additions,

  /// <summary>
  /// Table of multiplications
  /// <para>Should trigger the instantiationn of a <see cref="Exercises.MathExercices.MultiplicationsExercises"/></para>
  /// </summary>
  [TextRepresentations("multiplication")]
  Multiplications,

  /// <summary>
  /// Table of substractions
  /// <para>Should trigger the instantiationn of a <see cref="Exercises.MathExercices.SubstractionsExercises"/></para>
  /// </summary>
  [TextRepresentations("soustraction", "substraction", "soustractions", "substractions")]
  Substractions,

  //[TextRepresentations("dictée")]
  //Dictation,

  /// <summary>
  /// Comparing and sorting numbers
  /// <para>Should trigger the instantiationn of a <see cref="Exercises.MathExercices.SortExercises"/></para>
  /// </summary>
  [TextRepresentations("tri", "tri nombres", "plus grand plus petit", "plus petit plus grand")]
  SortNumbers
}
