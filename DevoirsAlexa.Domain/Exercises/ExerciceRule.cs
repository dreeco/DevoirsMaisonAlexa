namespace DevoirsAlexa.Domain.Exercises;

/// <summary>
/// A rule to be checked against the question key in order to assert that it matched what is possible.
/// It is usually linked to different question complexity according to class level.
/// </summary>
/// <param name="Name">Name of the rule for debugging purpose</param>
/// <param name="IsValid">Checker to verify if the question key matches the rule</param>
public record ExerciceRule(string Name, Func<string, bool> IsValid);
