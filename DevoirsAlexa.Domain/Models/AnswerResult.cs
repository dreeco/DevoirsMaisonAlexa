namespace DevoirsAlexa.Domain.Models
{
  public class AnswerResult
  {
    public AnswerValidation? Validation { get; set; }
    public Question? Question { get; set; }
    public ExerciceResult? Exercice { get; set; }
  }
}
