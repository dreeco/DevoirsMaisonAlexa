using DevoirsAlexa.Domain.Enums;
using Xunit;

namespace DevoirsAlexa.Tests.Domain.Models;

public enum TestEnum {
  Unknown,
  [TextRepresentations("toto")]
  WithTextRepresentation,
  [TextRepresentations("tata")]
  OtherWithTextRepresentation,
  WithoutTextRepresentation
}

public class EnumTests
{
  [Theory]
  [InlineData(TestEnum.WithTextRepresentation, "toto")]
  [InlineData(TestEnum.OtherWithTextRepresentation, "tata")]
  [InlineData(null, "titi")]
  [InlineData(null, "")]

  [InlineData(TestEnum.WithTextRepresentation, nameof(TestEnum.WithTextRepresentation))]
  [InlineData(TestEnum.OtherWithTextRepresentation, nameof(TestEnum.OtherWithTextRepresentation))]
  [InlineData(TestEnum.WithoutTextRepresentation, nameof(TestEnum.WithoutTextRepresentation))]
  [InlineData(TestEnum.Unknown, nameof(TestEnum.Unknown))]

  public void ShouldReturnExpectedEnum_WhenGettingEnumByString_GivenMixOfTextRepresentation(TestEnum? expectedEnum, string s) {
    Assert.Equal(expectedEnum, EnumHelper.GetEnumFromTextRepresentations<TestEnum>(s));
  }
}
