namespace DevoirsAlexa.Domain.Enums;

/// <summary>
/// The class levels. French classes
/// </summary>
public enum Levels
{
  /// <summary>
  /// Cours préparatoire
  /// </summary>
  [TextRepresentations("C.P", "CP", "cours préparatoire")]
  CP,

  /// <summary>
  /// Cours élémentaire 1
  /// </summary>
  [TextRepresentations("C.E.un", "C.E.1", "CE1", "Cours élémentaire 1")]
  CE1,

  /// <summary>
  /// Cours élémentaire 2
  /// </summary>
  [TextRepresentations("C.E.2", "C.E.deux", "CE2", "Cours élémentaire 2")]
  CE2,

  /// <summary>
  /// Cours moyen 1
  /// </summary>
  [TextRepresentations("C.M.un", "C.M.1", "CM1", "Cours moyen 1")]
  CM1,

  /// <summary>
  /// Cours moyen 2
  /// </summary>
  [TextRepresentations("C.M.2", "C.M.deux", "CM2", "Cours moyen 2")]
  CM2
}
