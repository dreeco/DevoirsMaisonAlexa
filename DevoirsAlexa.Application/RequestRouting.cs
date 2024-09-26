using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Application
{
  public class RequestRouting
  {
    public static HomeworkStep GetNextStep(IHomeworkSession session)
    {
      if (string.IsNullOrWhiteSpace(session.FirstName))
        return HomeworkStep.GetFirstName;
      else if (session.Age == null)
        return HomeworkStep.GetAge;
      else if (session.Exercice == null || session.Exercice == HomeworkExercisesTypes.Unknown)
        return HomeworkStep.GetExercice;
      else if (session.NbExercice == null || session.NbExercice < 1 || session.NbExercice > 20)
        return HomeworkStep.GetNbExercice;
      else
        return HomeworkStep.StartExercice;
    }

  }
}
