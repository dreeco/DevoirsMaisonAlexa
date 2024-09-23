namespace Homework.Models;

public class AnswerValidation
{
  public bool IsValid { get; set; }
  public string CorrectAnswer { get; }

  public AnswerValidation(bool isValid, string correctAnswer)
  {
    IsValid = isValid;
    CorrectAnswer = correctAnswer;
  }
}