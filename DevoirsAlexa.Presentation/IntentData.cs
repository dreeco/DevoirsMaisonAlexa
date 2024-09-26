using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Presentation;

public class IntentData
{
  public string[] Slots { get; set; } = [];
  public HomeworkStep RelatedStep { get; set; } = HomeworkStep.GetFirstName;
}