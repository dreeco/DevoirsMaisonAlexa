namespace DevoirsAlexa.Domain.Enums;

public enum BooleanAnswer
{
  Unknown,
  [TextRepresentations("true", "vrai", "oui", "vraie")]
  True,
  [TextRepresentations("false", "faux", "non", "fausse")]
  False
}
