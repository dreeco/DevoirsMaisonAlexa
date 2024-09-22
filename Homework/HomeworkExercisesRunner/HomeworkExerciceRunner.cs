using Homework.Enums;
using Homework.Models;

namespace Homework.HomeworkExercisesRunner
{
  public class HomeworkExerciceRunner
  {
    private HomeworkSession SessionData { get; }
    private HomeworkExerciceDispatcher _dispatcher { get; }

    public string FirstName => SessionData?.FirstName ?? string.Empty;
    public int? Age => SessionData?.Age;

    public string? LastQuestionKey => SessionData?.AlreadyAsked.LastOrDefault();

    public HomeworkExerciceRunner(HomeworkSession sessionData)
    {
      SessionData = sessionData;
      _dispatcher = new HomeworkExerciceDispatcher();
    }

    public HomeworkStep GetNextStep()
    {
      if (string.IsNullOrWhiteSpace(SessionData.FirstName))
        return HomeworkStep.GetFirstName;
      else if (SessionData.Age == null)
        return HomeworkStep.GetAge;
      else if (SessionData.Exercice == null || SessionData.Exercice == HomeworkExercisesTypes.Unknown)
        return HomeworkStep.GetExercice;
      else if (SessionData.NbExercice == null || SessionData.NbExercice < 1 || SessionData.NbExercice > 20)
        return HomeworkStep.GetNbExercice;
      else
        return HomeworkStep.StartExercice;
    }

    public string NextQuestion()
    {
      if (SessionData.Age == null)
        throw new ArgumentNullException(nameof(SessionData.Age));
      var exercice = GetExerciceQuestionsRunner();

      string text = string.Empty;

      if (!string.IsNullOrEmpty(SessionData.AlreadyAsked.LastOrDefault()))
        text = ValidateAnswer(exercice, SessionData.LastAnswer);

      if (SessionData.QuestionAsked >= SessionData.NbExercice)
      {
        text += " Quel exercice souhaites-tu faire désormais ?";
        ResetSessionAfterExercice();
        return text;
      }

      var question = exercice.NextQuestion(SessionData.Age.Value, SessionData.AlreadyAsked);
      AddNewQuestionToSession(question);

      return text + " " + question.Text;
    }

    private IExerciceQuestionsRunner GetExerciceQuestionsRunner()
    {
      if (SessionData.Exercice == null || SessionData.Exercice == HomeworkExercisesTypes.Unknown)
        throw new ArgumentNullException(nameof(SessionData.Exercice));

      return _dispatcher.GetExerciceQuestionsRunner(SessionData.Exercice.Value) ?? throw new ArgumentNullException(nameof(SessionData.Exercice));
    }

    private void AddNewQuestionToSession(Question question)
    {
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
    }

    private string ValidateAnswer(IExerciceQuestionsRunner exercice, string? answer)
    {
      var lastAsked = SessionData.AlreadyAsked.Last();
      var answerValidation = exercice.ValidateAnswer(lastAsked, answer ?? string.Empty);

      if (answerValidation.IsValid)
        SessionData.CorrectAnswers++;

      var text = answerValidation.IsValid ? "Bien joué !" : $"Oh non ! La bonne réponse était {answerValidation.CorrectAnswer}.";
      return text;
    }

    public string GetCorrectAnswer(string questionKey)
    {
      return GetExerciceQuestionsRunner().ValidateAnswer(questionKey, string.Empty).CorrectAnswer;
    }
  }
}
