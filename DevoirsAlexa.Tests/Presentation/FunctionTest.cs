using Xunit;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using DevoirsAlexa.Infrastructure.Models;
using Alexa.NET.Request.Type;
using DevoirsAlexa.Presentation;
using DevoirsAlexa.Application.Handlers;
using Alexa.NET.Request;
using DevoirsAlexa.Domain.Enums;
using static System.Reflection.Metadata.BlobBuilder;
using Xunit.Abstractions;

namespace DevoirsAlexa.Tests.Presentation;

public class FunctionTest : BaseFunctionTest
{
  [Theory]
  [InlineData("", "", "Quel est ton prénom ?")]
  [InlineData("", "FirstName=Lucie", "en quelle classe es tu ?")]
  [InlineData("", "FirstName=Lucie,Level=CE2", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData("", "FirstName=Lucie,Level=CE1,LastAnswer=20", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData("", "FirstName=Lucie,Level=CE2,Exercice=Additions", "Sur combien de questions souhaites-tu t'entraîner ?")]
  [InlineData("SetFirstName", "", "Quel est ton prénom ?")]
  [InlineData("SetFirstName", "FirstName=Lucie", "en quelle classe es tu ?")]
  [InlineData("SetFirstName", "FirstName=Lucie,Level=CE2", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData("SetFirstName", "FirstName=Lucie,Level=CE1,LastAnswer=20", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData("SetFirstName", "FirstName=Lucie,Level=CE2,Exercice=Additions", "Sur combien de questions souhaites-tu t'entraîner ?")]
  [InlineData(null, "", "Quel est ton prénom ?")]
  [InlineData(null, "FirstName=Lucie", "en quelle classe es tu ?")]
  [InlineData(null, "FirstName=Lucie,Level=CE2", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData(null, "FirstName=Lucie,Level=CE1,LastAnswer=20", "Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?")]
  [InlineData(null, "FirstName=Lucie,Level=CE2,Exercice=Additions", "Sur combien de questions souhaites-tu t'entraîner ?")]
  [InlineData(null, "FirstName=Lucie,Level=CE2,Exercice=Multiplications", "Sur combien de questions souhaites-tu t'entraîner ?")]
  [InlineData(null, "FirstName=Lucie,Level=CE2,Exercice=Substraction", "Sur combien de questions souhaites-tu t'entraîner ?")]
  [InlineData(null, "FirstName=Lucie,Level=CE2,Exercice=SortNumbers", "Sur combien de questions souhaites-tu t'entraîner ?")]
  public async Task ShouldAskNextQuestion_WhenUsingTheSkill_GivenSpecificContext(string? intent, string context, string expectedText)
  {
    SetContextData(context);
    // Invoke the lambda function and get status
    SkillResponse response;
    if (intent == null)
      response = await WhenIUseALaunchRequest();
    else
      response = await WhenIUseTheFollowingIntent(intent);

    //is plain text
    PlainTextOutputSpeech? speech = ThenThereIsAnOutputSpeech<PlainTextOutputSpeech>(response);

    //Has given info
    ThenIHaveThisResponseText(expectedText, speech);
  }


  [Theory]
  [InlineData("", "", "", "Quel est ton prénom ?", "SetFirstName")]
  [InlineData("", "SetFirstName", "FirstName=Lucie", "en quelle classe es tu ?", "SetLevel")]
  [InlineData("FirstName=Lucie", "SetLevel", "Level=CP", "Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ?", "SetExercice")]
  [InlineData("FirstName=Lucie,Level=CE1", "SetExercice", "Exercice=Additions", "OK ! Et sur combien de questions souhaites-tu t'entraîner ?", "SetNbExercice")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Additions", "SetNbExercice", "NbExercice=5", null, "SetAnswer")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Additions,NbExercice=3", "SetAnswer", "LastAnswer=*", null, "SetAnswer")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=SortNumbers", "SetNbExercice", "NbExercice=5", null, "SetBoolAnswer")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=SortNumbers,NbExercice=3", "SetBoolAnswer", "Answer=*", null, "SetBoolAnswer")]
  public async Task ShouldFillCustomData_WhenUsingTheSkill_GivenIntentAnswer(string context, string intent, string slots, string? expectedText, string expectedNextIntent)
  {
    SetContextData(context);
    // Invoke the lambda function and get status
    SkillResponse response = await WhenIUseTheFollowingIntent(intent, slots);

    //Has given info
    if (!string.IsNullOrEmpty(expectedText))
    {      //is plain text
      var speech = ThenThereIsAnOutputSpeech<PlainTextOutputSpeech>(response);
      ThenIHaveThisResponseText(expectedText, speech);
    }
    ThenIHaveANewSessionAttributeWithTheSlotValue(intent, slots, response);
    ThenIReturnTheNextExpectedIntentInTheResponse(response, expectedNextIntent);
  }

  [Theory]
  [InlineData("", "Au revoir !")]
  [InlineData("FirstName=Lucie", "Au revoir !")]
  [InlineData("FirstName=Lucie,Level=CE1", "Au revoir !")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Additions", "Au revoir !")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Additions,NbExercice=3", "Au revoir !")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Additions,NbExercice=3,AlreadyAsked=2+2;4+4,CorrectAnswers=1,QuestionAsked=1", "Tu as 1 bonne réponse sur 1 question", "Au revoir !")]
  public async Task ShouldEndSession_GivenStopIntent(string context, params string[] expectedTextParts)
  {
    SetContextData(context);
    SkillResponse response = await WhenIUseTheFollowingIntent(Function.StopIntent);

    Assert.True(response.Response.ShouldEndSession);
    var speech = ThenThereIsAnOutputSpeech<SsmlOutputSpeech>(response);
    foreach (var expectedText in expectedTextParts)
      ThenIHaveThisResponseText(expectedText, speech);

    Assert.Empty(response.SessionAttributes);
  }

  [Theory]
  [InlineData("", "Comment t'appelles tu ?")]
  [InlineData("FirstName=Lucie", "En quel niveau es-tu à l'école ? Je comprends les classes suivantes :")]
  [InlineData("FirstName=Lucie,Level=LevelThatDoesNotExists", "En quel niveau es-tu à l'école ? Je comprends les classes suivantes :")]
  [InlineData("FirstName=Lucie,Level=CE1", "Je souhaite savoir quel exercice tu souhaites faire.")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=ExerciceTHatDoesNotExists", "Je souhaite savoir quel exercice tu souhaites faire.")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Additions", "Je souhaite savoir combien de questions te poser sur cette session d'exercice.")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=Multiplications", "Je souhaite savoir combien de questions te poser sur cette session d'exercice.")]
  [InlineData("FirstName=Lucie,Level=CE1,Exercice=SortNumbers", "Je souhaite savoir combien de questions te poser sur cette session d'exercice.")]
  public async Task ShouldGiveHelp_GivenHelpIntent(string context, params string[] expectedTextParts)
  {
    var h = SetContextData(context);

    SkillResponse response = await WhenIUseTheFollowingIntent(Function.HelpIntent);

    Assert.False(response.Response.ShouldEndSession);
    foreach (var expectedText in expectedTextParts)
      ThenIHaveThisResponseText(expectedText, response.Response.OutputSpeech);

    AssertSessionAttributeIsNotErased(h.FirstName, nameof(h.FirstName), response);
    AssertSessionAttributeIsNotErased(h.Level?.ToString(), nameof(h.Level), response);
    AssertSessionAttributeIsNotErased(h.Exercice?.ToString(), nameof(h.Exercice), response);
    AssertSessionAttributeIsNotErased(h.NbExercice?.ToString(), nameof(h.NbExercice), response);
  }

  private static void AssertSessionAttributeIsNotErased(string? attr, string attrName, SkillResponse response)
  {
    if (!string.IsNullOrEmpty(attr))
      Assert.Equal(attr, response.SessionAttributes[attrName]?.ToString());
  }

  [Theory]
  [InlineData(true, true)]
  [InlineData(false, true)]
  [InlineData(true, false)]
  public async Task ShouldNotThrowErrorButAnswerGracefully_GivenWrongInput(bool nullInput, bool nullContext)
  {
    var response = await _sut.FunctionHandler(nullInput ? null : _request, nullContext ? null : _context);
    Assert.NotNull(response);
    var speech = ThenThereIsAnOutputSpeech<PlainTextOutputSpeech>(response);
    ThenIHaveThisResponseText("Une erreur inattendue est survenue, merci de relancer la skill.", speech);
  }

  [Fact]
  public async Task ShouldNotThrowErrorButAnswerGracefully_GivenWrongInputRequestType()
  {
    var response = await _sut.FunctionHandler(new SkillRequest { Request = new MyFakeRequest(), Session = new Session()}, _context);
    Assert.NotNull(response);
    var speech = ThenThereIsAnOutputSpeech<PlainTextOutputSpeech>(response);
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
    var slotName = NextRequestRouting.GetIntent(intent)?.Slots[0] ?? throw new Exception($"Could not find slots for intent {intent}");
    var slotActualValue = response.SessionAttributes[slotName].ToString();
    if (expectedValue == "*")
      Assert.True(!string.IsNullOrEmpty(slotActualValue));
    else
      Assert.Equal(expectedValue, slotActualValue);
  }

  private async Task<SkillResponse> WhenIUseTheFollowingIntent(string intent, string? slots = null)
  {
    BuildSkillRequestWithIntent(intent, new HomeworkSession(slots));
    return await _sut.FunctionHandler(_request, _context);
  }

  private async Task<SkillResponse> WhenIUseALaunchRequest(string? slots = null)
  {
    BuildSkillLaunchRequest();
    return await _sut.FunctionHandler(_request, _context);
  }

  private static void ThenIHaveThisResponseText(string expectedText, PlainTextOutputSpeech speech)
  {
    Assert.Contains(expectedText, speech.Text, StringComparison.InvariantCultureIgnoreCase);
  }
  private static void ThenIHaveThisResponseText(string expectedText, SsmlOutputSpeech speech)
  {
    Assert.Contains(expectedText, speech.Ssml, StringComparison.InvariantCultureIgnoreCase);
  }

  private static void ThenIHaveThisResponseText(string expectedText, IOutputSpeech speech)
  {
    if (speech is SsmlOutputSpeech ssml)
      ThenIHaveThisResponseText(expectedText, ssml);
    else if (speech is PlainTextOutputSpeech plain)
      ThenIHaveThisResponseText(expectedText, plain);
    else
      throw new Exception($"Unhandled output speech {speech}");
  }
}

internal class MyFakeRequest : Request { }