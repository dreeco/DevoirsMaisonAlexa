using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Application.Handlers;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Infrastructure;
using DevoirsAlexa.Infrastructure.Models;
using Xunit;

namespace DevoirsAlexa.Tests.Application
{
    public class RoutingHandlerTests
  {
    [Theory]
    [InlineData(RequestType.Stop, "", @"Au revoir !", null)]
    [InlineData(RequestType.Stop, "FirstName=Adrien", @"Au revoir !", null)]
    [InlineData(RequestType.Stop, "FirstName=Adrien,Level=CE1", @"Au revoir !", null)]
    [InlineData(RequestType.Stop, "FirstName=Adrien,Level=CE1,Exercice=Multiplications", @"Au revoir !", null)]
    [InlineData(RequestType.Stop, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercices=2", @"Au revoir !", null)]
    [InlineData(RequestType.Stop, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercices=2,LastAnswer=2,QuestionAsked=1,AlreadyAsked=2*1", @"Au revoir !", null)]

    [InlineData(RequestType.Normal, "", "Quel est ton prénom ?", "Je n'ai pas compris ton prénom, peux tu répéter ?")]
    [InlineData(RequestType.Normal, "FirstName=Adrien", @", en quelle classe es tu ?", "Je n'ai pas compris ta classe, peux tu répéter ?")]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1", @"Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ? Soustractions ?", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.")]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1,Exercice=Multiplications", "OK ! Et sur combien de questions souhaites-tu t'entraîner ?", "Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?")]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2", @"Combien font", "Hmmmm. Je n'ai pas compris ta réponse. Peux tu répéter ? La question était : Combien font ")]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2,LastAnswer=2,AlreadyAsked=2*2;2*1,CorrectAnswers=1,QuestionAsked=2", @"Tu as 2 bonnes réponses sur 2 questions, en moins de 30 secondes", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.", 30)]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2,LastAnswer=2,AlreadyAsked=2*2;2*1,CorrectAnswers=1,QuestionAsked=2", @"C'est une bonne réponse !", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.", 30)]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2,LastAnswer=3,AlreadyAsked=2*2;2*1,CorrectAnswers=1,QuestionAsked=2", @"La bonne réponse était 2", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.", 30)]
    [InlineData(RequestType.Normal, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2,LastAnswer=2,AlreadyAsked=2*2;2_1,CorrectAnswers=1,QuestionAsked=2", @"Ce n'est pas la bonne réponse", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.", 30)]

    [InlineData(RequestType.Help, "", "Comment t'appelles tu ?", "Je n'ai pas compris ton prénom, peux tu répéter ?")]
    [InlineData(RequestType.Help, "FirstName=Adrien", @"En quel niveau es-tu à l'école ? Je comprends les classes suivantes : CP, CE1, CE2, CM1, CM2. Essaye de prononcer distinctement chaque lettre.", "Je n'ai pas compris ta classe, peux tu répéter ?")]
    [InlineData(RequestType.Help, "FirstName=Adrien,Level=CE1", @"Je souhaite savoir quel exercice tu souhaites faire. Dis moi : ""Additions"" si tu veux des calculs d'additions, je comprends également ""Multiplications"" et ""Soustractions""", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.")]
    [InlineData(RequestType.Help, "FirstName=Adrien,Level=CE1,Exercice=Multiplications", "Je souhaite savoir combien de questions te poser sur cette session d'exercice.", "Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?")]
    [InlineData(RequestType.Help, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2,AlreadyAsked=2*2", @"La bonne réponse est entre ", "Hmmmm. Je n'ai pas compris ta réponse. Peux tu répéter ? La question était : Combien font ")]
    [InlineData(RequestType.Help, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2", @"Impossible de t'aider pour cette question.", "Hmmmm. Je n'ai pas compris ta réponse. Peux tu répéter ? La question était : ")]
    public void ShouldSetPromptAndReprompt_GivenSpecificContext(RequestType requestType, string sessionString, string promptMatch, string? repromptMatch, int? startedXSecondsAgo = null) {
      var prompt = new SentenceBuilder();
      var reprompt = new SentenceBuilder();
      var session = new HomeworkSession(sessionString);
      if (startedXSecondsAgo != null)
        session.ExerciceStartTime = DateTime.UtcNow.Add(-TimeSpan.FromSeconds(startedXSecondsAgo.Value));

      var requestsHandler = new RequestsHandler(new DevoirsAlexa.Domain.HomeworkExercisesRunner.ExerciceRunner(Extensions.GetRunner, session), session);
      requestsHandler.ExecuteRequest(prompt, reprompt, requestType);
      
      Assert.Contains(promptMatch, prompt.GetPromptAsText(), StringComparison.InvariantCultureIgnoreCase);

      if (repromptMatch == null)
        Assert.True(reprompt.IsEmpty());
      else
        Assert.Contains(repromptMatch, reprompt.GetPromptAsText(), StringComparison.InvariantCultureIgnoreCase);
    }

  }
}
