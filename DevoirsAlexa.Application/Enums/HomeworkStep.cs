namespace DevoirsAlexa.Application.Enums;

/// <summary>
/// Different steps during the exercice session
/// </summary>
public enum HomeworkStep
{
  /// <summary>
  /// The user needs to tell its first name
  /// </summary>
  GetFirstName = 1,

  /// <summary>
  /// The user needs to tell its name
  /// </summary>
  GetLevel = 2,

  /// <summary>
  /// The user needs to tell which <cref>ExerciceType</cref> is chosen
  /// </summary>
  GetExercice = 3,

  /// <summary>
  /// The user needs to tell how many questions should be asked
  /// </summary>
  GetNbExercice = 4,

  /// <summary>
  /// The user has started the exercice, a question is aked and an answer is expected
  /// </summary>
  StartExercice = 5
}