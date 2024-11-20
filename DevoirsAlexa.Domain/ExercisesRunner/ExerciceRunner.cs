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
  private Func<HomeworkExercisesTypes, IExerciceQuestionsRunner> ExerciceFactory { get; }
  private IHomeworkSession SessionData { get; }

  private IExerciceQuestionsRunner? _exercice;

#pragma warning disable CS8629 // Nullable value type may be null.
  private IExerciceQuestionsRunner Exercice => (_exercice ??= ExerciceFactory(SessionData.Exercice.Value));
#pragma warning restore CS8629 // Nullable value type may be null.


  /// <summary>
  /// Instantiate the exercice session runner
  /// </summary>
  /// <param name="exerciceFactory">Factory to get an exercice from its type</param>
  /// <param name="sessionData">The current session state</param>
  public ExerciceRunner(Func<HomeworkExercisesTypes, IExerciceQuestionsRunner> exerciceFactory, IHomeworkSession sessionData)
  {
    ExerciceFactory = exerciceFactory;
    SessionData = sessionData;
  }

  /// <summary>
  /// Should be called when a user answer a question or asks to stop the current exercice session.
  /// Validate the <see cref="IHomeworkSession.Answer"/> or <see cref="IHomeworkSession.LastAnswer"/> against the <see cref="IExerciceQuestionsRunner.ValidateAnswer(string, string)"/>
  /// Returns a new <see cref="Question"/> after the <see cref="IExerciceQuestionsRunner.NextQuestion(Levels, IEnumerable{string})"/> if needed
  /// </summary>
  /// <param name="isStopping">Did the user asked to stop the exercice</param>
  /// <returns>The <see cref="AnswerResult">AnswerResult</see> with:
  ///   <para> - An <see cref="AnswerValidation">AnswerValidation</see> if a question was already asked: the status of this answer (valid or not).</para>
  ///   <para> - A new <see cref="Question">Question</see> if the exercice is not over.</para>
  ///   <para> - The <see cref="ExerciceResult">ExerciceResult</see> (summary of the exercice) if it is over.</para>
  /// </returns>
  public AnswerResult ValidateAnswerAndGetNext(bool isStopping)
  {
    var answerResult = new AnswerResult();

    if (SessionData.Level == null ||
      SessionData.Exercice == null ||
      SessionData.NbExercice == null)
    {
      answerResult.CouldNotStart = true;
      return answerResult;
    }

    if (!isStopping && !string.IsNullOrEmpty(SessionData.AlreadyAsked.LastOrDefault()))
      answerResult.Validation = ValidateAnswer();

    if (isStopping || SessionData.QuestionAsked >= SessionData.NbExercice)
    {
      var timeInSeconds = DateTime.UtcNow - SessionData.ExerciceStartTime ?? TimeSpan.Zero;
      if (isStopping)
        SessionData.QuestionAsked--;
      answerResult.Exercice = new ExerciceResult(timeInSeconds, SessionData.CorrectAnswers, SessionData.QuestionAsked);

      EndSession(isStopping);
      return answerResult;
    }

    answerResult.Question = Exercice.NextQuestion(SessionData.Level.Value, SessionData.AlreadyAsked);
    if (answerResult.Question != null) 
      AddNewQuestionToSession(answerResult.Question);

    return answerResult;
  }

  /// <summary>
  /// Get some help over the exercice
  /// </summary>
  /// <returns>A <cref>AnswerResult</cref> object with a <cref>HelpResult</cref> containing a sentence that should help the user without giving the answer.</returns>
  public AnswerResult Help()
  {
    var key = SessionData.AlreadyAsked.LastOrDefault();
    if (SessionData.Exercice == null || key == null)
      return new AnswerResult { CouldNotStart = true };

    return new AnswerResult { Help = Exercice.Help(key) };
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

  private void AddNewQuestionToSession(Question question)
  {
    if (SessionData.ExerciceStartTime == null)
      SessionData.ExerciceStartTime = DateTime.UtcNow;

    if (!SessionData.AlreadyAsked.Contains(question.Key))
      SessionData.AlreadyAsked = SessionData.AlreadyAsked.Add(question.Key);

    SessionData.QuestionAsked++;
  }

  private AnswerValidation ValidateAnswer()
  {
    var lastAsked = SessionData.AlreadyAsked.Last();

    var answer = SessionData.LastAnswer;
    if (string.IsNullOrEmpty(answer))
      answer = SessionData.Answer.ToString() ?? string.Empty;

    var answerValidation = Exercice.ValidateAnswer(lastAsked, answer);

    if (answerValidation.IsValid)
      SessionData.CorrectAnswers++;

    return answerValidation;
  }

  internal string? GetCorrectAnswer(string questionKey)
  {
    return Exercice.ValidateAnswer(questionKey, string.Empty).CorrectAnswer;
  }
}
