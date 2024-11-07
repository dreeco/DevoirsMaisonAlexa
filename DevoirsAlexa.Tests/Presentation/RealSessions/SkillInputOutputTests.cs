using DevoirsAlexa.Presentation;

using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Xunit;

namespace DevoirsAlexa.Tests.Presentation;

public class SkillInputOutputTests
{
  [Theory]
  [InlineData("StartSkill")]
  [InlineData("SetFirstName")]
  [InlineData("SetLevel")]
  [InlineData("SetExercice")]
  [InlineData("AnswerAddition", true)]
  [InlineData("ExerciceEnd", true)]
  [InlineData("SessionEndedRequest", true)]
  [InlineData("HelpFirstname")]
  [InlineData("HelpLevel")]
  [InlineData("HelpExercice")]
  [InlineData("HelpNbExercice")]
  [InlineData("HelpStartExerciceAddition", true)]
  public async Task ShouldProvideExpectedResponse_WhenCallingFunction_WithSpecificRequest(string fileName, bool hasUnpredictableElements = false)
  {
    var skillRequest = ReadRequestFile(fileName);

    var response = await Function.FunctionHandler(skillRequest, new TestLambdaContext());

    var responseObject = ReadResponseFile(fileName);

    if (hasUnpredictableElements)
      StripUnpredictableElementsFromBothObjects(response, responseObject);

    Assert.Equivalent(responseObject, response);
  }

  private static void StripUnpredictableElementsFromBothObjects(SkillResponse response, SkillResponse responseObject)
  {
    response.Response.OutputSpeech = null;
    if (response.Response.Reprompt != null)
      response.Response.Reprompt.OutputSpeech = null;
    response.SessionAttributes["AlreadyAsked"] = null;

    responseObject.Response.OutputSpeech = null;
    if (responseObject.Response.Reprompt != null)
      responseObject.Response.Reprompt.OutputSpeech = null;
    responseObject.SessionAttributes["AlreadyAsked"] = null;
  }

  private static SkillResponse ReadResponseFile(string fileName)
  {
    return ReadSkillJsonFile<SkillResponse>("Responses", fileName);
  }

  private static SkillRequest ReadRequestFile(string fileName)
  {
    return ReadSkillJsonFile<SkillRequest>("Requests", fileName);
  }

  private static T ReadSkillJsonFile<T>(string subFolder, string fileName)
  {
    var text = File.ReadAllText($"./Presentation/RealSessions/{subFolder}/{fileName}.json", System.Text.Encoding.UTF8);
    return JsonConvert.DeserializeObject<T>(text) ?? throw new Exception($"Impossible to deserialize {fileName}.json");

  }
}
