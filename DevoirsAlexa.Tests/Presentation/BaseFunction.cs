using Xunit;
using Amazon.Lambda.TestUtilities;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;

using DevoirsAlexa.Infrastructure.Models;

namespace DevoirsAlexa.Tests.Presentation;

public class BaseFunctionTest
{
    protected TestLambdaContext _context;
    protected SkillRequest _request;

    public BaseFunctionTest()
    {
        _context = new TestLambdaContext() { ClientContext = new TestClientContext() { Custom = new Dictionary<string, string>() } };
        _request = new SkillRequest();
        _request.Session = new Session();
        _request.Session.Attributes = new HomeworkSession();
    }

    protected static T ThenThereIsAnOutputSpeech<T>(SkillResponse response) where T : class, IOutputSpeech
    {
        var speech = response.Response.OutputSpeech as T;
        Assert.NotNull(speech);
        return speech;
    }

    protected void BuildSkillRequest(string intentName, HomeworkSession session)
    {
        _request.Request = new IntentRequest()
        {
            Intent = new Intent()
            {
                Name = intentName,
                Slots = session
              .Where(s => !string.IsNullOrEmpty(s.Key))
              .ToDictionary(
                  s => s.Key,
                  s => new Slot() { Value = s.Value.ToString(), SlotValue = new SlotValue() { Value = s.Value.ToString() } }
              )
            }
        };
    }

    protected void SetContextData(string context)
    {
        _request.Session.Attributes = HomeworkSession.CreateSessionFromCommaSeparatedKeyValues(context);
    }

}
