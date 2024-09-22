using Homework.Enums;

namespace DevoirsAlexa
{
  public class IntentData
  {
    public string[] Slots { get; set; } = [];
    public HomeworkStep RelatedStep { get; set; } = HomeworkStep.GetFirstName;
  }
}