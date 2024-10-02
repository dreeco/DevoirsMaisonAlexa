namespace DevoirsAlexa.Domain.Enums;

public enum Levels
{
  [TextRepresentations("C.P", "CP", "cours préparatoire")]
  CP,

  [TextRepresentations("C.E.un", "C.E.1", "CE1", "Cours élémentaire 1")]
  CE1,

  [TextRepresentations("C.E.2", "C.E.deux", "CE2", "Cours élémentaire 2")]
  CE2,

  [TextRepresentations("C.M.un", "C.M.1", "CM1", "Cours moyen 1")]
  CM1,

  [TextRepresentations("C.M.2", "C.M.deux", "CM2", "Cours moyen 2")]
  CM2
}
