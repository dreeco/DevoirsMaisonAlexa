namespace DevoirsAlexa.Domain.Exercises;

public class ExerciceRule
{
  public string Name { get; private set; }
  public Func<string, bool> IsValid { get; }

  public ExerciceRule(string name, Func<string, bool> validityChecker)
  {
    Name = name ?? throw new ArgumentNullException(nameof(name));
    IsValid = validityChecker;
  }

}

//public static void toto() {
//  new ExerciceRule("Sum lower than 20", (string key) => key.Split('+').Select(p => int.Parse(p)).Sum() < 20);
//  new ExerciceRule("Sum lower than 40", (string key) => key.Split('+').Select(p => int.Parse(p)).Sum() < 40);

//  var t = 56;
//  new ExerciceRule("Sum lower than 40", (string key) => key.Split('+').Select(p => int.Parse(p)).Sum() < t);

//}
