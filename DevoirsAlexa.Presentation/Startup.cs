using DevoirsAlexa.Application.Handlers;
using DevoirsAlexa.Domain;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.HomeworkExercises;
using DevoirsAlexa.Domain.HomeworkExercisesRunner;
using DevoirsAlexa.Domain.Models;
using DevoirsAlexa.Infrastructure;
using DevoirsAlexa.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DevoirsAlexa.Presentation;

/// <summary>
/// Handles dependency injection (domain - infra)
/// </summary>
public class Startup
{
  /// <summary>
  /// Defines the mapping between interfaces and implementations for dependency injection
  /// <para>Create service definition for <see cref="IWordsRepository"/> infrastructure implementation</para>
  /// <para>Create service definition for all <see cref="IExerciceQuestionsRunner"/> implementations</para>
  /// <para>Creates a factory <see cref="HomeworkExercisesTypes"/> -> <see cref="IExerciceQuestionsRunner"/></para>
  /// </summary>
  /// <returns></returns>
  public IServiceProvider ConfigureServices()
  {
    var services = new ServiceCollection();


    RegisterInfrastructureImplementations(services);
    RegisterExercisesImplementations(services);
    RegisterApplicationImplementations(services);

    return services.BuildServiceProvider();
  }

  private static void RegisterApplicationImplementations(ServiceCollection services)
  {
    services.AddScoped<RequestsHandler>();
  }

  private static void RegisterExercisesImplementations(ServiceCollection services)
  {
    //Register each implementation
    var exercices = typeof(IExerciceQuestionsRunner).Assembly.GetTypes().Where(type => typeof(IExerciceQuestionsRunner).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);
    foreach (var exercise in exercices)
    {
      services.AddScoped(exercise);
    }

    //Create exercises factory
    services.AddScoped<Func<HomeworkExercisesTypes, IExerciceQuestionsRunner>>(serviceProvider => key =>
    {
      return exercices
      .Select(t => serviceProvider.GetRequiredService(t) as IExerciceQuestionsRunner)
      .FirstOrDefault(t => t?.Type == key) ?? throw new ArgumentException($"Could not find exercice matching type {key}");
    });

    //Register the exercises runner
    services.AddScoped<ExerciceRunner>();
  }

  private static void RegisterInfrastructureImplementations(ServiceCollection services)
  {
    services.AddScoped<HomeworkSession>();
    services.AddScoped<IHomeworkSession, HomeworkSession>(provider => provider.GetRequiredService<HomeworkSession>());
    services.AddScoped<IWordsRepository, WordsRepository>();
  }
}
