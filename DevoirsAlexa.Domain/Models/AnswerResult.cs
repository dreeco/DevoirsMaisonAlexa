namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// Represent the result when the user has given an answer
/// </summary>
public class AnswerResult
{
  /// <summary>
  /// Contains information about the user answer (<see cref="AnswerValidation.IsValid">valid or not</see>, provide a <see cref="AnswerValidation.CorrectAnswer"/>, etc.)
  /// </summary>
  public AnswerValidation? Validation { get; set; }

  /// <summary>
  /// Next question to ask to the user
  /// </summary>
  public Question? Question { get; set; }

  /// <summary>
  /// Summarize the exercice final result
  /// </summary>
  public ExerciceResult? Exercice { get; set; }

  /// <summary>
  /// Filled if the user asked for help
  /// </summary>
  public HelpResult? Help { get; set; }

  /// <summary>
  /// If there is an error with the current status
  /// </summary>
  public bool CouldNotStart { get; set; }
}
