using Alexa.NET.Response;
using DevoirsAlexa.Infrastructure;
using Xunit;

namespace DevoirsAlexa.Tests.Infrastructure;

public class SentenceBuilderTests
{
  [Fact]
  public void ShouldNotAuthorizeTagInSentence()
  {
    var sentenceBuilder = new SentenceBuilder();
    Assert.Throws<ArgumentException>(() => sentenceBuilder.AppendSimpleText("<speak>toto</speak>"));
  }
  [Fact]
  public void ShouldAddExpectedText()
  {
    var sentenceBuilder = new SentenceBuilder();
    sentenceBuilder.AppendInterjection("Hello!");
    sentenceBuilder.AppendSimpleText(" How are you doing? ");
    sentenceBuilder.AppendSpelling("ET");
    sentenceBuilder.AppendSimpleText(" is the best ");
    sentenceBuilder.AppendPossiblePlural("movie", 1);
    sentenceBuilder.AppendSimpleText(". There were a total of 3 ");
    sentenceBuilder.AppendPossiblePlural("movie", 3);
    sentenceBuilder.AppendSimpleText(".");

    var ssml = sentenceBuilder.GetSpeech() as SsmlOutputSpeech;
    Assert.NotNull(ssml);
    Assert.Equal("<speak><say-as interpret-as='interjection'>Hello!</say-as> How are you doing? <say-as interpret-as='spell-out'>ET</say-as> is the best movie. There were a total of 3 movies.</speak>", ssml.Ssml);
    Assert.Equal("Hello! How are you doing? ET is the best movie. There were a total of 3 movies.", sentenceBuilder.GetAsText());
  }
}
