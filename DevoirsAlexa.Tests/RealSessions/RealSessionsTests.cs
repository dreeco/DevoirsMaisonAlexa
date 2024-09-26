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
  [InlineData("SetAge")]
  [InlineData("SetExercice")]
  [InlineData("AnswerAddition", true)]
  public async Task ShouldProvideExpectedResponse_WhenCallingFunction_WithSpecificRequest(string fileName, bool stripAnswer = false)
  {
    var skillRequest = ReadRequestFile(fileName);
    
    var response = await Function.FunctionHandler(skillRequest, new TestLambdaContext());
    
    var responseObject = ReadResponseFile(fileName);

    //Workaround randomness of questions
    if (stripAnswer)
    {
      response.Response.OutputSpeech = null;
      response.Response.Reprompt.OutputSpeech = null;
      response.SessionAttributes["AlreadyAsked"] = null;

      responseObject.Response.OutputSpeech = null;
      responseObject.Response.Reprompt.OutputSpeech = null;
      responseObject.SessionAttributes["AlreadyAsked"] = null;
    }

    Assert.Equivalent(responseObject, response);
  }

  private static SkillResponse ReadResponseFile(string fileName)
  {
    var text = File.ReadAllText($"./RealSessions/Responses/{fileName}.json", System.Text.Encoding.UTF8);
    return JsonConvert.DeserializeObject<SkillResponse>(text) ?? throw new Exception($"Impossible to deserialize {fileName}.json");
  }

  private static SkillRequest ReadRequestFile(string fileName)
  {
    var text = File.ReadAllText($"./RealSessions/Requests/{fileName}.json", System.Text.Encoding.UTF8);
    return JsonConvert.DeserializeObject<SkillRequest>(text) ?? throw new Exception($"Impossible to deserialize {fileName}.json");

  }
}
