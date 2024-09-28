using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.HomeworkExercises;

public interface IExerciceQuestionsRunner
{
  Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked);
  AnswerValidation ValidateAnswer(string questionKey, string answer);

  IDictionary<Levels, ExerciceRule[]> ExercisesRulesByLevel { get; set; }
}
