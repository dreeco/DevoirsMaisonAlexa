using DevoirsAlexa.Domain;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises.LanguageExercices;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Domain.Models.Entities;
using Moq;
using Xunit;

namespace DevoirsAlexa.Tests.Domain.Language;
public class TestLexicalSortExercises
{
  private Mock<IWordsRepository> _wordRepositoryMock;
  private LexicalSortExercises _exercice;
  private Word[][] _sequence;

  public TestLexicalSortExercises()
  {
    _wordRepositoryMock = new();
    _exercice = new(_wordRepositoryMock.Object);
    _sequence = Array.Empty<Word[]>();
  }

  [Theory]
  [InlineData(Levels.CP)]
  [InlineData(Levels.CE1)]
  [InlineData(Levels.CE2)]
  [InlineData(Levels.CM1)]
  [InlineData(Levels.CM2)]
  public void ShouldAskProperQuestionsAndAcceptOrRefuseAnswers_WhenUsingSortExercice(Levels level)
  {
    var alreadyAsked = new List<string>();

    GivenIHaveAListOfWordsForALevel(level);

    for (var i = 0; i < _sequence.Length; i++)
    {
      var question = WhenIGetTheNextQuestion(level, alreadyAsked);
      Assert.NotNull(question);

      var isBefore = question.Key.Contains('<') == true;
      var operationText = isBefore ? "avant" : "après";
      ThenIHaveAProperlyFormattedQuestion(i, question, isBefore, operationText);
      ThenOneAnswerIsValidAndTheOthersAreNot(i, question, isBefore);
      ThenICanGetHelpForTheQuestion(i, question, operationText);
    }
  }

  private void ThenICanGetHelpForTheQuestion(int i, Question? question, string operationText)
  {
    Assert.NotNull(question);
    var help = _exercice.Help(question.Key);
    Assert.NotNull(help);
    Assert.Equal($"Tu dois indiquer par \"vrai\" ou \"faux\" si le mot {_sequence[i][0].Text} vient {operationText} le mot {_sequence[i][1].Text} dans le dictionnaire.", help.Text);
  }

  private void ThenOneAnswerIsValidAndTheOthersAreNot(int i, Question question, bool isBefore)
  {
    var isanswerTrue = _sequence[i][0].Text.CompareTo(_sequence[i][1].Text) == (isBefore ? -1 : 1);
    var answer = isanswerTrue ? BooleanAnswer.True.ToString() : BooleanAnswer.False.ToString();
    var falseAnswer = isanswerTrue ? BooleanAnswer.False.ToString() : BooleanAnswer.True.ToString();
    Assert.True(_exercice.ValidateAnswer(question.Key, answer).IsValid, "The expected answer should be accepted");
    Assert.False(_exercice.ValidateAnswer(question.Key, falseAnswer).IsValid, "The wrong answer should be refused");
    Assert.False(_exercice.ValidateAnswer(question.Key, string.Empty).IsValid, "Empty answer should be refused");
    Assert.False(_exercice.ValidateAnswer(question.Key, "kdqjshdkjqsh").IsValid, "No sense answer should be refused");
  }

  private void ThenIHaveAProperlyFormattedQuestion(int i, Question question, bool isBefore, string operationText)
  {
    Assert.Matches($"{_sequence[i][0].Text}(<|>){_sequence[i][1].Text}", question.Key);
    Assert.Matches($@"{_sequence[i][0].Text} vient {operationText} le mot {_sequence[i][1].Text} dans le dictionnaire ?", question.Text);
  }

  private Question? WhenIGetTheNextQuestion(Levels level, List<string> alreadyAsked)
  {
    return _exercice.NextQuestion(level, alreadyAsked);
  }

  private void GivenIHaveAListOfWordsForALevel(Levels level)
  {
    _sequence = [
      [new Word("chat", level), new Word("chien", level)],
      [new Word("mouette", level), new Word("cochon", level)],
      [new Word("toto", level), new Word("tata", level)]
    ];
    _wordRepositoryMock.SetupSequence(wp => wp.GetWordsForComparison(level, 2))
      .Returns(_sequence[0])
      .Returns(_sequence[1])
      .Returns(_sequence[2]);
  }

  [Theory]
  [InlineData(Levels.CP)]
  [InlineData(Levels.CE1)]
  [InlineData(Levels.CE2)]
  [InlineData(Levels.CM1)]
  public void Shouldfail_WhenUsingSortExercice_givenNoDefinitionForClassLevel(Levels level)
  {
    var alreadyAsked = new List<string>();

    Word[][] sequence = [
      [new Word("chat", level), new Word("chien", level)],
      [new Word("mouette", level), new Word("cochon", level)],
      [new Word("toto", level), new Word("tata", level)]
    ];

    _wordRepositoryMock.SetupSequence(wp => wp.GetWordsForComparison(Levels.CM2, 2))
      .Returns(sequence[0])
      .Returns(sequence[1])
      .Returns(sequence[2]);

    var question = _exercice.NextQuestion(level, alreadyAsked);
    Assert.Null(question);
  }


  [Fact]
  public void ShouldReturnFalse_WhenValdatingAnswer_GivenWrongQuestion() {
    Assert.False(_exercice.ValidateAnswer("chat_chien", BooleanAnswer.True.ToString()).IsValid, "No answer could be accepted if question is wrong");
    Assert.False(_exercice.ValidateAnswer("chat_chien", BooleanAnswer.False.ToString()).IsValid, "No answer could be accepted if question is wrong");
  }
}
