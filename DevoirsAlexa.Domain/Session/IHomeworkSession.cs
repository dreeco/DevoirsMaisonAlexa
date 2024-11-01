using DevoirsAlexa.Domain.Enums;
using System.Collections.Immutable;

namespace DevoirsAlexa.Domain.Models;

public interface IHomeworkSession : IDictionary<string, object>
{
  public string? FirstName { get; set; }
  public Levels? Level { get; set; }
  public int? NbExercice { get; set; }
  public int QuestionAsked { get; set; }
  public int CorrectAnswers { get; set; }
  public ImmutableArray<string> AlreadyAsked { get; set; }
  public HomeworkExercisesTypes? Exercice { get; set; }
  public string? LastAnswer { get; set; }//Integer answer
  public BooleanAnswer? Answer { get; set; }
  public DateTime? ExerciceStartTime { get; set; }

  public QuestionType? LastQuestionType { get; set; }
}
