using Homework.Enums;
using Homework.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Homework.HomeworkExercisesRunner
{
  public class HomeworkExerciceRunner
  {
    public string? FirstName { get; private set; }
    public int? Age { get; private set; }
    public HomeworkExercises? Exercice { get; private set; }
    public int? NbExercice { get; private set; }
    public HashSet<string> AlreadyAsked { get; private set; }
    public int QuestionAsked { get; private set; }
    public int CorrectAnswers { get; private set; }

    private HomeworkSession SessionData { get; }

    public HomeworkExerciceRunner(HomeworkSession sessionData)
    {
      if (sessionData.TryGetString(nameof(FirstName), out var firstName) && !string.IsNullOrWhiteSpace(firstName))
        FirstName = firstName;

      if (sessionData.TryGetString(nameof(Age), out var ageAsString) && int.TryParse(ageAsString, out var age))
        Age = age;

      if (sessionData.TryGetString(nameof(Exercice), out var exerciceAsString))
        Exercice = GetExercice(exerciceAsString);

      if (sessionData.TryGetString(nameof(NbExercice), out var nbExerciceAsString) && int.TryParse(nbExerciceAsString, out var nbExercice))
        NbExercice = nbExercice;

      if (sessionData.TryGetString(nameof(AlreadyAsked), out var alreadyAsked))
      {
        AlreadyAsked = alreadyAsked.Split(';').ToHashSet(); // split different exercices
      }
      else
        AlreadyAsked = new HashSet<string>();

      if (sessionData.TryGetString(nameof(QuestionAsked), out var questionAskedAsString) && int.TryParse(questionAskedAsString, out var questionAsked))
        QuestionAsked = questionAsked;
      else
        QuestionAsked = 0;


      if (sessionData.TryGetString(nameof(CorrectAnswers), out var CorrectAnswersAsString) && int.TryParse(CorrectAnswersAsString, out var correctAnswers))
        CorrectAnswers = correctAnswers;
      else
        CorrectAnswers = 0;

      SessionData = sessionData;
    }

    private HomeworkExercises? GetExercice(string exerciceAsString)
    {
      if (Enum.TryParse<HomeworkExercises>(exerciceAsString, ignoreCase: true, out var exercice))
        return exercice;


      foreach (var e in Enum.GetValues<HomeworkExercises>())
      {
        if (exerciceAsString.Contains(e.ToString(), StringComparison.InvariantCultureIgnoreCase))
          return e;

        var textRepresentations = e.GetType()?.GetField(e.ToString())?.GetCustomAttributes(typeof(TextRepresentationsAttribute), false).FirstOrDefault() as TextRepresentationsAttribute;
        foreach (var representation in textRepresentations?.StringValue ?? [])
        {
          if (exerciceAsString.Contains(representation, StringComparison.InvariantCultureIgnoreCase))
            return e;
        }
      }
      return null;
    }

    public HomeworkStep GetNextStep()
    {
      if (string.IsNullOrWhiteSpace(FirstName))
        return HomeworkStep.GetFirstName;
      else if (Age == null)
        return HomeworkStep.GetAge;
      else if (Exercice == null || Exercice == HomeworkExercises.Unknown)
        return HomeworkStep.GetExercice;
      else if (NbExercice == null || NbExercice < 1 || NbExercice > 20)
        return HomeworkStep.GetNbExercice;
      else
        return HomeworkStep.StartExercice;
    }

    public string NextQuestion(string answerPreviousQuestion)
    {
      string text = string.Empty;

      if (!string.IsNullOrEmpty(AlreadyAsked.LastOrDefault()))
        text = ValidateAnswer(answerPreviousQuestion);

      if (QuestionAsked >= NbExercice)
      {
        text += " Quel exercice souhaites-tu faire désormais ?";

        CorrectAnswers = 0;
        AlreadyAsked.Clear();
        QuestionAsked = 0;

        SessionData[nameof(QuestionAsked)] = QuestionAsked;
        SessionData[nameof(AlreadyAsked)] = AlreadyAsked;
        SessionData[nameof(CorrectAnswers)] = CorrectAnswers;


        Exercice = null;
        NbExercice = 0;
        SessionData[nameof(Exercice)] = string.Empty;
        SessionData[nameof(NbExercice)] = NbExercice;

        return text;
      }

      if (Age == null)
        throw new ArgumentNullException(nameof(Age));

      var exercice = new AdditionsExercises();
      var question = exercice.NextQuestion(Age.Value, AlreadyAsked);
      if (!AlreadyAsked.Contains(question.Key))
        AlreadyAsked.Add(question.Key);
      QuestionAsked++;

      //Persist data in session
      SessionData[nameof(QuestionAsked)] = QuestionAsked;
      SessionData[nameof(AlreadyAsked)] = AlreadyAsked;

      return text + " " + question.Text;
    }

    private string ValidateAnswer(string answer)
    {
      var exercice = new AdditionsExercises();
      var lastAsked = AlreadyAsked.Last();
      var answerValidation = exercice.ValidateAnswer(lastAsked, answer);

      //Persist in database
      if (answerValidation.IsValid)
      {
        CorrectAnswers++;
        SessionData[nameof(CorrectAnswers)] = CorrectAnswers;
      }

      var text = answerValidation.IsValid ? "Bien joué !" : $"Oh non ! La bonne réponse était {answerValidation.CorrectAnswer}.";
      return text;
    }

    public string GetCorrectAnswer(string questionKey)
    {
      var exercice = new AdditionsExercises();
      return exercice.ValidateAnswer(questionKey, string.Empty).CorrectAnswer;

    }
  }
}
