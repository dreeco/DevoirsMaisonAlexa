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
}
