using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Exercises.LanguageExercices;
using DevoirsAlexa.Domain.Exercises.MathExercices;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevoirsAlexa.Tests.Presentation;

public class DependencyInjection
{
  [Fact]
  public void ShouldInstantiateProperExercice_WhenUsingDependencyInjection()
  {
    var sut = new Function();
    var factory = sut._serviceProvider.GetRequiredService<Func<HomeworkExercisesTypes, IExerciceQuestionsRunner>>();

    Assert.Throws<ArgumentException>(() => factory((HomeworkExercisesTypes)42));
    Assert.Throws<ArgumentException>(() => factory(HomeworkExercisesTypes.Unknown));

    Assert.IsType<AdditionsExercises>(factory(HomeworkExercisesTypes.Additions));
    Assert.IsType<MultiplicationsExercises>(factory(HomeworkExercisesTypes.Multiplications));
    Assert.IsType<SortExercises>(factory(HomeworkExercisesTypes.SortNumbers));
    Assert.IsType<LexicalSortExercises>(factory(HomeworkExercisesTypes.SortWords));
    Assert.IsType<SubstractionsExercises>(factory(HomeworkExercisesTypes.Substractions));
  }

  //[Fact]
  //public void toto()
  //{
  //  var sut = new Function();
  //  var runner = sut._serviceProvider.GetRequiredService<ExerciceRunner>();
  //  runner.

  //}
}
