using DevoirsAlexa.Domain.Models;
using Xunit;

namespace DevoirsAlexa.Tests.Domain.Models;

public class QuestionTests
{
  [Theory(Skip = "Need to see what we do with records and string that we dont want empty")]
  [InlineData(true, true)]
  [InlineData(false, true)]
  [InlineData(true, false)]
  public void ShouldThrowAnError_WhenEmptyQuestionTextOrKey(bool emptyKey, bool emptyText) {
    Assert.Throws<ArgumentNullException>(() => new Question(emptyKey ? string.Empty : "2+2", emptyText ? string.Empty : "2 plus 2", DevoirsAlexa.Domain.Enums.QuestionType.Integer, 1));
  }
}
