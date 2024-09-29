namespace DevoirsAlexa.Domain.Exercises;

public class ExerciceRule
{
  public string Name { get; private set; }
  public Func<string, bool> IsValid { get; }

  public ExerciceRule(string name, Func<string, bool> validityChecker)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentNullException(nameof(name));
    
    Name = name;
    IsValid = validityChecker;
  }
}
