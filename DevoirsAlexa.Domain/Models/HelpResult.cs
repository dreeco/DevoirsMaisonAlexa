using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Domain.Models;

public record HelpResult(string Text, string QuestionText, QuestionType QuestionType);
