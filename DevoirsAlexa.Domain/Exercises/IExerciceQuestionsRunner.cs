using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.HomeworkExercises;

/// <summary>
/// Represent the Question Runner during an exercice session
/// </summary>
public interface IExerciceQuestionsRunner
{
  /// <summary>
  /// Get a new question according to class level, ideally not already asked
  /// </summary>
  /// <param name="level">Get a question matching a class level expectations.</param>
  /// <param name="alreadyAsked">List of question keys already asked to avoid re asking the same question.</param>
  /// <returns>A question (key and text) to ask to the user.</returns>
  Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked);

  /// <summary>
  /// Validate if the answer is correct or not
  /// </summary>
  /// <param name="questionKey">Question asked</param>
  /// <param name="answer">Answer given</param>
  /// <returns></returns>
  AnswerValidation ValidateAnswer(string questionKey, string answer);

  /// <summary>
  /// Get help for the current question
  /// </summary>
  /// <param name="questionKey">The question asked</param>
  /// <returns>A object containing a sentence designed to help the user (if he asked for it) to asnwer the question.</returns>
  HelpResult Help(string questionKey);

  /// <summary>
  /// The type of the exercice to be matched while searching for an exercice runner
  /// </summary>
  HomeworkExercisesTypes Type { get; }
}
