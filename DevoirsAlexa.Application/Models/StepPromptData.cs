using DevoirsAlexa.Domain;
using DevoirsAlexa.Application.Enums;

namespace DevoirsAlexa.Application.Models;

/// <summary>
/// Defines handler prompt and reprompt concerning RequestType
/// </summary>
internal class StepPromptsData
{
  /// <summary>
  /// Called when a <see cref="RequestType.Normal"/> is made
  /// </summary>
  public AskDelegate Ask { get; }

  public delegate void AskDelegate(ISentenceBuilder prompt, ISentenceBuilder reprompt);

  /// <summary>
  /// Called when a <see cref="RequestType.Help"/> is made
  /// </summary>
  public HelpDelegate Help { get; }
  public delegate void HelpDelegate(ISentenceBuilder prompt, ISentenceBuilder reprompt);

  /// <summary>
  /// Called when a <see cref="RequestType.Stop"/> is made
  /// </summary>
  public QuitDelegate Stop { get; }
  public delegate void QuitDelegate(ISentenceBuilder prompt);

  public StepPromptsData(AskDelegate ask, HelpDelegate help, QuitDelegate stop)
  {
    Ask = ask;
    Help = help;
    Stop = stop;
  }

  public void Call(RequestType state, ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    switch (state)
    {
      case RequestType.Normal:
        Ask(prompt, reprompt);
        break;
      case RequestType.Help:
        Help(prompt, reprompt);
        break;
      case RequestType.Stop:
        Stop(prompt);
        break;
    }
  }
}
