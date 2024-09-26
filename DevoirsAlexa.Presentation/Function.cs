using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using DevoirsAlexa.Application;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Infrastructure;
using DevoirsAlexa.Infrastructure.Models;
using System.Text.Json.Serialization;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DevoirsAlexa;

public class Function
{
  /// <summary>
  /// The main entry point for the Lambda function. The main function is called once during the Lambda init phase. It
  /// initializes the .NET Lambda runtime client passing in the function handler to invoke for each Lambda event and
  /// the JSON serializer to use for converting Lambda JSON format to the .NET types. 
  /// </summary>

  private static async Task Main()
  {
    var handler = FunctionHandler;
    await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
        .Build()
        .RunAsync();
  }

  public const string StopIntent = "AMAZON.StopIntent";


  /// <summary>
  /// A simple function that takes a string and does a ToUpper.
  ///
  /// To use this handler to respond to an AWS event, reference the appropriate package from 
  /// https://github.com/aws/aws-lambda-dotnet#events
  /// and change the string input parameter to the desired event type. When the event type
  /// is changed, the handler type registered in the main method needs to be updated and the LambdaFunctionJsonSerializerContext 
  /// defined below will need the JsonSerializable updated. If the return type and event type are different then the 
  /// LambdaFunctionJsonSerializerContext must have two JsonSerializable attributes, one for each type.
  ///
  // When using Native AOT extra testing with the deployed Lambda functions is required to ensure
  // the libraries used in the Lambda function work correctly with Native AOT. If a runtime 
  // error occurs about missing types or methods the most likely solution will be to remove references to trim-unsafe 
  // code or configure trimming options. This sample defaults to partial TrimMode because currently the AWS 
  // SDK for .NET does not support trimming. This will result in a larger executable size, and still does not 
  // guarantee runtime trimming errors won't be hit. 
  /// </summary>
  /// <param name="input">The event for the Lambda function handler to process.</param>
  /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
  /// <returns></returns>
  public static async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
  {
    context.Logger.LogInformation($"Skill received the following input: {System.Text.Json.JsonSerializer.Serialize(input)}");

    if (input == null)
      return ResponseBuilder.Tell("Une erreur inattendue est survenue, merci de relancer la skill");

    await Task.Delay(0);

    var homeworkSession = GetHomeworkSession(input);    
    context.Logger.LogInformation($"Updated input: {System.Text.Json.JsonSerializer.Serialize(input)}");

    homeworkSession.TryGetValue(nameof(homeworkSession.ExerciceStartTime), out var e);
    context.Logger.LogInformation($"Exercice start time: {homeworkSession.ExerciceStartTime}, {e}, started {(DateTime.UtcNow - homeworkSession.ExerciceStartTime)?.TotalSeconds} seconds ago");


    if (input.Request is SessionEndedRequest sessionEnded)
    {
      if (sessionEnded.Reason == Reason.ExceededMaxReprompts)
        homeworkSession.LastAnswer = "_";
    }

    var response = BuildAnswer(homeworkSession, isStopping: (input.Request as IntentRequest)?.Intent?.Name == StopIntent);
    
    
    response.SessionAttributes = homeworkSession.ToDictionary();

    SetNextIntentExpected(homeworkSession, response, context.Logger);
    return response;
  }

  private static void SetNextIntentExpected(IHomeworkSession session, SkillResponse r, ILambdaLogger logger)
  {
    var data = RequestRouting.GetNextExpectedIntent(session);
    if (data != null)
    {
      r.Response.Directives.Add(new DialogElicitSlot(data.Slots[0]) { UpdatedIntent = new Intent { Name = data.Name, Slots = data.Slots.ToDictionary(s => s, s => new Slot() { Name = s }) } });
      logger.LogInformation($"Expected Next intent is : {data.Name}");
    }
  }

  private static SkillResponse BuildAnswer(IHomeworkSession session, bool isStopping)
  {
    var prompt = new SentenceBuilder();
    var reprompt = new SentenceBuilder();

    RequestHandler.FillPromptAndReprompt(prompt, reprompt, isStopping, session);

    if (reprompt.IsEmpty())
      return ResponseBuilder.Tell(prompt.GetSpeech());
    else
      return ResponseBuilder.Ask(prompt.GetSpeech(), new Reprompt() { OutputSpeech = reprompt.GetSpeech() });
  }

  private static HomeworkSession GetHomeworkSession(SkillRequest input)
  {
    if (input.Request is not IntentRequest intentRequest)
      return new HomeworkSession(input.Session?.Attributes);

    var intent = RequestRouting.GetIntent(intentRequest.Intent.Name);
    foreach (var slotName in intent?.Slots ?? [])
    {
      intentRequest.Intent.Slots.TryGetValue(slotName, out var slot);

      var value = slot?.SlotValue.Value;
      if (!string.IsNullOrEmpty(value))
        input.Session.Attributes[slotName] = value;
    }

    return new HomeworkSession(input.Session?.Attributes);
  }
}

/// <summary>
/// This class is used to register the input event and return type for the FunctionHandler method with the System.Text.Json source generator.
/// There must be a JsonSerializable attribute for each type used as the input and return type or a runtime error will occur 
/// from the JSON serializer unable to find the serialization information for unknown types.
/// </summary>
[JsonSerializable(typeof(string))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
  // By using this partial class derived from JsonSerializerContext, we can generate reflection free JSON Serializer code at compile time
  // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
  // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}