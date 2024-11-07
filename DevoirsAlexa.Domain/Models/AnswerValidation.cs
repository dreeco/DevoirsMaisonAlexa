namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// Was the user answer correct
/// </summary>
/// <param name="IsValid">True if the answer is correct. False otherwise.</param>
/// <param name="CorrectAnswer">A text representing the proper answer.</param>
public record AnswerValidation(bool IsValid, string CorrectAnswer);
