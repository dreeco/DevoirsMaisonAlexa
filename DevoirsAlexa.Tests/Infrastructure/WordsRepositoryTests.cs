using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Infrastructure;
using Xunit;

namespace DevoirsAlexa.Tests.Infrastructure;

public class WordsRepositoryTests
{
  [Theory]
  [InlineData(Levels.CP, 100)]
  [InlineData(Levels.CE1, 100)]
  [InlineData(Levels.CE2, 100)]
  [InlineData(Levels.CM1, 100)]
  [InlineData(Levels.CM2, 100)]
  [InlineData(null, 500)]
  public void ShouldHaveDistinctListOfWordsForEachLevel_WhenUsingWordRepository(Levels? level, int expected)
  {

    var dictionaryPart = GetWordsForLevel(level);

    var w = dictionaryPart.Where(w => dictionaryPart.Count(w2 => w2 == w) > 1);
    if (w.Any())
      throw new Exception($"{w.Distinct().Count()} duplicates words found in the {(level != null ? level : "All")} list : [{string.Join(", ", w.Distinct())}]");

    Assert.Equal(expected, dictionaryPart.Count());
  }

  private static IEnumerable<string> GetWordsForLevel(Levels? level)
  {
    return level switch
    {
      Levels.CP => WordsRepository.CPWords,
      Levels.CE1 => WordsRepository.CE1Words,
      Levels.CE2 => WordsRepository.CE2Words,
      Levels.CM1 => WordsRepository.CM1Words,
      Levels.CM2 => WordsRepository.CM2Words,
      _ => WordsRepository.AllWords.Select(x => x.Text)
    };
  }

  private static int GetNbTotalWordsForLevel(Levels level)
  {
    return level switch
    {
      Levels.CP => WordsRepository.CPWords.Length,
      Levels.CE1 => WordsRepository.CPWords.Length + WordsRepository.CE1Words.Length,
      Levels.CE2 => WordsRepository.CPWords.Length + WordsRepository.CE1Words.Length + WordsRepository.CE2Words.Length,
      Levels.CM1 => WordsRepository.CPWords.Length + WordsRepository.CE1Words.Length + WordsRepository.CE2Words.Length + WordsRepository.CM1Words.Length,
      Levels.CM2 => WordsRepository.CPWords.Length + WordsRepository.CE1Words.Length + WordsRepository.CE2Words.Length + WordsRepository.CM1Words.Length + WordsRepository.CM2Words.Length,
      _ => throw new NotImplementedException(),
    };
  }

  [Theory]
  [InlineData(Levels.CP)]
  [InlineData(Levels.CE1)]
  [InlineData(Levels.CE2)]
  [InlineData(Levels.CM1)]
  [InlineData(Levels.CM2)]
  public void ShouldBeAbleToGetAllWords_WhenUsingGetWordsForComparisons_GivenAllCount(Levels level)
  {
    var repository = new WordsRepository();

    var nbWords = GetNbTotalWordsForLevel(level);

    var words = repository.GetWordsForComparison(level, nbWords);
    Assert.Equal(nbWords, words.Count());
    Assert.Equal(nbWords, words.Distinct().Count());
  }

  [Theory]
  [InlineData(Levels.CP, 101)]
  [InlineData(Levels.CE1, 201)]
  [InlineData(Levels.CE2, 301)]
  [InlineData(Levels.CM1, 401)]
  [InlineData(Levels.CM2, 501)]
  public void ShouldReturnAnError_WhenUsingGetWordsForComparisons_GivenMoreThanCount(Levels level, int nbWords)
  {
    var repository = new WordsRepository();

    Assert.Throws<ArgumentOutOfRangeException>(() => repository.GetWordsForComparison(level, nbWords));
  }
}
