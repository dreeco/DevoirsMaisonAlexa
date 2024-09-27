using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Infrastructure.Models;
using System.Collections.Immutable;
using Xunit;

namespace DevoirsAlexa.Tests.Infrastructure;

public class SessionPersistenceTests
{
  [Fact]
  public void ShouldReturnDateChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = DateTime.UtcNow;
    session.ExerciceStartTime = d;
    Assert.Equal(d, session.ExerciceStartTime);
  }
  [Fact]
  public void ShouldReturnFirstNameChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = "Lucie";
    session.FirstName = d;
    Assert.Equal(d, session.FirstName);
  }
  [Fact]
  public void ShouldReturnLevelChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = Levels.CP;
    session.Level = d;
    Assert.Equal(d, session.Level);
  }
  [Fact]
  public void ShouldReturnExerciceChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = HomeworkExercisesTypes.Additions;
    session.Exercice = d;
    Assert.Equal(d, session.Exercice);
  }

  [Fact]
  public void ShouldReturnNbExerciceChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = 4;
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

  [Fact]
  public void ShouldReturnLastAnswerChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = "42";
    session.LastAnswer = d;
    Assert.Equal(d, session.LastAnswer);
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
