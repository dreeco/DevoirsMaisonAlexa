namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// The summary of the exercice. How many good answers, how long did it take.
/// </summary>
/// <param name="ElapsedTime">Timespan elapsed since the first question was asked</param>
/// <param name="CorrectAnswers">Number of correct answers</param>
/// <param name="TotalQuestions">Total number of questions</param>
public record ExerciceResult(TimeSpan ElapsedTime, int CorrectAnswers, int TotalQuestions);
