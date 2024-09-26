using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.MathExercices;

[Exercice(HomeworkExercisesTypes.Additions)]
public class AdditionsExercises : BaseTableExercises, IExerciceQuestionsRunner
{
    public AdditionsExercises() : base('+', "plus") { }

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
                min = 0;
                max = 10;
                break;
            case <= 10:
                min = 0;
                max = 30;
                break;
        }

        return NextQuestion(min, max, alreadyAsked);
    }
}
