using Homework.Enums;
using Homework.Models;

namespace Homework.HomeworkExercisesRunner
{
  [Exercice(HomeworkExercises.Additions)]
  public class AdditionsExercises : IExerciceQuestionsRunner
  {
    public Question NextQuestion(int age, IEnumerable<string> alreadyAsked)
    {
      var min = 0;
      var max = 1000;

      switch ( age) {
        case < 4:
          min = 1;
          max = 4;
          break;
        case < 8:
          min = 0;
          max = 10;
          break;
        case <= 10:
          min = 0;
          max = 30;
          break;
      }

      var random = new Random();
      int x;
      int y;
      string key;
      var n = 0;
      do
      {
        x = random.Next(min, max);
        y = random.Next(min, max);
        key = $"{x}+{y}";
      }
      while (n++ < 100 && alreadyAsked.Contains(key));
      
      return new Question(key, $"Combien font {x} plus {y} ?");
    }

    public AnswerValidation ValidateAnswer(string questionKey, string answer)
    {
      var parts = questionKey.Split('+');
      var resultNumber = parts.Sum(p => int.Parse(p));

      if (!int.TryParse(answer, out var answerNumber))
        return new AnswerValidation(false, resultNumber.ToString());

      return new AnswerValidation(resultNumber == answerNumber, string.Empty);
    }
  }


}
