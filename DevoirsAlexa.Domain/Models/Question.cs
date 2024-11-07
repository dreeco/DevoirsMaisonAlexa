using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Domain.Models;

/// <summary>
/// Represent the question asked to the user
/// </summary>
/// <param name="Key">A key to simplify the question. Should be unique per question and represent exactly wjat was asked</param>
/// <param name="Text">The text to output to the user to ask the question</param>
/// <param name="Type">The type of answer expected</param>
/// <param name="Index">The index of the question</param>
public record Question(string Key, string Text, QuestionType Type, int Index);