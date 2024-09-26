using Xunit;
using DevoirsAlexa.Tests.HomeworkTests;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using DevoirsAlexa.Infrastructure.Models;
using DevoirsAlexa.Application;

namespace DevoirsAlexa.Tests.Presentation;

public class FunctionTest : BaseFunctionTest
{
  [Theory]
  [InlineData("", "Quel est ton prénom ?")]
  [InlineData("FirstName=Lucie", "Quel âge as-tu ?")]
  [InlineData("FirstName=Lucie,Age=8", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData("FirstName=Lucie,Age=8,Exercice=Additions", "Sur combien de questions souhaites-tu t'entraîner ?")]
  public async Task ShouldAskNextQuestion_WhenUsingTheSkill_GivenSpecificContext(string context, string expectedText)
  {
    SetContextData(context);
    // Invoke the lambda function and get status
    SkillResponse response = await WhenIUseTheFollowingIntent("");

    //is plain text
    PlainTextOutputSpeech? speech = ThenThereIsAPlainTextOutputSpeech(response);

    //Has given info
    ThenIHaveThisResponseText(expectedText, speech);
  }


  [Theory]
  [InlineData("", "", "", "Quel est ton prénom ?", "SetFirstName")]
  [InlineData("", "SetFirstName", "FirstName=Lucie", "Quel âge as-tu ?", "SetAge")]
  [InlineData("FirstName=Lucie", "SetAge", "Age=1", "Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ?", "SetExercice")]
  [InlineData("FirstName=Lucie,Age=8", "SetExercice", "Exercice=Additions", "OK ! Et sur combien de questions souhaites-tu t'entraîner ?", "SetNbExercice")]
  [InlineData("FirstName=Lucie,Age=8,Exercice=Additions", "SetNbExercice", "NbExercice=5", null, "SetAnswer")]
  [InlineData("FirstName=Lucie,Age=8,Exercice=Additions,NbExercice=3", "SetAnswer", "LastAnswer=*", null, "SetAnswer")]
  public async Task ShouldFillCustomData_WhenUsingTheSkill_GivenIntentAnswer(string context, string intent, string slots, string? expectedText, string expectedNextIntent)
  {
    SetContextData(context);
    // Invoke the lambda function and get status
    SkillResponse response = await WhenIUseTheFollowingIntent(intent, slots);

    //Has given info
    if (!string.IsNullOrEmpty(expectedText))
    {      //is plain text
      PlainTextOutputSpeech? speech = ThenThereIsAPlainTextOutputSpeech(response);
      ThenIHaveThisResponseText(expectedText, speech);
    }
    ThenIHaveANewSessionAttributeWithTheSlotValue(intent, slots, response);
    ThenIReturnTheNextExpectedIntentInTheResponse(response, expectedNextIntent);
  }

  private void ThenIReturnTheNextExpectedIntentInTheResponse(SkillResponse response, string expectedNextIntent)
  {
    Assert.NotEmpty(response.Response.Directives ?? []);
    var dialogDelegateIntents = response.Response.Directives?.Where(d => d is DialogElicitSlot dial).Select(d => d as DialogElicitSlot);
    Assert.NotEmpty(dialogDelegateIntents ?? []);

    Assert.True(dialogDelegateIntents?.FirstOrDefault(d => d?.UpdatedIntent?.Name == expectedNextIntent) != null, $"Next intents where found but none with the name {expectedNextIntent}. Found intents : [{string.Join(", ", dialogDelegateIntents?.Select(d => d?.UpdatedIntent?.Name) ?? [])}]");

    Assert.False(response.Response.ShouldEndSession);
  }

  private static void ThenIHaveANewSessionAttributeWithTheSlotValue(string intent, string slots, SkillResponse response)
  {
    if (string.IsNullOrEmpty(slots))
      return;

    var expectedValue = slots.Split('=')[1];
    var slotName = RequestRouting.GetIntent(intent)?.Slots[0] ?? throw new Exception($"Could not find slots for intent {intent}");
    var slotActualValue = response.SessionAttributes[slotName].ToString();
    if (expectedValue == "*")
      Assert.True(!string.IsNullOrEmpty(slotActualValue));
    else
      Assert.Equal(expectedValue, slotActualValue);
  }

  private async Task<SkillResponse> WhenIUseTheFollowingIntent(string intent, string? slots = null)
  {
    BuildSkillRequest(intent, new HomeworkSession(slots));
    return await Function.FunctionHandler(_request, _context);
  }

  private static void ThenIHaveThisResponseText(string expectedText, PlainTextOutputSpeech speech)
  {
    Assert.Contains(expectedText, speech.Text, StringComparison.InvariantCultureIgnoreCase);
  }
}