using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Application.Handlers;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Infrastructure;
using DevoirsAlexa.Infrastructure.Models;
using System.Diagnostics.CodeAnalysis;
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

  [ExcludeFromCodeCoverage]
  private static async Task Main()
  {
    var handler = FunctionHandler;
    await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
        .Build()
        .RunAsync();
  }

  public const string StopIntent = "AMAZON.StopIntent";
  public const string HelpIntent = "AMAZON.HelpIntent";

  /// <summary>
  /// Handler called by the Alexa endpoints
  /// Parse the request
  /// Set the current session
  /// call the request router
  /// outputs the answer
  /// </summary>
  /// <param name="input">The SkillRequest object</param>
  /// <param name="context">The AWS lambda context</param>
  /// <returns></returns>
  public static async Task<SkillResponse> FunctionHandler(SkillRequest? input, ILambdaContext? context)
  {
    if (input?.Session == null || context?.Logger == null)
      return ResponseBuilder.Tell("Une erreur inattendue est survenue, merci de relancer la skill.");

    LogInputData(input, context);

    await Task.Delay(0);

    var homeworkSession = GetHomeworkSession(input);
    context.Logger.LogInformation($"Updated session: {System.Text.Json.JsonSerializer.Serialize(input.Session)}");

    homeworkSession.TryGetValue(nameof(homeworkSession.ExerciceStartTime), out var e);
    context.Logger.LogInformation($"Exercice start time: {homeworkSession.ExerciceStartTime}, {e}, started {(DateTime.UtcNow - homeworkSession.ExerciceStartTime)?.TotalSeconds} seconds ago");

    if (input.Request is SessionEndedRequest sessionEnded)
    {
      //Prevent false positive answers if user does not respond and previous answer is valid
      homeworkSession.LastAnswer = "_";
    }

    var intentRequest = input.Request as IntentRequest;

    var state = intentRequest?.Intent.Name switch
    {
      StopIntent => RequestType.Stop,
      HelpIntent => RequestType.Help,
      _ => RequestType.Normal // LaunchRequest or any custom Intent request
    };

    var response = BuildAnswer(homeworkSession, state);

    response.SessionAttributes = homeworkSession.ToDictionary();
    SetNextIntentExpected(homeworkSession, response, context.Logger);
    return response;
  }

  private static void LogInputData(SkillRequest input, ILambdaContext context)
  {
    context.Logger.LogInformation($"Skill received the following session: {System.Text.Json.JsonSerializer.Serialize(input.Session)}");

    string requestSerialized = input.Request switch
    {
      IntentRequest => System.Text.Json.JsonSerializer.Serialize(input.Request as IntentRequest),
      LaunchRequest => System.Text.Json.JsonSerializer.Serialize(input.Request as LaunchRequest),
      SessionEndedRequest => System.Text.Json.JsonSerializer.Serialize(input.Request as SessionEndedRequest),
      _ => System.Text.Json.JsonSerializer.Serialize(input.Request),
    };

    context.Logger.LogInformation($"Skill received the following Intent: {requestSerialized}");
  }

  private static void SetNextIntentExpected(IHomeworkSession session, SkillResponse r, ILambdaLogger logger)
  {
    var data = NextRequestRouting.GetNextExpectedIntent(session);

    r.Response.Directives.Add(new DialogElicitSlot(data.Slots[0])
    {
      UpdatedIntent = new Intent
      {
        Name = data.Name,
        Slots = data.Slots.ToDictionary(s => s, s => new Slot() { Name = s })
      }
    });

    logger.LogInformation($"Expected Next intent is : {data.Name}");
  }

  private static SkillResponse BuildAnswer(IHomeworkSession session, RequestType state)
  {
    var prompt = new SentenceBuilder();
    var reprompt = new SentenceBuilder();

    RequestsHandler.ExecuteRequest(prompt, reprompt, state, session);

    if (reprompt.IsEmpty())
      return ResponseBuilder.Tell(prompt.GetSpeech());
    else
      return ResponseBuilder.Ask(prompt.GetSpeech(), new Reprompt() { OutputSpeech = reprompt.GetSpeech() });
  }

  private static HomeworkSession GetHomeworkSession(SkillRequest input)
  {
    if (input.Request is not IntentRequest intentRequest)
      return new HomeworkSession(input.Session.Attributes);

    var intent = NextRequestRouting.GetIntent(intentRequest.Intent.Name);
    foreach (var slotName in intent?.Slots ?? [])
    {
      intentRequest.Intent.Slots.TryGetValue(slotName, out var slot);

      var value = slot?.SlotValue.Value;
      if (!string.IsNullOrEmpty(value))
        input.Session.Attributes[slotName] = value;
    }

    return new HomeworkSession(input.Session.Attributes);
  }
}

/// <summary>
/// This class is used to register the input event and return type for the FunctionHandler method with the System.Text.Json source generator.
/// There must be a JsonSerializable attribute for each type used as the input and return type or a runtime error will occur 
/// from the JSON serializer unable to find the serialization information for unknown types.
/// </summary>
[JsonSerializable(typeof(string))]
[ExcludeFromCodeCoverage]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
  // By using this partial class derived from JsonSerializerContext, we can generate reflection free JSON Serializer code at compile time
  // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
  // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}