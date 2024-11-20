using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models.Entities;

namespace DevoirsAlexa.Domain;

/// <summary>
/// Interface representing what the domain needs to handle words in exercises
/// </summary>
public interface IWordsRepository
{
  /// <summary>
  /// Get n words for lexical comparisons
  /// </summary>
  /// <param name="level">the user level</param>
  /// <param name="nbWords">number of words to return</param>
  /// <returns></returns>
  IEnumerable<Word> GetWordsForComparison(Levels level, int nbWords);
}
