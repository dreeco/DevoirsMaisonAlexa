using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Domain.ToRemove;

namespace DevoirsAlexa.Domain.HomeworkExercisesRunner;

public class ExerciceRunner
{
  private IHomeworkSession SessionData { get; }
  private ExerciceDispatcher _dispatcher { get; }

  public string FirstName => SessionData?.FirstName ?? string.Empty;
  public int? Age => SessionData?.Age;

  public string? LastQuestionKey => SessionData?.AlreadyAsked.LastOrDefault();

  public ExerciceRunner(IHomeworkSession sessionData)
  {
    SessionData = sessionData;
    _dispatcher = new ExerciceDispatcher();
  }

  public string NextQuestion(IExerciceSentenceBuilder exerciceSentenceBuilder, ISentenceBuilder sentenceBuilder)
  {
    if (SessionData.Age == null)
      throw new ArgumentNullException(nameof(SessionData.Age));
    var exercice = GetExerciceQuestionsRunner();

    if (!string.IsNullOrEmpty(SessionData.AlreadyAsked.LastOrDefault()))
      ValidateAnswer(exerciceSentenceBuilder, sentenceBuilder, exercice, SessionData.LastAnswer);

    if (SessionData.QuestionAsked >= SessionData.NbExercice)
    {
      var timeInSeconds = DateTime.UtcNow - SessionData.ExerciceStartTime ?? TimeSpan.Zero;
      sentenceBuilder.AppendSimpleText(" ");
      EndSession(exerciceSentenceBuilder, sentenceBuilder, continueAfter: true);
      return string.Empty;
    }

    var question = exercice.NextQuestion(SessionData.Age.Value, SessionData.AlreadyAsked);
    AddNewQuestionToSession(question);

    if (SessionData.QuestionAsked == 1)
      sentenceBuilder.AppendInterjection("C'est parti");

    sentenceBuilder.AppendSimpleText(" " + question.Text);
    
    return question.Text;
  }

  public void EndSession(IExerciceSentenceBuilder exerciceSentenceBuilder, ISentenceBuilder sentenceBuilder, bool continueAfter) {
    if (SessionData.QuestionAsked == 0)
    {
      sentenceBuilder.AppendInterjection("Au revoir !");
      return;
    }

    var timeSpan = (DateTime.UtcNow - SessionData.ExerciceStartTime) ?? TimeSpan.Zero;
    exerciceSentenceBuilder.GetEndOfExerciceCompletionSentence(sentenceBuilder, SessionData.CorrectAnswers, SessionData.QuestionAsked, timeSpan);
    if (continueAfter)
    {
      sentenceBuilder.AppendSimpleText(" Quel exercice souhaites-tu faire désormais ?");
      ResetSessionAfterExercice();
    }
    else
    {
      SessionData.Clear();
    }
  }

  private IExerciceQuestionsRunner GetExerciceQuestionsRunner()
  {
    if (SessionData.Exercice == null || SessionData.Exercice == HomeworkExercisesTypes.Unknown)
      throw new ArgumentNullException(nameof(SessionData.Exercice));

    return _dispatcher.GetExerciceQuestionsRunner(SessionData.Exercice.Value) ?? throw new ArgumentNullException(nameof(SessionData.Exercice));
  }

  private void AddNewQuestionToSession(Question question)
  {
    if (SessionData.ExerciceStartTime == null)
      SessionData.ExerciceStartTime = DateTime.UtcNow;

    if (!SessionData.AlreadyAsked.Contains(question.Key))
      SessionData.AlreadyAsked = SessionData.AlreadyAsked.Add(question.Key);

    SessionData.QuestionAsked++;
  }

  private void ResetSessionAfterExercice()
  {
    SessionData.CorrectAnswers = 0;
    SessionData.AlreadyAsked = SessionData.AlreadyAsked.Clear();
    SessionData.QuestionAsked = 0;
    SessionData.Exercice = null;
    SessionData.NbExercice = 0;
    SessionData.ExerciceStartTime = null;
  }

  private void ValidateAnswer(IExerciceSentenceBuilder exerciceSentenceBuilder, ISentenceBuilder sentenceBuilder, IExerciceQuestionsRunner exercice, string? answer)
  {
    var lastAsked = SessionData.AlreadyAsked.Last();
    var answerValidation = exercice.ValidateAnswer(lastAsked, answer ?? string.Empty);

    if (answerValidation.IsValid)
      SessionData.CorrectAnswers++;

    exerciceSentenceBuilder.GetExerciceAnswerSentence(sentenceBuilder, answerValidation.IsValid, answerValidation.CorrectAnswer);
  }

  public string GetCorrectAnswer(string questionKey)
  {
    return GetExerciceQuestionsRunner().ValidateAnswer(questionKey, string.Empty).CorrectAnswer;
  }
}
