using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Application.Models;

public record IntentData(string Name, string[] Slots, HomeworkStep RelatedStep = HomeworkStep.GetFirstName, QuestionType? QuestionType = null);
