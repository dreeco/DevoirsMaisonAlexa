using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models;
using System.Collections.Immutable;

namespace DevoirsAlexa.Infrastructure.Models;

public class HomeworkSession : Dictionary<string, object>, IHomeworkSession
{
  public HomeworkSession() : base() { }
  public HomeworkSession(Dictionary<string, object>? session) : base(session ?? new Dictionary<string, object>()) { }
  public HomeworkSession(string? session) : base(CreateSessionFromCommaSeparatedKeyValues(session)) { }


  public string? FirstName { 
    get { return TryGetString(nameof(FirstName)); } 
    set { this[nameof(FirstName)] = value ?? string.Empty; } 
  }

  public Levels? Level
  {
    get { return TryGetString(nameof(Level)).GetEnumFromTextRepresentations<Levels>(); }
    set { this[nameof(Level)] = value?.ToString() ?? string.Empty; }
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
    get { return TryGetString(nameof(Exercice)).GetEnumFromTextRepresentations<HomeworkExercisesTypes>(); }
    set { this[nameof(Exercice)] = value?.ToString() ?? string.Empty; }
  }

  public QuestionType? LastQuestionType
  {
    get { return TryGetString(nameof(LastQuestionType)).GetEnumFromTextRepresentations<QuestionType>(); }
    set { this[nameof(LastQuestionType)] = value?.ToString() ?? string.Empty; }
  }

  public string? LastAnswer
  {
    get { return TryGetString(nameof(LastAnswer)); }
    set { this[nameof(LastAnswer)] = value ?? string.Empty; }
  }

  public BooleanAnswer? Answer
  {
    get { return TryGetString(nameof(Answer)).GetEnumFromTextRepresentations<BooleanAnswer>(); }
    set { this[nameof(Answer)] = value?.ToString() ?? string.Empty; }
  }

  public DateTime? ExerciceStartTime
  {
    get {
      var s = TryGetString(nameof(ExerciceStartTime));
      return long.TryParse(s, out var d) ? DateTime.FromFileTimeUtc(d) : null;
    }
    set { this[nameof(ExerciceStartTime)] = value?.ToFileTimeUtc().ToString() ?? string.Empty; }
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
    value = valueObject.ToString() ?? string.Empty;
    return true;

  }
  private string TryGetString(string key)
  {
    TryGetString(key, out var value);
    return value;
  }
}
