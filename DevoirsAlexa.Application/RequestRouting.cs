using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Application
{
  public class RequestRouting
  {
    private static IntentData[] Intents = [
    new IntentData("SetFirstName") { Slots = [nameof(IHomeworkSession.FirstName)], RelatedStep = HomeworkStep.GetFirstName },
    new IntentData("SetLevel") { Slots = [nameof(IHomeworkSession.Level)], RelatedStep = HomeworkStep.GetLevel },
    new IntentData("SetExercice") { Slots = [nameof(IHomeworkSession.Exercice)], RelatedStep = HomeworkStep.GetExercice },
    new IntentData("SetNbExercice") { Slots = [nameof(IHomeworkSession.NbExercice)], RelatedStep = HomeworkStep.GetNbExercice },
    new IntentData("SetAnswer") { Slots = [nameof(IHomeworkSession.LastAnswer)], RelatedStep = HomeworkStep.StartExercice },
  ];

    public static IntentData? GetNextExpectedIntent(IHomeworkSession session)
    {
      return Intents.FirstOrDefault(i => i.RelatedStep == GetNextStep(session));
    }

    public static IntentData? GetIntent(string name)
    {
      return Intents.FirstOrDefault(i => i.Name == name);
    }

    public static HomeworkStep GetNextStep(IHomeworkSession session)
    {
      if (string.IsNullOrWhiteSpace(session.FirstName))
        return HomeworkStep.GetFirstName;
      else if (session.Level == null || session.Level == Levels.Unknown)
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
