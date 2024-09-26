
namespace DevoirsAlexa.Domain.ToRemove;

public interface IExerciceSentenceBuilder
{
  public void GetExerciceAnswerSentence(ISentenceBuilder sentenceBuilder, bool isValidAnswer, string correctAnswer);

  public void GetEndOfExerciceCompletionSentence(ISentenceBuilder sentenceBuilder, int nbCorrectAnswers, int nbQuestionAsked, TimeSpan totalTime);

}
