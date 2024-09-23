namespace Homework.ExercisesRunner;

internal class ExerciceSentenceBuilder
{
  private readonly static string[] PositiveFeedback = [
    "Bien joué !", "Super !", "Génial !", "Saperlipopette !", "Pas mal !", "Fantastique !",
    "Bravo !", "Magnifique !", "Parfait !", "Excellent !", "Impressionant !", "Top !",
    "Chapeau !", "Sensass !", "Extraordinaire !"
    ];

  private readonly static string[] NegativeFeedback = [
    "Dommage...", "Non...", "Raté...", "Faux...", "Erreur...", "Zut...", "Aïe...",
    "Oups...", "Manqué...", "Hélas...", "Désolé...", "Oh non...", "Aouch..."];


  private readonly static Dictionary<int, string[]> LevelAssessment = new Dictionary<int, string[]>{
          { 1, [
                  "Tu t'en sors bien !",
                  "Pas mal du tout !",
                  "C’est un bon début !",
                  "Bien essayé !",
                  "Tu progresses doucement !",
                  "C’est déjà ça !",
                  "Tu avances tranquillement !",
                  "Tu fais de bons efforts !",
                  "Tu es sur la bonne voie !",
                  "Ça commence à prendre forme !"
              ]
          },
          { 2, [
                  "Continue comme ça !",
                  "Bon travail, c’est prometteur !",
                  "Bravo, ça se voit que tu y mets du tien !",
                  "Joli travail, continue !",
                  "Tu prends de l'assurance !",
                  "Bien joué, c’est en bonne voie !",
                  "Tes efforts commencent à payer !",
                  "Tu t'améliores à chaque étape !",
                  "C’est de mieux en mieux !",
                  "Bien vu, tu progresses !"
              ]
          },
          { 3, [
                  "Super travail !",
                  "Bravo, tu fais du bon boulot !",
                  "Excellent effort !",
                  "Tu es sur la bonne piste !",
                  "Très bon résultat !",
                  "Formidable, tu es sur la bonne route !",
                  "Tu fais vraiment de beaux progrès !",
                  "Bon travail, ça se ressent !",
                  "Tes efforts sont remarquables !",
                  "Super, tu avances bien !"
              ]
          },
          { 4, [
                  "Bravo, c’est exactement ça !",
                  "Magnifique effort, continue !",
                  "Quelle belle avancée !",
                  "Génial, tu te surpasses !",
                  "Tu as un vrai talent !",
                  "C’est impeccable !",
                  "Bravo, c’est tout à fait ça !",
                  "Tu fais ça comme un pro !",
                  "Formidable, tu es incroyable !",
                  "Quelle performance !"
              ]
          },
          { 5, [
                  "Fantastique, tu as réussi !",
                  "Impressionnant, quel succès !",
                  "Tu maîtrises parfaitement !",
                  "Extraordinaire, vraiment bien joué !",
                  "Bravo, mission accomplie avec brio !",
                  "Chapeau, tu excelles !",
                  "Incroyable, tu es un champion !",
                  "C’est sensationnel, rien à redire !",
                  "Splendide, tu fais un sans-faute !",
                  "Génial, tu as atteint la perfection !"
              ]
          }
      };

  internal static string GetExerciceAnswerSentence(bool isValidAnswer, string correctAnswer)
  {
    var rand = new Random();
    var text = isValidAnswer ? PositiveFeedback[rand.Next(0, PositiveFeedback.Length)] : $"{NegativeFeedback[rand.Next(0, NegativeFeedback.Length)]} La bonne réponse était {correctAnswer}.";
    return text;
  }

  internal static string GetEndOfExerciceCompletionSentence(int nbCorrectAnswers, int nbQuestionAsked, TimeSpan totalTime)
  {
    var rand = new Random();

    var level = Math.Round((double)nbCorrectAnswers / (double)nbQuestionAsked * 5);
    var pluralAnswer = nbCorrectAnswers > 1 ? "s" : "";
    var pluralQuestion = nbQuestionAsked > 1 ? "s" : "";

    var assessmentsForLevel = LevelAssessment[(int)level];
    var assessment = assessmentsForLevel[rand.Next(0, assessmentsForLevel.Length)];
    var text = string.Empty;
    if (totalTime.TotalSeconds > 0 && totalTime.TotalSeconds < 300)
    {
      var time = GetTimeSpanDescription(totalTime);
      text += $"L'exercice est terminé en moins de {time} secondes. ";
    }
    text += $"Tu as {nbCorrectAnswers} bonne{pluralAnswer} réponse{pluralAnswer} sur {nbQuestionAsked} question{pluralQuestion}. {assessment}";

    return text;

  }

  private static string GetTimeSpanDescription(TimeSpan timeSpan)
  {
    int minutes = timeSpan.Minutes;
    int seconds = timeSpan.Seconds;

    string minuteText = minutes > 0 ? $"{minutes} minute{(minutes > 1 ? "s" : "")}" : "";
    string secondText = seconds > 0 ? $"{seconds} seconde{(seconds > 1 ? "s" : "")}" : "";
    string inBetween = minutes > 0 && seconds > 0 ? " et " : " ";
    return $"{minuteText}{inBetween}{secondText}";
  }

}
