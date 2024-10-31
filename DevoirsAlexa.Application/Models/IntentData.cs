using DevoirsAlexa.Application.Enums;

namespace DevoirsAlexa.Application.Models;

public record IntentData(string Name, string[] Slots, HomeworkStep RelatedStep = HomeworkStep.GetFirstName);
