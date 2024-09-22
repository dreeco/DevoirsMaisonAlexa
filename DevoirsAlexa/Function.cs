using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Homework;
using Homework.Enums;
using Homework.HomeworkExercisesRunner;
using Homework.Models;
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

  public static Dictionary<String, String[]> Intents = new Dictionary<string, string[]> {
    { "SetFirstName", ["FirstName"] },
    { "SetAge", ["Age"] } ,
    { "SetExercice", ["Exercice"] } ,
    { "SetNbExercice", ["NbExercice"] } ,
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

    FillRequestSessionWithNewValues(input);

    context.Logger.LogInformation($"Updated input: {System.Text.Json.JsonSerializer.Serialize(input)}");

    var homeworkSession = new HomeworkSession(input.Session?.Attributes);
    var homeworkRunner = new HomeworkExerciceRunner(homeworkSession);
    var nextStep = homeworkRunner.GetNextStep();

    context.Logger.LogInformation($"Next step is : {nextStep}");

    var r = BuildAnswerFromCurrentStep(homeworkRunner, nextStep);
    r.SessionAttributes = homeworkSession;
    return r;
  }

  private static SkillResponse BuildAnswerFromCurrentStep(HomeworkExerciceRunner runner, HomeworkStep nextStep)
  {
    switch (nextStep)
    {
      case HomeworkStep.GetFirstName:
        return ResponseBuilder.Ask("Quel est ton prénom ?", new Reprompt("Je n'ai pas compris ce prénom, peux tu répéter ?"));
      case HomeworkStep.GetAge:
        return ResponseBuilder.Ask($"Bonjour {runner.FirstName}, quel âge as-tu ?", new Reprompt("Je n'ai pas compris ce chiffre, peux tu répéter ?"));
      case HomeworkStep.GetExercice:
        return ResponseBuilder.Ask("Très bien ! Quel exercice souhaites-tu faire aujourd'hui ? Additions ? Multiplications ?", new Reprompt("Je n'ai pas compris cet exercice, Tu peux me demander additions ou multiplications."));
      case HomeworkStep.GetNbExercice:
        return ResponseBuilder.Ask("OK ! Et sur combien de questions souhaites-tu t'entraîner ?", new Reprompt("Je n'ai pas compris ce chiffre, peux tu répéter ?"));
      case HomeworkStep.StartExercice:
        return ResponseBuilder.Ask(runner.NextQuestion(string.Empty), new Reprompt("Je n'ai pas compris ce chiffre, peux tu répéter ?"));
    }

    return ResponseBuilder.Tell("OK");
  }

  private static void FillRequestSessionWithNewValues(SkillRequest input)
  {
    if (input.Request is not IntentRequest intentRequest)
      return;

    var name = intentRequest.Intent.Name;
    foreach (var intent in Intents)
    {
      if (intent.Key == name)
      {
        foreach (var slotName in intent.Value)
        {
          intentRequest.Intent.Slots.TryGetValue(slotName, out var slot);

          var value = slot?.SlotValue.Value;
          if (!string.IsNullOrEmpty(value))
            input.Session.Attributes[slotName] = value;
        }
      }
    }
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