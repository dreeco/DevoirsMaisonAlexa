using Xunit;
using DevoirsAlexa.Tests.HomeworkTests;
using Alexa.NET.Response;
using System.Collections.Specialized;
using Homework.Models;
using Alexa.NET.Request;
using Alexa.NET.Response.Directive;

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
  [InlineData("", "SetFirstName", "FirstName=Lucie", "Quel âge as-tu ?", "SetAge")]
  [InlineData("FirstName=Lucie", "SetAge", "Age=1", "Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ?", "SetExercice")]
  [InlineData("FirstName=Lucie,Age=8", "SetExercice", "Exercice=Additions", "OK ! Et sur combien de questions souhaites-tu t'entraîner ?", "SetNbExercice")]
  [InlineData("FirstName=Lucie,Age=8,Exercice=Additions", "SetNbExercice", "NbExercice=5", null, "SetAnswer")]
  public async Task ShouldFillCustomData_WhenUsingTheSkill_GivenIntentAnswer(string context, string intent, string slots, string? expectedText, string expectedNextIntent)
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
    ThenIHaveTheFollowingNextIntent(response, expectedNextIntent);
  }

  private void ThenIHaveTheFollowingNextIntent(SkillResponse response, string expectedNextIntent)
  {
    Assert.NotEmpty(response.Response.Directives ?? []);
    var dialogDelegateIntents = response.Response.Directives?.Where(d => d is DialogElicitSlot dial).Select(d => d as DialogElicitSlot);
    Assert.NotEmpty(dialogDelegateIntents ?? []);

    Assert.True(dialogDelegateIntents?.FirstOrDefault(d => d?.UpdatedIntent?.Name == expectedNextIntent) != null, $"Next intents where found but none with the name {expectedNextIntent}. Found intents : [{string.Join(", ", dialogDelegateIntents?.Select(d => d?.UpdatedIntent?.Name) ?? [])}]");

    Assert.False(response.Response.ShouldEndSession);
  }

  private static void ThenIHaveTheFollowingSessionAttribute(string intent, string expectedValue, SkillResponse response)
  {
    Assert.Equal(expectedValue, response.SessionAttributes[Function.Intents[intent].Slots[0]].ToString());
  }

  private async Task<SkillResponse> WhenIUseTheFollowingIntent(string intent, string? slots = null)
  {
    BuildSkillRequest(intent, new HomeworkSession(slots));
    return await Function.FunctionHandler(_request, _context);
  }
}