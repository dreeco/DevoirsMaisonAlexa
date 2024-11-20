using Alexa.NET.Response;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises.LanguageExercices;
using DevoirsAlexa.Domain.Exercises.MathExercices;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Infrastructure;
using System.Text.RegularExpressions;

namespace DevoirsAlexa.Tests
{
  public static class Extensions
  {
    public static string? GetAsText(this SentenceBuilder prompt)
    {
      var speech = prompt.GetSpeech();
      if (speech is SsmlOutputSpeech ssml)
        return Regex.Replace(ssml.Ssml, "<[^>]*>", string.Empty);
      else if (speech is PlainTextOutputSpeech plain)
        return plain.Text;
      else
        return null;
    }

    public static IExerciceQuestionsRunner GetRunner(this HomeworkExercisesTypes t)
    {
      return t switch
      {
        HomeworkExercisesTypes.Additions => new AdditionsExercises(),
        HomeworkExercisesTypes.Multiplications => new MultiplicationsExercises(),
        HomeworkExercisesTypes.Substractions => new SubstractionsExercises(),
        HomeworkExercisesTypes.SortWords => new LexicalSortExercises(new WordsRepository()),
        HomeworkExercisesTypes.SortNumbers => new SortExercises(),
        _ => throw new ArgumentException("Should not call with unknown value"),
      };
    }

  }
}
