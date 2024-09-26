using DevoirsAlexa.Infrastructure.Models;
using Xunit;

namespace DevoirsAlexa.Tests.SessionTests;

public class SessionManagementTests
{
  [Fact]
  public void ShouldReturnDateChosen_WhenUsingSession()
  {
    var session = new HomeworkSession();
    var d = DateTime.UtcNow;
    session.ExerciceStartTime = d;
    Assert.Equal(d, session.ExerciceStartTime);
  }
}
