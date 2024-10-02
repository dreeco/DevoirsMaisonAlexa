using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Domain;
using DevoirsAlexa.Application.Enums;

namespace DevoirsAlexa.Application.Models;


/// <summary>
/// Defines handler prompt and reprompt concerning RequestType
/// </summary>
internal class StepPromptsData
{
  /// <summary>
  /// Called when a RequestType.Normal is made
  /// </summary>
  public Action<ISentenceBuilder, ISentenceBuilder, IHomeworkSession> Ask { get; }

  /// <summary>
  /// Called when a RequestType.Help is made
  /// </summary>
  public Action<ISentenceBuilder, ISentenceBuilder, IHomeworkSession> Help { get; }

  /// <summary>
  /// Called when a RequestType.Stop is made
  /// </summary>
  public Action<ISentenceBuilder, IHomeworkSession> Stop { get; }

  public StepPromptsData(Action<ISentenceBuilder, ISentenceBuilder, IHomeworkSession> ask, Action<ISentenceBuilder, ISentenceBuilder, IHomeworkSession> help, Action<ISentenceBuilder, IHomeworkSession> stop)
  {
    Ask = ask;
    Help = help;
    Stop = stop;
  }

  public void Call(RequestType state, ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    switch (state)
    {
      case RequestType.Normal:
        Ask(prompt, reprompt, session);
        break;
      case RequestType.Help:
        Help(prompt, reprompt, session);
        break;
      case RequestType.Stop:
        Stop(prompt, session);
        break;
    }
  }
}
