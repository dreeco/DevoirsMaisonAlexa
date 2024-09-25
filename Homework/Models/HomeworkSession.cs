using Homework.Enums;
using System.Collections.Immutable;
using System.Globalization;

namespace Homework.Models;

public class HomeworkSession : Dictionary<string, object>
{
  public HomeworkSession() : base() { }
  public HomeworkSession(Dictionary<string, object>? session) : base(session ?? new Dictionary<string, object>()) { }
  public HomeworkSession(string? session) : base(CreateSessionFromCommaSeparatedKeyValues(session)) { }


  public string? FirstName { 
    get { return TryGetString(nameof(FirstName)); } 
    set { this[nameof(FirstName)] = value ?? string.Empty; } 
  }

  public int? Age
  {
    get { return int.TryParse(TryGetString(nameof(Age)), out var n) ? n : null; }
    set { this[nameof(Age)] = value?.ToString() ?? string.Empty; }
  }

  public int? NbExercice
  {
    get { return  int.TryParse(TryGetString(nameof(NbExercice)), out var n) ? n : null; }
    set { this[nameof(NbExercice)] = value?.ToString() ?? string.Empty; }
  }

  public int QuestionAsked
  {
    get { return int.TryParse(TryGetString(nameof(QuestionAsked)), out var n) ? n : 0; }
    set { this[nameof(QuestionAsked)] = value.ToString(); }
  }


  public int CorrectAnswers
  {
    get { return int.TryParse(TryGetString(nameof(CorrectAnswers)), out var n) ? n : 0; }
    set { this[nameof(CorrectAnswers)] = value.ToString(); }
  }

  public ImmutableArray<string> AlreadyAsked
  {
    get { return TryGetString(nameof(AlreadyAsked)).Split(';').Where(t => !string.IsNullOrWhiteSpace(t)).ToImmutableArray(); }
    set { this[nameof(AlreadyAsked)] = string.Join(';', value); }
  }

  public HomeworkExercisesTypes? Exercice
  {
    get { return GetExercice(TryGetString(nameof(Exercice))); }
    set { this[nameof(Exercice)] = value?.ToString() ?? string.Empty; }
  }

  public string? LastAnswer
  {
    get { return TryGetString(nameof(LastAnswer)); }
    set { this[nameof(LastAnswer)] = value ?? string.Empty; }
  }

  public DateTime? ExerciceStartTime
  {
    get {
      var s = TryGetString(nameof(ExerciceStartTime));
      return DateTime.TryParseExact(s, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var d) ? d.ToUniversalTime() : null; 
    }
    set { this[nameof(ExerciceStartTime)] = value?.ToString("o", CultureInfo.InvariantCulture) ?? string.Empty; }
  }

  private HomeworkExercisesTypes? GetExercice(string exerciceAsString)
  {
    if (Enum.TryParse<HomeworkExercisesTypes>(exerciceAsString, ignoreCase: true, out var exercice))
      return exercice;


    foreach (var e in Enum.GetValues<HomeworkExercisesTypes>())
    {
      if (exerciceAsString.Contains(e.ToString(), StringComparison.InvariantCultureIgnoreCase))
        return e;

      var textRepresentations = e.GetType()?.GetField(e.ToString())?.GetCustomAttributes(typeof(TextRepresentationsAttribute), false).FirstOrDefault() as TextRepresentationsAttribute;
      foreach (var representation in textRepresentations?.StringValue ?? [])
      {
        if (exerciceAsString.Contains(representation, StringComparison.InvariantCultureIgnoreCase))
          return e;
      }
    }
    return null;
  }

  public static HomeworkSession CreateSessionFromCommaSeparatedKeyValues(string? str)
  {
    var _this = new HomeworkSession();

    if (string.IsNullOrWhiteSpace(str))
      return _this;

    foreach (var s in str
      .Split(',') // separate different properties
      .Where(s => !string.IsNullOrWhiteSpace(s)) //Filter out empty ones
      .Select(s => s.Split('='))//Separate key from value
      )
      _this.Add(s[0], s[1]);

    return _this;
  }

  private bool TryGetString(string key, out string value)
  {
    value = string.Empty;
    if (!this.TryGetValue(key, out var valueObject))
      return false;
    value = (valueObject is string ? valueObject as string : valueObject.ToString()) ?? string.Empty;
    return true;

  }
  private string TryGetString(string key)
  {
    TryGetString(key, out var value);
    return value;
  }
}
