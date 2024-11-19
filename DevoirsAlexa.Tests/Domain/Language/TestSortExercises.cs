using DevoirsAlexa.Domain;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises.LanguageExercices;
using DevoirsAlexa.Domain.Models.Entities;
using Moq;
using Xunit;

namespace DevoirsAlexa.Tests.Domain.Language;
public class TestLexicalSortExercises
{
  private Mock<IWordsRepository> WordRepository;
  private LexicalSortExercises _exercice;

  public TestLexicalSortExercises()
  {
    WordRepository = new();
    _exercice = new(WordRepository.Object);
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

    Word[][] sequence = [
      [new Word("chat", level), new Word("chien", level)],
      [new Word("mouette", level), new Word("cochon", level)],
      [new Word("toto", level), new Word("tata", level)]
    ];
    WordRepository.SetupSequence(wp => wp.GetWordsForComparison(level, 2))
      .Returns(sequence[0])
      .Returns(sequence[1])
      .Returns(sequence[2]);

    for (var i = 0; i < sequence.Length; i++)
    {
      var question = _exercice.NextQuestion(level, alreadyAsked);

      Assert.NotNull(question);
      Assert.Matches($"{sequence[i][0].Text}(<|>){sequence[i][1].Text}", question.Key);
      var isBefore = question.Key.Contains('<');
      var operationText = isBefore ? "avant" : "après";
      Assert.Matches($@"{sequence[i][0].Text} vient {operationText} le mot {sequence[i][1].Text} dans le dictionnaire ?", question.Text);

      var isanswerTrue = sequence[i][0].Text.CompareTo(sequence[i][1].Text) == (isBefore ? -1 : 1);
      var answer = isanswerTrue ? BooleanAnswer.True.ToString() : BooleanAnswer.False.ToString();
      var falseAnswer = isanswerTrue ? BooleanAnswer.False.ToString() : BooleanAnswer.True.ToString();
      Assert.True(_exercice.ValidateAnswer(question.Key, answer).IsValid, "The expected answer should be accepted");
      Assert.False(_exercice.ValidateAnswer(question.Key, falseAnswer).IsValid, "The wrong answer should be refused");
      Assert.False(_exercice.ValidateAnswer(question.Key, string.Empty).IsValid, "Empty answer should be refused");
      Assert.False(_exercice.ValidateAnswer(question.Key, "kdqjshdkjqsh").IsValid, "No sense answer should be refused");


      var help = _exercice.Help(question.Key);
      Assert.NotNull(help);
      Assert.Equal($"Tu dois indiquer par \"vrai\" ou \"faux\" si le mot {sequence[i][0].Text} vient {operationText} le mot {sequence[i][1].Text} dans le dictionnaire.", help.Text);
    }
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

    WordRepository.SetupSequence(wp => wp.GetWordsForComparison(Levels.CM2, 2))
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
