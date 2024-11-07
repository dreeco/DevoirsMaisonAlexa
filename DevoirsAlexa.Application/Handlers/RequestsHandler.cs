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
public static class RequestsHandler
{
  /// <summary>
  /// Binds handlers to combinaison of HomeworkStep and RequestType
  /// </summary>
  private static readonly IDictionary<HomeworkStep, StepPromptsData> PromptsForSteps = new Dictionary<HomeworkStep, StepPromptsData>() {
    { HomeworkStep.GetFirstName, new StepPromptsData(AskForFirstName, HelpForFirstName, QuitSkill) },
    { HomeworkStep.GetLevel, new StepPromptsData(AskForClassLevel, HelpForClassLevel, QuitSkill) },
    { HomeworkStep.GetExercice, new StepPromptsData(AskForExerciceType, HelpForExerciceType, QuitSkill) },
    { HomeworkStep.GetNbExercice, new StepPromptsData(AskForNumberOfQuestions, HelpForNumberOfQuestions, QuitSkill) },
    { HomeworkStep.StartExercice, new StepPromptsData(NextQuestion, HelpQuestion, StopQuestion) },
  };


  /// <summary>
  /// Execute the request by getting the next sentence after the current state and user session data
  /// <para>Example: if the user has no sessoin data, we will start by asking its first name</para>
  /// </summary>
  /// <param name="prompt">A reference to the prompt that will be given to the user</param>
  /// <param name="reprompt">A reference to the reprompt, that will be given to the user if no answer is given</param>
  /// <param name="state">The current request state</param>
  /// <param name="session">The user session data</param>
  public static void ExecuteRequest(ISentenceBuilder prompt, ISentenceBuilder reprompt, RequestType state, IHomeworkSession session)
  {
    session.LastQuestionType = null;
    var nextStep = NextRequestRouting.GetNextStep(session);
    var promptsForStep = PromptsForSteps[nextStep];
    promptsForStep.Call(state, prompt, reprompt, session);
  }

  private static void QuitSkill(ISentenceBuilder prompt, IHomeworkSession session)
  {
    session.Clear();
    prompt.AppendInterjection("Au revoir !");
  }

  #region firstname
  private static void AskForFirstName(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText("Quel est ton prénom ?");
    reprompt.AppendSimpleText("Je n'ai pas compris ton prénom, peux tu répéter ?");
  }

  private static void HelpForFirstName(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText("Comment t'appelles tu ?");
    reprompt.AppendSimpleText("Je n'ai pas compris ton prénom, peux tu répéter ?");
  }

  #endregion

  #region nb questions
  private static void AskForNumberOfQuestions(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText("OK ! Et sur combien de questions souhaites-tu t'entraîner ?");
    reprompt.AppendSimpleText("Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?");
  }

  private static void HelpForNumberOfQuestions(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText("Je souhaite savoir combien de questions te poser sur cette session d'exercice.");
    reprompt.AppendSimpleText("Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?");
  }

  #endregion

  #region exercice

  private static void AskForExerciceType(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText("Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ? Soustractions ?");
    reprompt.AppendSimpleText("Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.");
  }

  private static void HelpForExerciceType(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText("Je souhaite savoir quel exercice tu souhaites faire. Dis moi : \"Additions\" si tu veux des calculs d'additions, je comprends également \"Multiplications\" et \"Soustractions\"");
    reprompt.AppendSimpleText("Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.");
  }


  #endregion

  #region level

  private static void AskForClassLevel(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    prompt.AppendSimpleText($"Bonjour {session.FirstName}, en quelle classe es tu ?");
    reprompt.AppendSimpleText("Je n'ai pas compris ta classe, peux tu répéter ?");
  }

  private static void HelpForClassLevel(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
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
  private static void NextQuestion(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    var runner = new ExerciceRunner(session);
    var result = runner.ValidateAnswerAndGetNext(false);
    GetPromptForQuestionResult(prompt, result);
    GetRepromptForQuestionResult(reprompt, result.Question?.Text ?? string.Empty);
    session.LastQuestionType = result.Question?.Type;
  }

  private static void HelpQuestion(ISentenceBuilder prompt, ISentenceBuilder reprompt, IHomeworkSession session)
  {
    var runner = new ExerciceRunner(session);
    var result = runner.Help();
    if (result.Help != null)
    {
      prompt.AppendSimpleText(result.Help.Text);
      prompt.AppendSimpleText(" La question était : ");
      prompt.AppendSimpleText(result.Help.QuestionText);
    }
    else
      prompt.AppendSimpleText("Impossible de t'aider pour cette question.");

    GetRepromptForQuestionResult(reprompt, result.Help?.QuestionText ?? string.Empty);
    session.LastQuestionType = result.Help?.QuestionType;
  }

  private static void StopQuestion(ISentenceBuilder prompt, IHomeworkSession session)
  {
    var runner = new ExerciceRunner(session);
    var result = runner.ValidateAnswerAndGetNext(true);

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
    } //else should not happen => log ?
  }

  #endregion


}
