using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// If the user asked for help, here it is
/// </summary>
/// <param name="Text">A sentence designed to help the user</param>
/// <param name="QuestionText">The question that was asked as text</param>
/// <param name="QuestionType">The question key that was asked</param>
public record HelpResult(string Text, string QuestionText, QuestionType QuestionType);
