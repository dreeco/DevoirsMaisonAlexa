using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;
using System.Reflection.Metadata.Ecma335;

namespace DevoirsAlexa.Domain.HomeworkExercisesRunner;

public class ExerciceRunner
{
  private IHomeworkSession SessionData { get; }
  private ExerciceDispatcher _dispatcher { get; }

  public ExerciceRunner(IHomeworkSession sessionData)
  {
    SessionData = sessionData;
    _dispatcher = new ExerciceDispatcher();
  }

  public AnswerResult ValidateAnswerAndGetNext(bool isStopping)
  {
    IExerciceQuestionsRunner exercice;
    var answerResult = new AnswerResult();

    IExerciceQuestionsRunner? e;
    if (SessionData.Level == null ||
      SessionData.Exercice == null ||
      SessionData.NbExercice == null ||
      (e = GetExerciceQuestionsRunner(SessionData.Exercice.Value)) == null)
    {
      answerResult.CouldNotStart = true;
      return answerResult;
    }

    exercice = e;

    if (!isStopping && !string.IsNullOrEmpty(SessionData.AlreadyAsked.LastOrDefault()))
      answerResult.Validation = ValidateAnswer(exercice);

    if (isStopping || SessionData.QuestionAsked >= SessionData.NbExercice)
    {
      var timeInSeconds = DateTime.UtcNow - SessionData.ExerciceStartTime ?? TimeSpan.Zero;
      answerResult.Exercice = new ExerciceResult
      {
        CorrectAnswers = SessionData.CorrectAnswers,
        ElapsedTime = timeInSeconds,
        TotalQuestions = SessionData.QuestionAsked
      };

      EndSession(isStopping);
      return answerResult;
    }

    answerResult.Question = exercice.NextQuestion(SessionData.Level.Value, SessionData.AlreadyAsked);
    AddNewQuestionToSession(answerResult.Question);

    return answerResult;
  }

  public AnswerResult Help()
  {
    IExerciceQuestionsRunner? e;

    var key = SessionData.AlreadyAsked.LastOrDefault();
    if (SessionData.Exercice == null || (e = GetExerciceQuestionsRunner(SessionData.Exercice.Value)) == null || key == null)
      return new AnswerResult { CouldNotStart = true };

    return new AnswerResult { Help = e.Help(key) };
  }


  private void EndSession(bool isStopping)
  {
    if (isStopping)
    {
      SessionData.Clear();
    }
    else
    {
      ResetSessionAfterExercice();
    }
  }

  private IExerciceQuestionsRunner? GetExerciceQuestionsRunner(HomeworkExercisesTypes exercice)
  {
    return _dispatcher.GetExerciceQuestionsRunner(exercice);
  }

  private void AddNewQuestionToSession(Question question)
  {
    if (SessionData.ExerciceStartTime == null)
      SessionData.ExerciceStartTime = DateTime.UtcNow;

    if (!SessionData.AlreadyAsked.Contains(question.Key))
      SessionData.AlreadyAsked = SessionData.AlreadyAsked.Add(question.Key);

    SessionData.QuestionAsked++;
    question.Index = SessionData.QuestionAsked;
  }

  private void ResetSessionAfterExercice()
  {
    SessionData.CorrectAnswers = 0;
    SessionData.AlreadyAsked = SessionData.AlreadyAsked.Clear();
    SessionData.QuestionAsked = 0;
    SessionData.Exercice = null;
    SessionData.NbExercice = 0;
    SessionData.ExerciceStartTime = null;
    SessionData.LastAnswer = null;
  }

  private AnswerValidation ValidateAnswer(IExerciceQuestionsRunner exercice)
  {
    var lastAsked = SessionData.AlreadyAsked.Last();
    var answerValidation = exercice.ValidateAnswer(lastAsked, SessionData.LastAnswer ?? string.Empty);

    if (answerValidation.IsValid)
      SessionData.CorrectAnswers++;

    return answerValidation;
  }

  public string? GetCorrectAnswer(HomeworkExercisesTypes exercice, string questionKey)
  {
    return GetExerciceQuestionsRunner(exercice)?.ValidateAnswer(questionKey, string.Empty).CorrectAnswer;
  }
}
