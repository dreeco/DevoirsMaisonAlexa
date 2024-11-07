using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.HomeworkExercisesRunner;

/// <summary>
/// Main class to operate exercises
/// Give the possibility to check an answer, get a question, end an exercice
/// </summary>
public class ExerciceRunner
{
  private IHomeworkSession SessionData { get; }
  private ExerciceDispatcher _dispatcher { get; }

  /// <summary>
  /// Setup from the current session state
  /// </summary>
  /// <param name="sessionData"></param>
  public ExerciceRunner(IHomeworkSession sessionData)
  {
    SessionData = sessionData;
    _dispatcher = new ExerciceDispatcher();
  }

  /// <summary>
  /// Validate the answer
  /// Returns a new question if exercice is not over
  /// Returns
  /// </summary>
  /// <param name="isStopping">Did the user asked to stop the exercice</param>
  /// <returns>The <cref>AnswerResult</cref> with:
  ///   <para> - An <cref>AnswerValidation</cref> if a question was already asked.</para>
  ///   <para> - A new <cref>Question</cref> if the exercice is not over.</para>
  ///   <para> - The <cref>ExerciceResult</cref> (summary of the exercice) if it is over.</para>
  /// </returns>
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
      answerResult.Exercice = new ExerciceResult(timeInSeconds, SessionData.CorrectAnswers, SessionData.QuestionAsked);

      EndSession(isStopping);
      return answerResult;
    }

    answerResult.Question = exercice.NextQuestion(SessionData.Level.Value, SessionData.AlreadyAsked);
    AddNewQuestionToSession(answerResult.Question);

    return answerResult;
  }

  /// <summary>
  /// Get some help over the exercice
  /// </summary>
  /// <returns>A <cref>AnswerResult</cref> object with a <cref>HelpResult</cref> containing a sentence that should help the user without giving the answer.</returns>
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
    SessionData.CorrectAnswers = 0;
    SessionData.AlreadyAsked = SessionData.AlreadyAsked.Clear();
    SessionData.QuestionAsked = 0;
    SessionData.Exercice = null;
    SessionData.NbExercice = 0;
    SessionData.ExerciceStartTime = null;
    SessionData.LastAnswer = null;

    if (isStopping)
      SessionData.Clear();

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
  }

  private AnswerValidation ValidateAnswer(IExerciceQuestionsRunner exercice)
  {
    var lastAsked = SessionData.AlreadyAsked.Last();

    var answer = SessionData.LastAnswer;
    if (string.IsNullOrEmpty(answer))
      answer = SessionData.Answer.ToString() ?? string.Empty;

    var answerValidation = exercice.ValidateAnswer(lastAsked, answer);

    if (answerValidation.IsValid)
      SessionData.CorrectAnswers++;

    return answerValidation;
  }

  internal string? GetCorrectAnswer(HomeworkExercisesTypes exercice, string questionKey)
  {
    return GetExerciceQuestionsRunner(exercice)?.ValidateAnswer(questionKey, string.Empty).CorrectAnswer;
  }
}
