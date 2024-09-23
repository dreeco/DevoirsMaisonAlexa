using Homework.Models;

namespace Homework.HomeworkExercises;

public interface IExerciceQuestionsRunner
{
    Question NextQuestion(int age, IEnumerable<string> alreadyAsked);
    AnswerValidation ValidateAnswer(string questionKey, string answer);
}
