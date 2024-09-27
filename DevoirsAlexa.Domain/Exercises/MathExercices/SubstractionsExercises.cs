using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

[Exercice(HomeworkExercisesTypes.Substractions)]
public class SubstractionsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
  public SubstractionsExercises() : base('-', "moins") { }

  public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
  {
    var min = 0;
    var max = 1000;

    switch (level)
    {
      case Levels.CP:
        min = 1;
        max = 4;
        break;
      case Levels.CE1:
        min = 0;
        max = 10;
        break;
      case Levels.CE2:
        min = 0;
        max = 30;
        break;
      case Levels.CM1:
        min = 0;
        max = 60;
        break;
      case Levels.CM2:
        min = 0;
        max = 100;
        break;
    }

    return NextQuestion(min, max, alreadyAsked);
  }
}
