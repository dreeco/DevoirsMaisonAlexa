using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Application.Models;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Application.Handlers
{
  /// <summary>
  /// Route matching against intent configuration
  /// </summary>
  public class NextRequestRouting
  {

    /// <summary>
    /// Represent the mapping between Intents, Steps and Slots
    /// </summary>
    private static IntentData[] Intents => [
      new IntentData("SetFirstName", [nameof(IHomeworkSession.FirstName)], HomeworkStep.GetFirstName),
      new IntentData("SetLevel", [nameof(IHomeworkSession.Level)], HomeworkStep.GetLevel),
      new IntentData("SetExercice", [nameof(IHomeworkSession.Exercice)], HomeworkStep.GetExercice),
      new IntentData("SetNbExercice", [nameof(IHomeworkSession.NbExercice)], HomeworkStep.GetNbExercice),
      new IntentData("SetAnswer", [nameof(IHomeworkSession.LastAnswer)], HomeworkStep.StartExercice, QuestionType.Integer),
      new IntentData("SetBoolAnswer", [nameof(IHomeworkSession.Answer)], HomeworkStep.StartExercice, QuestionType.Boolean),
    ];


    /// <summary>
    /// What is the next expected intent (Answer, first name, etc.)
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static IntentData GetNextExpectedIntent(IHomeworkSession session)
    {
      return Intents.First(i => i.RelatedStep == GetNextStep(session) && i.QuestionType == session.LastQuestionType);
    }


    /// <summary>
    /// What is the current intent
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IntentData? GetIntent(string name)
    {
      return Intents.FirstOrDefault(i => i.Name == name);
    }

    internal static HomeworkStep GetNextStep(IHomeworkSession session)
    {
      if (string.IsNullOrWhiteSpace(session.FirstName))
        return HomeworkStep.GetFirstName;
      else if (session.Level == null)
        return HomeworkStep.GetLevel;
      else if (session.Exercice == null || session.Exercice == HomeworkExercisesTypes.Unknown)
        return HomeworkStep.GetExercice;
      else if (session.NbExercice == null || session.NbExercice < 1 || session.NbExercice > 20)
        return HomeworkStep.GetNbExercice;
      else
        return HomeworkStep.StartExercice;
    }
  }
}
