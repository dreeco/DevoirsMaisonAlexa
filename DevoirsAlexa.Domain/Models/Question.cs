using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// Represent the question asked to the user
/// </summary>
/// <param name="Key">The key or identifier. Shortest version of the question, it should be unique and represent exactly what was asked</param>
/// <param name="Text">The text as it will be asked to the user</param>
/// <param name="Type">The type of answer expected</param>
/// <param name="Index">The index of the question. Allow the application to know if first question of the current session.</param>
public record Question(string Key, string Text, QuestionType Type, int Index);