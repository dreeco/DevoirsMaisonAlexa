﻿using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models;
using System.Collections.Immutable;

namespace DevoirsAlexa.Infrastructure.Models;

/// <summary>
/// Represents the Session between the user and the skill
/// </summary>
public class HomeworkSession : Dictionary<string, object>, IHomeworkSession
{
  /// <summary>
  /// A constructor use for instantiating Homework session in the tests usually
  /// </summary>
  public HomeworkSession() : base() { }

  /// <summary>
  /// Instantiate an homework session from the Alexa session dictionary
  /// </summary>
  /// <param name="source">The Alexa session dictionary</param>
  public void FillFromSessionAttributes(Dictionary<string, object>? source)
  {
    if (source != null)
    {
      foreach (var kvp in source)
      {
        // Add or update the current instance with each key-value pair from the source
        this[kvp.Key] = kvp.Value;
      }
    }
  }

  /// <summary>
  /// Parse a string to instantiate an HomeworkSession
  /// </summary>
  /// <param name="session"></param>
  internal HomeworkSession(string? session) : base(CreateSessionFromCommaSeparatedKeyValues(session)) { }

  /// <inheritdoc/>
  public string? FirstName
  {
    get { return TryGetString(nameof(FirstName)); }
    set { this[nameof(FirstName)] = value ?? string.Empty; }
  }

  /// <inheritdoc/>
  public Levels? Level
  {
    get { return TryGetString(nameof(Level)).GetEnumFromTextRepresentations<Levels>(); }
    set { this[nameof(Level)] = value?.ToString() ?? string.Empty; }
  }

  /// <inheritdoc/>
  public int? NbExercice
  {
    get { return int.TryParse(TryGetString(nameof(NbExercice)), out var n) ? n : null; }
    set { this[nameof(NbExercice)] = value?.ToString() ?? string.Empty; }
  }

  /// <inheritdoc/>
  public int QuestionAsked
  {
    get { return int.TryParse(TryGetString(nameof(QuestionAsked)), out var n) ? n : 0; }
    set { this[nameof(QuestionAsked)] = value.ToString(); }
  }


  /// <inheritdoc/>
  public int CorrectAnswers
  {
    get { return int.TryParse(TryGetString(nameof(CorrectAnswers)), out var n) ? n : 0; }
    set { this[nameof(CorrectAnswers)] = value.ToString(); }
  }

  /// <inheritdoc/>
  public ImmutableArray<string> AlreadyAsked
  {
    get { return TryGetString(nameof(AlreadyAsked)).Split(';').Where(t => !string.IsNullOrWhiteSpace(t)).ToImmutableArray(); }
    set { this[nameof(AlreadyAsked)] = string.Join(';', value); }
  }

  /// <inheritdoc/>
  public HomeworkExercisesTypes? Exercice
  {
    get { return TryGetString(nameof(Exercice)).GetEnumFromTextRepresentations<HomeworkExercisesTypes>(); }
    set { this[nameof(Exercice)] = value?.ToString() ?? string.Empty; }
  }

  /// <inheritdoc/>
  public QuestionType? LastQuestionType
  {
    get { return TryGetString(nameof(LastQuestionType)).GetEnumFromTextRepresentations<QuestionType>(); }
    set { this[nameof(LastQuestionType)] = value?.ToString() ?? string.Empty; }
  }

  /// <inheritdoc/>
  public string? LastAnswer
  {
    get { return TryGetString(nameof(LastAnswer)); }
    set { this[nameof(LastAnswer)] = value ?? string.Empty; }
  }

  /// <inheritdoc/>
  public BooleanAnswer? Answer
  {
    get { return TryGetString(nameof(Answer)).GetEnumFromTextRepresentations<BooleanAnswer>(); }
    set { this[nameof(Answer)] = value?.ToString() ?? string.Empty; }
  }

  /// <inheritdoc/>
  public DateTime? ExerciceStartTime
  {
    get
    {
      var s = TryGetString(nameof(ExerciceStartTime));
      return long.TryParse(s, out var d) ? DateTime.FromFileTimeUtc(d) : null;
    }
    set { this[nameof(ExerciceStartTime)] = value?.ToFileTimeUtc().ToString() ?? string.Empty; }
  }

  /// <summary>
  /// Instantiate Homework session from a string.
  /// Used in the tests
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
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
