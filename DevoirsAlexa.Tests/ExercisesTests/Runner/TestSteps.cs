using Homework.HomeworkExercisesRunner;
using Homework.Enums;
using Homework.Models;
using Xunit;

namespace DevoirsAlexa.Tests.ExercisesTests.Runner;

public class TestSteps
{

    [Theory]
    [InlineData("", HomeworkStep.GetFirstName)]
    [InlineData("FirstName=Lucie", HomeworkStep.GetAge)]
    [InlineData("FirstName=Lucie,Age=8", HomeworkStep.GetExercice)]

    [InlineData("FirstName=Lucie,Age=8,Exercice=Additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=Multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=Divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=Soustractions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=soustractions", HomeworkStep.GetNbExercice)]

    [InlineData("FirstName=Lucie,Age=8,Exercice=table d' Additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table d' additions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table de Multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table de multiplications", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table de Divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table de divisions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table de Soustractions", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=table de soustractions", HomeworkStep.GetNbExercice)]


    [InlineData("FirstName=Lucie,Age=8,Exercice=Additions,NbExercice=10", HomeworkStep.StartExercice)]

    [InlineData("Age=8,Exercice=Additions,NbExercice=10", HomeworkStep.GetFirstName)]
    [InlineData("FirstName=Lucie,Exercice=Additions,NbExercice=10", HomeworkStep.GetAge)]
    [InlineData("FirstName=Lucie,Age=8,NbExercice=10", HomeworkStep.GetExercice)]

    [InlineData("FirstName=Lucie,Age=8,Exercice=Additions,NbExercice=100", HomeworkStep.GetNbExercice)]
    [InlineData("FirstName=Lucie,Age=8,Exercice=DoesNotExists,NbExercice=10", HomeworkStep.GetExercice)]
    public void ShouldReturnExpectedStep_WhenGettingNextStep_GivenSpecificData(string serializedData, HomeworkStep expectedStep)
    {
        var data = HomeworkSession.CreateSessionFromCommaSeparatedKeyValues(serializedData);
        var homeworkRunner = new ExerciceRunner(data);

        Assert.Equal(expectedStep, homeworkRunner.GetNextStep());

    }
}
