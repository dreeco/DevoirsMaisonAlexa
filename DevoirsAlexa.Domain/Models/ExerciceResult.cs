namespace DevoirsAlexa.Domain.Models;

public class ExerciceResult
{
  public TimeSpan ElapsedTime { get; set; }
  public int CorrectAnswers   { get; set; }
  public int TotalQuestions { get; set; }
}
