using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Application.Models;
using DevoirsAlexa.Application.Text;
using DevoirsAlexa.Domain;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercisesRunner;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Application.Handlers;

/// <summary>
/// Handle a request.
/// <param>Match the current state to the next epected intent</param>
/// </summary>
public class RequestsHandler
{
  /// <summary>
  /// Binds handlers to combinaison of HomeworkStep and RequestType
  /// </summary>
  private readonly IDictionary<HomeworkStep, StepPromptsData> PromptsForSteps;

  private ExerciceRunner Runner { get; }
  private IHomeworkSession Session { get; }

  /// <summary>
  /// Instantiate a new request Handler.
  /// This handler route the intent to a prompt and eventually a reprompt
  /// </summary>
  /// <param name="runner">Dependency injected runner of the current exercice if any</param>
  /// <param name="session">Dependency injected current session state</param>
  public RequestsHandler(ExerciceRunner runner, IHomeworkSession session)
  {
    PromptsForSteps = new Dictionary<HomeworkStep, StepPromptsData>() {
      { HomeworkStep.GetFirstName, new StepPromptsData(AskForFirstName, HelpForFirstName, QuitSkill) },
      { HomeworkStep.GetLevel, new StepPromptsData(AskForClassLevel, HelpForClassLevel, QuitSkill) },
      { HomeworkStep.GetExercice, new StepPromptsData(AskForExerciceType, HelpForExerciceType, QuitSkill) },
      { HomeworkStep.GetNbExercice, new StepPromptsData(AskForNumberOfQuestions, HelpForNumberOfQuestions, QuitSkill) },
      { HomeworkStep.StartExercice, new StepPromptsData(NextQuestion, HelpQuestion, StopQuestion) },
    };

    Runner = runner;
    Session = session;
  }

  /// <summary>
  /// Execute the request by getting the next sentence after the current state and user session data
  /// <para>Example: if the user has no session data, we will start by asking its first name</para>
  /// </summary>
  /// <param name="prompt">A reference to the prompt that will be given to the user</param>
  /// <param name="reprompt">A reference to the reprompt, that will be given to the user if no answer is given</param>
  /// <param name="state">The current request state</param>
  public void ExecuteRequest(ISentenceBuilder prompt, ISentenceBuilder reprompt, RequestType state)
  {
    Session.LastQuestionType = null;
    var nextStep = NextRequestRouting.GetNextStep(Session);
    var promptsForStep = PromptsForSteps[nextStep];
    promptsForStep.Call(state, prompt, reprompt);
  }

  private void QuitSkill(ISentenceBuilder prompt)
  {
    Session.Clear();
    prompt.AppendInterjection("Au revoir !");
  }

  #region firstname
  private void AskForFirstName(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("Quel est ton prénom ?");
    reprompt.AppendSimpleText("Je n'ai pas compris ton prénom, peux tu répéter ?");
  }

  private void HelpForFirstName(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("Comment t'appelles tu ?");
    reprompt.AppendSimpleText("Je n'ai pas compris ton prénom, peux tu répéter ?");
  }

  #endregion

  #region nb questions
  private void AskForNumberOfQuestions(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("OK ! Et sur combien de questions souhaites-tu t'entraîner ?");
    reprompt.AppendSimpleText("Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?");
  }

  private void HelpForNumberOfQuestions(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("Je souhaite savoir combien de questions te poser sur cette session d'exercice.");
    reprompt.AppendSimpleText("Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?");
  }

  #endregion

  #region exercice

  private void AskForExerciceType(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ? Soustractions ?");
    reprompt.AppendSimpleText("Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.");
  }

  private void HelpForExerciceType(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("Je souhaite savoir quel exercice tu souhaites faire. Dis moi : \"Additions\" si tu veux des calculs d'additions, je comprends également \"Multiplications\" et \"Soustractions\"");
    reprompt.AppendSimpleText("Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.");
  }


  #endregion

  #region level

  private void AskForClassLevel(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText($"Bonjour {Session.FirstName}, en quelle classe es tu ?");
    reprompt.AppendSimpleText("Je n'ai pas compris ta classe, peux tu répéter ?");
  }

  private void HelpForClassLevel(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    prompt.AppendSimpleText("En quel niveau es-tu à l'école ? Je comprends les classes suivantes : ");
    var first = true;
    foreach (var e in Enum.GetNames(typeof(Levels)))
    {
      if (!first)
        prompt.AppendSimpleText(", ");

      prompt.AppendSpelling(e);
      first = false;
    }
    prompt.AppendSimpleText(". Essaye de prononcer distinctement chaque lettre.");
    reprompt.AppendSimpleText("Je n'ai pas compris ta classe, peux tu répéter ?");
  }

  #endregion

  #region start exercice
  private void NextQuestion(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    var result = Runner.ValidateAnswerAndGetNext(false);
    GetPromptForQuestionResult(prompt, result);

    GetRepromptForQuestionResult(reprompt, result.Question?.Text ?? string.Empty);

    Session.LastQuestionType = result.Question?.Type;
  }

  private void HelpQuestion(ISentenceBuilder prompt, ISentenceBuilder reprompt)
  {
    var result = Runner.Help();
    if (result.Help != null)
    {
      prompt.AppendSimpleText(result.Help.Text);
      prompt.AppendSimpleText(" La question était : ");
      prompt.AppendSimpleText(result.Help.QuestionText);
    }
    else
      prompt.AppendSimpleText("Impossible de t'aider pour cette question.");

    GetRepromptForQuestionResult(reprompt, result.Help?.QuestionText ?? string.Empty);
    Session.LastQuestionType = result.Help?.QuestionType;
  }

  private void StopQuestion(ISentenceBuilder prompt)
  {
    var result = Runner.ValidateAnswerAndGetNext(true);

    if (result.Exercice?.TotalQuestions > 0)
      result.Exercice.GetEndOfExerciceCompletionSentence(prompt);

    prompt.AppendInterjection("Au revoir !");
  }

  private static void GetRepromptForQuestionResult(ISentenceBuilder sentenceBuilder, string questionText)
  {
    sentenceBuilder.AppendInterjection("Hmmmm. ");
    sentenceBuilder.AppendPause(TimeSpan.FromMilliseconds(500));
    sentenceBuilder.AppendSimpleText("Je n'ai pas compris ta réponse. ");
    sentenceBuilder.AppendPause(TimeSpan.FromMilliseconds(500));
    sentenceBuilder.AppendSimpleText($"Peux tu répéter ? La question était : {questionText}");

    sentenceBuilder.AppendSimpleText("Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.");
  }
  private static void GetPromptForQuestionResult(ISentenceBuilder sentenceBuilder, AnswerResult result)
  {
    if (result.Validation != null)
    {
      result.Validation.AddAnswerValidationText(sentenceBuilder);
      sentenceBuilder.AppendSimpleText(" ");
    }

    if (result.Question != null) // Should ask new question
    {
      if (result.Question.Index == 1) // Is first Question
        sentenceBuilder.AppendInterjection("C'est parti ! ");

      sentenceBuilder.AppendSimpleText(result.Question.Text);
    }
    else if (result.Exercice?.TotalQuestions > 0) // Exercice is over
    {
      result.Exercice.GetEndOfExerciceCompletionSentence(sentenceBuilder);
      sentenceBuilder.AppendSimpleText(" Quel exercice souhaites-tu faire désormais ?");
    }
    else
    {
      sentenceBuilder.AppendSimpleText("Je n'ai pas réussi à trouver une nouvelle question à te poser. Tu peux relancer La skill si tu souhaite rejouer. ");
    }
  }

  #endregion
}
