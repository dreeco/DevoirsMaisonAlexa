using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Helpers;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.Models;

namespace DevoirsAlexa.Domain.Exercises.LanguageExercices
{
  /// <summary>
  /// The exercice to get questions about word lexical comparisons
  /// <para>Will provide <see cref="Question">Question</see> such as key: chat&lt;chien Text: "Chat vient avant chien ?"</para>
  /// </summary>
  public class LexicalSortExercises : IExerciceQuestionsRunner
  {
    private const char After = '>';
    private static string AfterString => After.ToString();
    private const char Before = '<';
    private static string BeforeString => Before.ToString();

    /// <inheritdoc/>
    public HomeworkExercisesTypes Type => HomeworkExercisesTypes.SortWords;

    private IWordsRepository WordRepository { get; }

    #region internal ctor for tests
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    internal LexicalSortExercises() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    /// <summary>
    /// Return an instance of <see cref="IExerciceQuestionsRunner"/> capable of providing word lexical comparisons and checking answers
    /// </summary>
    public LexicalSortExercises(IWordsRepository wordRepository)
    {
      WordRepository = wordRepository;
    }

    /// <inheritdoc/>
    public HelpResult Help(string questionKey)
    {
      return new HelpResult("Tu dois indiquer par \"vrai\" ou \"faux\" si le mot " + FormatKeyToText(questionKey) + ".", GetQuestionText(questionKey), QuestionType.Boolean);
    }

    /// <inheritdoc/>
    public Question NextQuestion(Levels level, IEnumerable<string> alreadyAsked)
    {
      return new Question("", "", QuestionType.Boolean, alreadyAsked.Count() + 1);
    }

    /// <inheritdoc/>
    public AnswerValidation ValidateAnswer(string questionKey, string answer)
    {
      var operationChar = MathHelper.GetOperationChar(questionKey);
      var strs = questionKey.Split(operationChar);
      var answeredTrue = answer.ParseBooleanAnswer();
      bool? shouldHaveAnsweredTrue = operationChar switch
      {
        After => strs.First().CompareTo(strs.Last()) < 0,
        Before => strs.First().CompareTo(strs.Last()) > 0,
        _ => null
      };

      return new AnswerValidation(shouldHaveAnsweredTrue != null && answeredTrue == shouldHaveAnsweredTrue, $"{(shouldHaveAnsweredTrue == true ? "vrai" : "faux")}");
    }

    private static string GetQuestionText(string key)
    {
      return FormatKeyToText(key) + " ?";
    }

    private static string FormatKeyToText(string key)
    {
      return key.Replace(AfterString, " vient après le mot ").Replace(BeforeString, " vient avant le mot ") + " dans le dictionnaire";
    }

    private char GetOperationChar(string questionKey)
    {
      return questionKey.FirstOrDefault(c => new[] { Before, After }.Contains(c));
    }
  }
}
