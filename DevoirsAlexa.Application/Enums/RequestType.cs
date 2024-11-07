namespace DevoirsAlexa.Application.Enums;

/// <summary>
/// What is the current request representing
/// </summary>
public enum RequestType
{
  /// <summary>
  /// Any intent. User will be guided to the exercice's questions
  /// </summary>
  Normal,

  /// <summary>
  /// User asked for help, according to the state of the exercice a sentence will be given to help.
  /// </summary>
  Help,

  /// <summary>
  /// The user asked to stop, either we stop or summarize the exercice status
  /// </summary>
  Stop
}
