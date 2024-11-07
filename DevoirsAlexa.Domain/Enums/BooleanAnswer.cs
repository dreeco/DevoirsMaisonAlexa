namespace DevoirsAlexa.Domain.Enums;

/// <summary>
/// The answer under boolean format
/// </summary>
public enum BooleanAnswer
{
  /// <summary>
  /// Not parsable
  /// </summary>
  Unknown,

  /// <summary>
  /// The user answered "true"
  /// </summary>
  [TextRepresentations("true", "vrai", "oui", "vraie")]
  True,

  /// <summary>
  /// The user answered "false"
  /// </summary>
  [TextRepresentations("false", "faux", "non", "fausse")]
  False
}
