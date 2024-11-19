using DevoirsAlexa.Domain.Enums;
using Xunit;

namespace DevoirsAlexa.Tests.Domain;

public enum TestEnum
{
  Unknown,
  
  [TextRepresentations("toto")]
  Toto,
  
  [TextRepresentations("tata", "tutu")]
  TataOrTutu,
  
  Nothing
}

public enum TestEnum2
{
}

public class EnumTests
{
  [Theory]
  [InlineData(TestEnum.Toto, "toto")]
  [InlineData(TestEnum.Toto, "I want toto")]
  [InlineData(null, "to")]
  [InlineData(null, "ta")]
  [InlineData(TestEnum.TataOrTutu, "tata")]
  [InlineData(TestEnum.TataOrTutu, "I want tata or tutu")]
  [InlineData(TestEnum.TataOrTutu, "tutu")]
  [InlineData(null, "titi")]
  [InlineData(null, "")]

  [InlineData(TestEnum.Toto, nameof(TestEnum.Toto))]
  [InlineData(TestEnum.TataOrTutu, nameof(TestEnum.TataOrTutu))]
  [InlineData(TestEnum.Unknown, nameof(TestEnum.Unknown))]
  [InlineData(TestEnum.Nothing, nameof(TestEnum.Nothing))]

  [InlineData(TestEnum.TataOrTutu, "tataortutu")]
  public void ShouldReturnExpectedEnum_WhenGettingEnumByString_GivenMixOfTextRepresentation(TestEnum? expectedEnum, string s)
  {
    Assert.Equal(expectedEnum, EnumHelper.GetEnumFromTextRepresentations<TestEnum>(s));
  }

  [Theory]
  [InlineData(null, "")]
  public void ShouldReturnnull_WhenGettingEnumByString_GivenEmptyEnum(TestEnum2? expectedEnum, string s)
  {
    Assert.Equal(expectedEnum, EnumHelper.GetEnumFromTextRepresentations<TestEnum2>(s));
  }
}
