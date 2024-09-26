using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Application;

public class IntentData
{
  public string Name { get; set; }
  public string[] Slots { get; set; } = [];
  public HomeworkStep RelatedStep { get; set; } = HomeworkStep.GetFirstName;
  public IntentData(string name)
  {
    Name = name;
  }
}