using Alexa.NET.Response;
using DevoirsAlexa.Infrastructure;
using System.Text.RegularExpressions;

namespace DevoirsAlexa.Tests
{
  public static class Extensions
  {
    public static string? GetPromptAsText(this SentenceBuilder prompt)
    {
      var speech = prompt.GetSpeech();
      if (speech is SsmlOutputSpeech ssml)
        return Regex.Replace(ssml.Ssml, "<[^>]*>", string.Empty);
      else if (speech is PlainTextOutputSpeech plain)
        return plain.Text;
      else
        return null;
    }
  }
}
