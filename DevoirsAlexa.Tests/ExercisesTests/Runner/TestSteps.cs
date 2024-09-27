using Xunit;
using DevoirsAlexa.Application;
using DevoirsAlexa.Infrastructure.Models;
using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Tests.Application;

public class RoutingNextStepTests
{
    [Theory]
    [InlineData("", HomeworkStep.GetFirstName)]
    [InlineData("FirstName=Lucie", HomeworkStep.GetLevel)]
    [InlineData("FirstName=Lucie,Level=CE2", HomeworkStep.GetExercice)]

    [InlineData("FirstName=Lucie,Level=CE2,Exercice=Additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=Multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=Divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=Soustractions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=soustractions", HomeworkStep.GetNbExercice)]

    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table d' Additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table d' additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table de Multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table de multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table de Divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table de divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table de Soustractions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=table de soustractions", HomeworkStep.GetNbExercice)]


    [InlineData("FirstName=Lucie,Level=CE2,Exercice=Additions,NbExercice=10", HomeworkStep.StartExercice)]

    [InlineData("Level=CE2,Exercice=Additions,NbExercice=10", HomeworkStep.GetFirstName)]
    [InlineData("FirstName=Lucie,Exercice=Additions,NbExercice=10", HomeworkStep.GetLevel)]
    [InlineData("FirstName=Lucie,Level=CE2,NbExercice=10", HomeworkStep.GetExercice)]

    [InlineData("FirstName=Lucie,Level=CE2,Exercice=Additions,NbExercice=100", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Level=CE2,Exercice=DoesNotExists,NbExercice=10", HomeworkStep.GetExercice)]
    public void ShouldReturnExpectedStep_WhenGettingNextStep_GivenSpecificData(string serializedData, HomeworkStep expectedStep)
    {
        var data = HomeworkSession.CreateSessionFromCommaSeparatedKeyValues(serializedData);

        Assert.Equal(expectedStep, RequestRouting.GetNextStep(data));
    }
}
