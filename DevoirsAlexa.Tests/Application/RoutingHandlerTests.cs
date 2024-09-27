using Alexa.NET.Response;
using DevoirsAlexa.Application;
using DevoirsAlexa.Infrastructure;
using DevoirsAlexa.Infrastructure.Models;
using System.Text.RegularExpressions;
using Xunit;

namespace DevoirsAlexa.Tests.Application
{
  public class RoutingHandlerTests
  {

    [Theory]
    [InlineData(true, "", @"Au revoir !", null)]
    [InlineData(true, "FirstName=Adrien", @"Au revoir !", null)]
    [InlineData(true, "FirstName=Adrien,Level=CE1", @"Au revoir !", null)]
    [InlineData(true, "FirstName=Adrien,Level=CE1,Exercice=Multiplications", @"Au revoir !", null)]
    [InlineData(true, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercices=2", @"Au revoir !", null)]
    [InlineData(true, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercices=2,LastAnswer=2,QuestionAsked=1,AlreadyAsked=2*1", @"Au revoir !", null)]

    [InlineData(false, "", "Quel est ton prénom ?", "Je n'ai pas compris ton prénom, peux tu répéter ?")]
    [InlineData(false, "FirstName=Adrien", @", en quelle classe es tu ?", "Je n'ai pas compris ta classe, peux tu répéter ?")]
    [InlineData(false, "FirstName=Adrien,Level=CE1", @"Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ? Soustractions ?", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.")]
    [InlineData(false, "FirstName=Adrien,Level=CE1,Exercice=Multiplications", "OK ! Et sur combien de questions souhaites-tu t'entraîner ?", "Je n'ai pas compris combien de questions tu souhaites, peux tu répéter ?")]
    [InlineData(false, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2", @"Combien font", "Hmmmm. Je n'ai pas compris ta réponse. Peux tu répéter ? La question était : Combien font ")]
    [InlineData(false, "FirstName=Adrien,Level=CE1,Exercice=Multiplications,NbExercice=2,LastAnswer=2,AlreadyAsked=2*2;2*1,CorrectAnswers=1,QuestionAsked=2", @"Tu as 2 bonnes réponses sur 2 questions, en moins de 30 secondes", "Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions.", 30)]
    public void ShouldSetPromptAndReprompt_GivenSpecificContext(bool isStoppingSkill, string sessionString, string promptMatch, string? repromptMatch, int? startedXSecondsAgo = null) {
      var prompt = new SentenceBuilder();
      var reprompt = new SentenceBuilder();
      var session = new HomeworkSession(sessionString);
      if (startedXSecondsAgo != null)
        session.ExerciceStartTime = DateTime.UtcNow.Add(-TimeSpan.FromSeconds(startedXSecondsAgo.Value));

      RequestHandler.FillPromptAndReprompt(prompt, reprompt, isStoppingSkill, session);
      
      Assert.Contains(promptMatch, prompt.GetPromptAsText(), StringComparison.InvariantCultureIgnoreCase);

      if (repromptMatch == null)
        Assert.True(reprompt.IsEmpty());
      else
        Assert.Contains(repromptMatch, reprompt.GetPromptAsText(), StringComparison.InvariantCultureIgnoreCase);

    }

  }
}
