using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Homework.Enums;
using Homework.HomeworkExercisesRunner;
using Homework.Models;
using Presentation;
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

  public static Dictionary<string, IntentData> Intents = new Dictionary<string, IntentData> {
    { "SetFirstName", new IntentData { Slots = [nameof(HomeworkSession.FirstName)], RelatedStep = HomeworkStep.GetFirstName } },
    { "SetAge", new IntentData { Slots = [nameof(HomeworkSession.Age)], RelatedStep = HomeworkStep.GetAge} } ,
    { "SetExercice", new IntentData { Slots = [nameof(HomeworkSession.Exercice)], RelatedStep = HomeworkStep.GetExercice}} ,
    { "SetNbExercice", new IntentData { Slots = [nameof(HomeworkSession.NbExercice)], RelatedStep = HomeworkStep.GetNbExercice}} ,
    { "SetAnswer", new IntentData { Slots = [nameof(HomeworkSession.LastAnswer)], RelatedStep = HomeworkStep.StartExercice}} ,
  };

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

    var intentName = FillRequestSessionWithNewValues(input);
    context.Logger.LogInformation($"Updated input: {System.Text.Json.JsonSerializer.Serialize(input)}");

    var homeworkSession = new HomeworkSession(input.Session?.Attributes);
    var homeworkRunner = new ExerciceRunner(homeworkSession);

    if (intentName == "AMAZON.StopIntent")
    {
      var sentenceBuilder = new SentenceBuilder();
      homeworkRunner.EndSession(sentenceBuilder, continueAfter: false);
      return ResponseBuilder.Tell(new SsmlOutputSpeech() { Ssml = sentenceBuilder.ToString() });
    }

    homeworkSession.TryGetValue(nameof(homeworkSession.ExerciceStartTime), out var e);
    context.Logger.LogInformation($"Exercice start time: {homeworkSession.ExerciceStartTime}, {e}, started {(DateTime.UtcNow - homeworkSession.ExerciceStartTime)?.TotalSeconds} seconds ago");

    var nextStep = homeworkRunner.GetNextStep();
    context.Logger.LogInformation($"Next step is : {nextStep}");

    var response = BuildAnswerFromCurrentStep(homeworkRunner, nextStep);
    response.SessionAttributes = homeworkSession;

    SetNextIntentExpected(homeworkRunner, response, context.Logger);
    return response;
  }

  private static void SetNextIntentExpected(ExerciceRunner homeworkRunner, SkillResponse r, ILambdaLogger logger)
  {
    var data = Intents.FirstOrDefault(i => i.Value.RelatedStep == homeworkRunner.GetNextStep());
    var nextIntentName = data.Key;
    if (!string.IsNullOrEmpty(nextIntentName))
    {
      r.Response.Directives.Add(new DialogElicitSlot(data.Value.Slots[0]) { UpdatedIntent = new Intent { Name = nextIntentName, Slots = data.Value.Slots.ToDictionary(s => s, s => new Slot() { Name = s }) } });
      logger.LogInformation($"Expected Next intent is : {nextIntentName}");
    }
  }

  private static SkillResponse BuildAnswerFromCurrentStep(ExerciceRunner runner, HomeworkStep nextStep)
  {
    switch (nextStep)
    {
      case HomeworkStep.GetFirstName:
        return ResponseBuilder.Ask("Quel est ton pr�nom ?", new Reprompt("Je n'ai pas compris ton pr�nom, peux tu r�p�ter ?"));
      case HomeworkStep.GetAge:
        return ResponseBuilder.Ask($"Bonjour {runner.FirstName}, quel �ge as-tu ?", new Reprompt("Je n'ai pas compris ton �ge, peux tu r�p�ter ?"));
      case HomeworkStep.GetExercice:
        return ResponseBuilder.Ask("Tr�s bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ? Soustractions ?", new Reprompt("Je n'ai pas compris le titre de cet exercice. Tu peux me demander : additions, multiplications ou soustractions."));
      case HomeworkStep.GetNbExercice:
        return ResponseBuilder.Ask("OK ! Et sur combien de questions souhaites-tu t'entra�ner ?", new Reprompt("Je n'ai pas compris combien de questions tu souhaites, peux tu r�p�ter ?"));
      case HomeworkStep.StartExercice:
        {
          var mainAnswerBuilder = new SentenceBuilder();
          var repromptBuilder = new SentenceBuilder();

          var question = runner.NextQuestion(mainAnswerBuilder);
          repromptBuilder.AppendInterjection("Hmmmm");
          repromptBuilder.AppendPause(TimeSpan.FromMilliseconds(500));
          repromptBuilder.AppendSimpleText("Je n'ai pas compris ta r�ponse.");
          repromptBuilder.AppendPause(TimeSpan.FromMilliseconds(500));
          repromptBuilder.AppendSimpleText($"Peux tu r�p�ter ? La question �tait : {question}");

          var reprompt = new Reprompt()
          {
            OutputSpeech = new SsmlOutputSpeech()
            {
              Ssml = repromptBuilder.ToString()
            }
          };

          return ResponseBuilder.Ask(new SsmlOutputSpeech() { Ssml = mainAnswerBuilder.ToString() }, reprompt);
        }
    }

    return ResponseBuilder.Tell("OK");
  }

  private static string? FillRequestSessionWithNewValues(SkillRequest input)
  {
    if (input.Request is not IntentRequest intentRequest)
      return null;

    var name = intentRequest.Intent.Name;
    foreach (var intent in Intents)
    {
      if (intent.Key == name)
      {
        foreach (var slotName in intent.Value.Slots)
        {
          intentRequest.Intent.Slots.TryGetValue(slotName, out var slot);

          var value = slot?.SlotValue.Value;
          if (!string.IsNullOrEmpty(value))
            input.Session.Attributes[slotName] = value;
        }
      }
    }

    return intentRequest.Intent.Name;
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