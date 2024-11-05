using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.Exercises.MathExercices
{
  public class SortExercises : IExerciceQuestionsRunner
  {
    public HomeworkExercisesTypes Type => HomeworkExercisesTypes.SortNumbers;
    public Dictionary<Levels, (int min, int max)> LevelsBoundaries { get; }
    public IDictionary<Levels, ExerciceRule[]> ExercisesRulesByLevel { get; }


    public SortExercises()
    {
      LevelsBoundaries = new Dictionary<Levels, (int min, int max)>() {
        { Levels.CP, (0, 50)},
        { Levels.CE1, (0, 100)},
        { Levels.CE2, (0, 1000)},
        { Levels.CM1, (0, 10000)},
        { Levels.CM2, (0, 100000)},
      };

      ExercisesRulesByLevel = LevelsBoundaries.ToDictionary(l => l.Key, l => GetRules(LevelsBoundaries[l.Key]));
    }

    private static ExerciceRule[] GetRules((int min, int max) bounds)
    {
      return [
        MathHelper.GetRuleForNoNumberUnder(bounds.min),
        MathHelper.GetRuleForNoNumberOver(bounds.max),
        MathHelper.GetRuleForDifferentNumbers()
        ];
    }

    public HelpResult Help(string questionKey)
    {
      return new HelpResult("Tu dois indiquer par \"vrai\" ou \"faux\" si le chiffre " + FormatKeyToText(questionKey) + ".", GetQuestionText(questionKey), QuestionType.Boolean);
    }

    private Question NextQuestion(Func<(int left, int right)> getNewNumbers, IEnumerable<ExerciceRule> rules, IEnumerable<string> alreadyAsked)
    {
      string key;
      var n = 0;
      var isValid = false;
      do
      {
        var numbers = getNewNumbers();
        var keySign = MathHelper.GetRandomBoolean() ? ">" : "<";
        key = $"{numbers.left}{keySign}{numbers.right}";
        isValid = n++ >= 1000 || (rules.All(r => r.IsValid(key)) && !alreadyAsked.Contains(key));
      }
      while (!isValid);

      return new Question(key, GetQuestionText(key), QuestionType.Boolean);
    }

    private static string GetQuestionText(string key)
    {
      return FormatKeyToText(key) + " ?";
    }

    private static string FormatKeyToText(string key)
    {
      return key.Replace(">", " est plus grand que ").Replace("<", " est plus petit que ");
    }

    public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
    {
      var bounds = LevelsBoundaries[level];
      var question = NextQuestion(() => MathHelper.GetRandomNumbersBothBetween(bounds.min, bounds.max), ExercisesRulesByLevel[level], alreadyAsked);
      if (!alreadyAsked.Any()) {
        question.Text = "Réponds par vrai ou faux. " + question.Text;
      }
      return question;
    }

    public AnswerValidation ValidateAnswer(string questionKey, string answer)
    {
      var operationChar = MathHelper.GetOperationChar(questionKey);
      var numbers = MathHelper.GetNumbersInQuestion(questionKey, operationChar);

      var answeredTrue = answer.ParseBooleanAnswer();
      bool? shouldHaveAnsweredTrue = operationChar switch
      {
        '>' => numbers.First() > numbers.Last(),
        '<' => numbers.First() < numbers.Last(),
        _ => null
      };

      return new AnswerValidation(shouldHaveAnsweredTrue != null && answeredTrue == shouldHaveAnsweredTrue, $"{(shouldHaveAnsweredTrue == true ? "vrai" : "faux")}");
    }
  }
}
