using DevoirsAlexa.Application.Enums;
using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Application.Models;

/// <summary>
/// Represent binding data for each Intent configured on Alexa
/// </summary>
/// <param name="Name">The intent name</param>
/// <param name="Slots">The slots that can be filled by the user</param>
/// <param name="RelatedStep">The <cref>HomeworkStep</cref> related to the current state</param>
/// <param name="QuestionType">The expected answer type</param>
public record IntentData(string Name, string[] Slots, HomeworkStep RelatedStep = HomeworkStep.GetFirstName, QuestionType? QuestionType = null);
