using Xunit;
using DevoirsAlexa.Tests.HomeworkTests;
using Alexa.NET.Response;
using System.Collections.Specialized;
using Homework.Models;

namespace DevoirsAlexa.Tests;

public class FunctionTest : BaseFunctionTest
{
  [Theory]
  [InlineData("", "Quel est ton prénom ?")]
  [InlineData("FirstName=Lucie", "Quel âge as-tu ?")]
  [InlineData("FirstName=Lucie,Age=8", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData("FirstName=Lucie,Age=8,Exercice=Additions", "Sur combien de questions souhaites-tu t'entraîner ?")]
  public async Task ShouldAskForNextStep_WhenUsingTheSkill_GivenSpecificContext(string context, string expectedText)
  {
    SetContextData(context);
    // Invoke the lambda function and get status
    SkillResponse response = await WhenIUseTheFollowingIntent("");

    //is plain text
    PlainTextOutputSpeech? speech = ThenThereIsAPlainTextOutputSpeech(response);

    //Has given info
    ThenIHaveTheFollowingTextInTheResponse(expectedText, speech);
  }

  private static void ThenIHaveTheFollowingTextInTheResponse(string expectedText, PlainTextOutputSpeech speech)
  {
    Assert.Contains(expectedText, speech.Text, StringComparison.InvariantCultureIgnoreCase);
  }

  [Theory]
  [InlineData("", "SetFirstName", "FirstName=Lucie", "Quel âge as-tu ?")]
  [InlineData("FirstName=Lucie", "SetAge", "Age=1", "Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ?")]
  [InlineData("FirstName=Lucie,Age=8", "SetExercice", "Exercice=Additions", "OK ! Et sur combien de questions souhaites-tu t'entraîner ?")]
  [InlineData("FirstName=Lucie,Age=8,Exercice=Additions", "SetNbExercice", "NbExercice=5", null)]
  public async Task ShouldFillCustomData_WhenUsingTheSkill_GivenIntentAnswer(string context, string intent, string slots, string? expectedText)
  {
    SetContextData(context);
    // Invoke the lambda function and get status
    SkillResponse response = await WhenIUseTheFollowingIntent(intent, slots);

    //is plain text
    PlainTextOutputSpeech? speech = ThenThereIsAPlainTextOutputSpeech(response);

    //Has given info
    if (!string.IsNullOrEmpty(expectedText))
      ThenIHaveTheFollowingTextInTheResponse(expectedText, speech);
    ThenIHaveTheFollowingSessionAttribute(intent, slots.Split('=')[1], response);
  }

  private static void ThenIHaveTheFollowingSessionAttribute(string intent, string expectedValue, SkillResponse response)
  {
    Assert.Equal(expectedValue, response.SessionAttributes[Function.Intents[intent][0]].ToString());
  }

  private async Task<SkillResponse> WhenIUseTheFollowingIntent(string intent, string? slots = null)
  {
    BuildSkillRequest(intent, new HomeworkSession(slots));
    return await Function.FunctionHandler(_request, _context);
  }
}