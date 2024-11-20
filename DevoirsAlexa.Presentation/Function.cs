using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Amazon.Lambda.Core;
using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Application.Handlers;
using DevoirsAlexa.Infrastructure;
using DevoirsAlexa.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DevoirsAlexa.Presentation;

/// <summary>
/// Holds the entry point for the Alexa Skill Handler
/// </summary>
public class Function
{
  /// <summary>
  /// The main entry point for the Lambda function. The main function is called once during the Lambda init phase. It
  /// initializes the .NET Lambda runtime client passing in the function handler to invoke for each Lambda event and
  /// the JSON serializer to use for converting Lambda JSON format to the .NET types. 
  /// </summary>
  /// <exclude/>

  //private static Function? function;

  //[ExcludeFromCodeCoverage]
  //private static async Task Main()
  //{
  //  //function ??= new Function();
  //  //await LambdaBootstrapBuilder.Create<SkillRequest>(async (skillRequest, lambdaContext) => { await function.FunctionHandler(skillRequest, lambdaContext); }, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
  //  //    .Build()
  //  //    .RunAsync();

  //  await LambdaBootstrapBuilder.Create(async (SkillRequest request, ILambdaContext context) =>
  //  {
  //    var f = new Function();
  //    await f.FunctionHandler(request, context);
  //  }, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
  //      .Build()
  //      .RunAsync();
  //}


  internal const string StopIntent = "AMAZON.StopIntent";
  internal const string HelpIntent = "AMAZON.HelpIntent";


  internal readonly IServiceProvider _serviceProvider;

  private HomeworkSession CurrentSession { get; }

  private readonly RequestsHandler RequestsHandler;


  /// <summary>
  /// Constructor called by the lambda, inject dependencies
  /// </summary>
  public Function() : this(new Startup().ConfigureServices()) { }

  internal Function(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;

    CurrentSession = serviceProvider.GetRequiredService<HomeworkSession>();
    RequestsHandler = serviceProvider.GetRequiredService<RequestsHandler>();
  }

  /// <summary>
  /// Handler called by the Alexa endpoints
  /// Parse the request
  /// Set the current session
  /// call the request router
  /// outputs the answer
  /// </summary>
  /// <param name="input">The <see href="https://github.com/timheuer/alexa-skills-dotnet?tab=readme-ov-file#request-types">SkillRequest</see> object</param>
  /// <param name="context">The AWS lambda context</param>
  /// <returns>The <see href="https://github.com/timheuer/alexa-skills-dotnet?tab=readme-ov-file#responses">Skill response</see></returns>
  public async Task<SkillResponse> FunctionHandler(SkillRequest? input, ILambdaContext? context)
  {
    if (input?.Session == null || context?.Logger == null)
      return ResponseBuilder.Tell("Une erreur inattendue est survenue, merci de relancer la skill.");

    LogInputData(input, context);

    await Task.Delay(0);

    FillHomeworkSession(input);

    context.Logger.LogInformation($"Updated session: {System.Text.Json.JsonSerializer.Serialize(input.Session)}");

    CurrentSession.TryGetValue(nameof(CurrentSession.ExerciceStartTime), out var e);
    context.Logger.LogInformation($"Exercice start time: {CurrentSession.ExerciceStartTime}, {e}, started {(DateTime.UtcNow - CurrentSession.ExerciceStartTime)?.TotalSeconds} seconds ago");

    if (input.Request is SessionEndedRequest sessionEnded)
    {
      //Prevent false positive answers if user does not respond and previous answer is valid
      CurrentSession.LastAnswer = "_";
    }

    var intentRequest = input.Request as IntentRequest;

    var state = intentRequest?.Intent.Name switch
    {
      StopIntent => RequestType.Stop,
      HelpIntent => RequestType.Help,
      _ => RequestType.Normal // LaunchRequest or any custom Intent request
    };

    var response = BuildAnswer(state);

    response.SessionAttributes = CurrentSession.ToDictionary();
    SetNextIntentExpected(response, context.Logger);
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

  private void SetNextIntentExpected(SkillResponse r, ILambdaLogger logger)
  {
    var data = NextRequestRouting.GetNextExpectedIntent(CurrentSession);

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

  private SkillResponse BuildAnswer(RequestType state)
  {
    var prompt = new SentenceBuilder();
    var reprompt = new SentenceBuilder();

    RequestsHandler.ExecuteRequest(prompt, reprompt, state);

    if (reprompt.IsEmpty())
      return ResponseBuilder.Tell(prompt.GetSpeech());
    else
      return ResponseBuilder.Ask(prompt.GetSpeech(), new Reprompt() { OutputSpeech = reprompt.GetSpeech() });
  }

  private void FillHomeworkSession(SkillRequest input)
  {
    if (input.Request is not IntentRequest intentRequest)
    {
      CurrentSession.FillFromSessionAttributes(input.Session.Attributes);
      return;
    }

    var intent = NextRequestRouting.GetIntent(intentRequest.Intent.Name);
    foreach (var slotName in intent?.Slots ?? [])
    {
      intentRequest.Intent.Slots.TryGetValue(slotName, out var slot);

      var value = slot?.SlotValue.Value;
      if (!string.IsNullOrEmpty(value))
        input.Session.Attributes[slotName] = value;
    }

    CurrentSession.FillFromSessionAttributes(input.Session.Attributes);
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