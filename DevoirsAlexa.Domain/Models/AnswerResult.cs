namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// Represent the result when the user has given an answer
/// </summary>
public class AnswerResult
{
  /// <summary>
  /// How was the user answer
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
