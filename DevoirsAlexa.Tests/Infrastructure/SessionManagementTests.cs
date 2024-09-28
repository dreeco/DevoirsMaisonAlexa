using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Infrastructure.Models;
using System.Collections.Immutable;
using Xunit;

namespace DevoirsAlexa.Tests.Infrastructure;

public class SessionPersistenceTests
{
  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void ShouldReturnDateChosen_WhenUsingSession(bool setNull)
  {
    var session = new HomeworkSession();
    DateTime? d = setNull ? null : DateTime.UtcNow;
    session.ExerciceStartTime = d;
    Assert.Equal(d, session.ExerciceStartTime);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void ShouldReturnFirstNameChosen_WhenUsingSession(bool setNull)
  {
    var session = new HomeworkSession();
    var d = setNull ? null : "Lucie";
    session.FirstName = d;
    Assert.Equal(setNull ? string.Empty : d, session.FirstName);
  }

  [Theory]
  [InlineData(null)]
  [InlineData(Levels.CP)]
  [InlineData(Levels.CE1)]
  [InlineData(Levels.CE2)]
  [InlineData(Levels.CM1)]
  [InlineData(Levels.CM2)]
  public void ShouldReturnLevelChosen_WhenUsingSession(Levels? level)
  {
    var session = new HomeworkSession();
    session.Level = level;
    Assert.Equal(level, session.Level);
  }


  [Theory]
  [InlineData(null)]
  [InlineData(HomeworkExercisesTypes.Additions)]
  [InlineData(HomeworkExercisesTypes.Dictation)]
  //[InlineData(HomeworkExercisesTypes.Divisions)]
  [InlineData(HomeworkExercisesTypes.Multiplications)]
  [InlineData(HomeworkExercisesTypes.Unknown)]
  [InlineData(HomeworkExercisesTypes.Substractions)]
  public void ShouldReturnExerciceChosen_WhenUsingSession(HomeworkExercisesTypes? exercice)
  {
    var session = new HomeworkSession();
    session.Exercice = exercice;
    Assert.Equal(exercice, session.Exercice);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void ShouldReturnNbExerciceChosen_WhenUsingSession(bool setNull)
  {
    var session = new HomeworkSession();
    int? d = setNull ? null : 4;
    session.NbExercice = d;
    Assert.Equal(d, session.NbExercice);
  }

  [Fact]
  public void ShouldReturnCorrectAnswersChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = 4;
    session.CorrectAnswers = d;
    Assert.Equal(d, session.CorrectAnswers);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void ShouldReturnLastAnswerChosen_WhenUsingSession(bool setNull)
  {
    var session = new HomeworkSession();
    var d = setNull ? null : "42";
    session.LastAnswer = d;
    Assert.Equal(setNull ? string.Empty : d, session.LastAnswer);
  }

  [Fact]
  public void ShouldReturnAlreadyAskedChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = new[] { "2+2", "42-2" }.ToImmutableArray();
    session.AlreadyAsked = d;
    Assert.Equivalent(d, session.AlreadyAsked);
  }
}
