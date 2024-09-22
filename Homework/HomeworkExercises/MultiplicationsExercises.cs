using Homework.Enums;
using Homework.HomeworkExercisesRunner;
using Homework.Models;

namespace Homework.HomeworkExercises;

[Exercice(HomeworkExercisesTypes.Multiplications)]
public class MultiplicationsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
  private const char operation = '*';
  private const string operationText = "multiplié par";

  public Question NextQuestion(int age, IEnumerable<string> alreadyAsked)
  {
    var min = 0;
    var max = 1000;

    switch (age)
    {
      case < 4:
        min = 1;
        max = 4;
        break;
      case < 8:
        min = 1;
        max = 10;
        break;
      case <= 10:
        min = 1;
        max = 30;
        break;
    }

    return NextQuestion(min, max, operation, operationText, alreadyAsked);
  }

  public AnswerValidation ValidateAnswer(string questionKey, string answer)
  {
    return ValidateAnswer(questionKey, answer, operation);
  }

}
