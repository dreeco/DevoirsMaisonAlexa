using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Xunit;

namespace DevoirsAlexa.Tests.RealSessions;

public class RealSessionsTests
{
  [Theory]
  [InlineData("StartSkill")]
  [InlineData("SetFirstName")]
  [InlineData("SetAge")]
  [InlineData("SetExercice")]
  public async Task ShouldProvideExpectedResponse_WhenCallingFunction_WithSpecificRequest(string fileName)
  {
    var requestFileContent = ReadRequestFile(fileName);
    var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(requestFileContent) ?? throw new Exception($"Impossible to deserialize {fileName}.json");
    
    var response = await Function.FunctionHandler(skillRequest, new TestLambdaContext());
    
    var responseFileContent = ReadResponseFile(fileName);
    Assert.Equivalent(JsonConvert.DeserializeObject<SkillResponse>(responseFileContent), response);
  }

  private static string ReadResponseFile(string fileName)
  {
    return File.ReadAllText($"./RealSessions/Responses/{fileName}.json", System.Text.Encoding.UTF8);
  }

  private static string ReadRequestFile(string fileName)
  {
    return File.ReadAllText($"./RealSessions/Requests/{fileName}.json", System.Text.Encoding.UTF8);
  }
}
