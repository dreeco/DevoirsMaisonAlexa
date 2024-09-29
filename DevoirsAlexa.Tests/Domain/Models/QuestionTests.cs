using DevoirsAlexa.Domain.Models;
using Xunit;

namespace DevoirsAlexa.Tests.Domain.Models;

public class QuestionTests
{
  [Theory]
  [InlineData(true, true)]
  [InlineData(false, true)]
  [InlineData(true, false)]
  public void toto(bool emptyKey, bool emptyText) {
    Assert.Throws<ArgumentNullException>(() => new Question(emptyKey ? string.Empty : "2+2", emptyText ? string.Empty : "2 plus 2"));
  }
}
