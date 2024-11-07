using DevoirsAlexa.Domain.Enums;
using System.Collections.Immutable;

namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// The session of the current exercice.
/// Renaming should be made with care because it is linked to Slot names and Intent mapping
/// This class should be decorrelated from the infrastructure HomeworkSession mapping
/// </summary>
public interface IHomeworkSession : IDictionary<string, object>
{
  /// <summary>
  /// First name of the user executing the exercice (for customization purpose in the sentences)
  /// </summary>
  public string? FirstName { get; set; }

  /// <summary>
  /// Class level, used to adapt the exercice level itself. From CP to CM2
  /// </summary>
  public Levels? Level { get; set; }

  /// <summary>
  /// Number of questions to ask within the exercice session.
  /// Used to check exercice's progress.
  /// </summary>
  public int? NbExercice { get; set; }

  /// <summary>
  /// Number of questions already asked during the exercice session.
  /// Used to check exercice's progress.
  /// </summary>
  public int QuestionAsked { get; set; }

  /// <summary>
  /// Number of correct answers during the current session.
  /// Should be less or equal to <cref>QuestionAsked</cref>
  /// Used for summary when the exercice is over.
  /// </summary>
  public int CorrectAnswers { get; set; }

  /// <summary>
  /// The list of questions keys already asked during the current session.
  /// Count should be equal to <cref>QuestionAsked</cref>.
  /// Used to prevent asking the same questions multiple times
  /// </summary>
  public ImmutableArray<string> AlreadyAsked { get; set; }

  /// <summary>
  /// The current Exercice.
  /// Used for triggering the right ExerciceQuestionsRunner;
  /// </summary>
  public HomeworkExercisesTypes? Exercice { get; set; }

  /// <summary>
  /// The latests integer answer by the user.
  /// Used by some exercises such as additions, multiplications, etc;
  /// </summary>
  public string? LastAnswer { get; set; }

  /// <summary>
  /// The latest boolean answer by the user.
  /// Used for some exercises such as SortNumbers
  /// </summary>
  public BooleanAnswer? Answer { get; set; }

  /// <summary>
  /// The date and time when the exercice started.
  /// Used for summary when the exercice is over.
  /// </summary>
  public DateTime? ExerciceStartTime { get; set; }

  /// <summary>
  /// The expected question type. Boolean or Integer
  /// Used to get the answer from the proper slot
  /// Should be resolved in infrastructure layer
  /// </summary>
  public QuestionType? LastQuestionType { get; set; }
}
