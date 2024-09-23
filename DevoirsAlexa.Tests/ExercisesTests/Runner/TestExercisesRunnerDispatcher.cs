using Homework.Enums;
using Homework.HomeworkExercises.MathExercices;
using Homework.HomeworkExercisesRunner;
using Homework.Models;
using Xunit;

namespace DevoirsAlexa.Tests.ExercisesTests.Runner
{
    public class TestExercisesRunnerDispatcher
    {
        public HomeworkSession? _currentSession { get; private set; }

        [Theory]
        [InlineData(HomeworkExercisesTypes.Unknown, null)]
        [InlineData(HomeworkExercisesTypes.Additions, typeof(AdditionsExercises))]
        [InlineData(HomeworkExercisesTypes.Substractions, typeof(SubstractionsExercises))]
        [InlineData(HomeworkExercisesTypes.Multiplications, typeof(MultiplicationsExercises))]
        [InlineData(HomeworkExercisesTypes.Divisions, typeof(DivisionsExercises))]
        public void ShouldReturnType_GivenSpecificExercice(HomeworkExercisesTypes exercice, Type? expectedType)
        {
            var dispatcher = new ExerciceDispatcher();
            var exerciceInstance = dispatcher.GetExerciceQuestionsRunner(exercice);

            if (expectedType == null)
                Assert.Null(exerciceInstance);
            else
                Assert.Equal(expectedType, exerciceInstance?.GetType());
        }


        [Theory]
        //[InlineData(HomeworkExercises.Unknown, null)]
        [InlineData("FirstName=Alix,Age=6,Exercice=Additions,NbExercice=", 5, @"\d+\+\d+", @"Combien font \d+ plus \d+ ?")]
        [InlineData("FirstName=Elio,Age=4,Exercice=Multiplications,NbExercice=", 5, @"\d+\*\d+", @"Combien font \d+ multiplié par \d+ ?")]
        [InlineData("FirstName=Jonathan,Age=47,Exercice=Soustractions,NbExercice=", 5, @"\d+\-\d+", @"Combien font \d+ moins \d+ ?")]
        public void ShouldReturnNextQuestionAfterExercice_GivenCompleteSessionData(string session, int nbExercice, string questionKeyPattern, string questionTextPattern)
        {
            session += nbExercice.ToString();
            _currentSession = new HomeworkSession(session);

            var runner = new ExerciceRunner(_currentSession);

            for (var questionAskedLoopBegin = 0; questionAskedLoopBegin <= nbExercice; questionAskedLoopBegin++)
            {
                ThereWasNQuestionAskedAndAnswered(questionAskedLoopBegin);
                var outputText = runner.NextQuestion();
                var questionAsked = questionAskedLoopBegin + 1;

                var lastQuestionKey = runner.LastQuestionKey;

                var isLastRun = questionAskedLoopBegin == nbExercice;
                if (!isLastRun)
                {
                    ThenIHaveANewCorrectAnswer(questionAskedLoopBegin);
                    Assert.NotNull(lastQuestionKey);
                    ThenTheQuestionAskedMatchesTheExpectedPatterns(lastQuestionKey, questionKeyPattern, questionTextPattern, runner, outputText);
                    _currentSession.LastAnswer = runner.GetCorrectAnswer(lastQuestionKey);
                }
                else
                {
                    Assert.Null(lastQuestionKey);

                    ThenTheQuestionDoesNotMatchExerciceQuestionPattern(questionTextPattern, outputText);
                    ThenTheDataAreCleanedForNextExercice();
                }
            }
        }

        private void ThenIHaveANewCorrectAnswer(int n)
        {
            Assert.NotNull(_currentSession);
            Assert.Equal(n, _currentSession.CorrectAnswers);
        }

        private void ThereWasNQuestionAskedAndAnswered(int n)
        {
            Assert.NotNull(_currentSession);
            Assert.Equal(n, _currentSession.QuestionAsked);
            Assert.Equal(Math.Max(0, n - 1), _currentSession.CorrectAnswers);
        }

        private static void ThenTheQuestionDoesNotMatchExerciceQuestionPattern(string questionTextPattern, string outputText)
        {
            Assert.DoesNotMatch(questionTextPattern, outputText);
        }

        private static string ThenTheQuestionAskedMatchesTheExpectedPatterns(string questionKey, string questionKeyPattern, string questionTextPattern, ExerciceRunner runner, string outputText)
        {
            Assert.Matches(questionKeyPattern, questionKey);
            Assert.Matches(questionTextPattern, outputText);
            return questionKey;
        }

        private void ThenTheDataAreCleanedForNextExercice()
        {
            Assert.NotNull(_currentSession);
            Assert.Empty(_currentSession.AlreadyAsked);
            Assert.Null(_currentSession.Exercice);
            Assert.Equal(0, _currentSession.CorrectAnswers);
            Assert.Equal(0, _currentSession.QuestionAsked);
        }
    }
}
