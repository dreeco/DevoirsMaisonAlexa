using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.HomeworkExercises;

public interface IExerciceQuestionsRunner
{
    Question NextQuestion(int age, IEnumerable<string> alreadyAsked);
    AnswerValidation ValidateAnswer(string questionKey, string answer);
}
