
using DevoirsAlexa.Domain.ToRemove;

namespace DevoirsAlexa.Application;

public class ExerciceSentenceBuilder : IExerciceSentenceBuilder
{
  private static Random _rand = new Random();

  private readonly static string[] PositiveFeedback = [
    "Bien joué !", "Super", "Génial !", "Saperlipopette !", "Pas mal !", "Fantastique !",
    "Bravo", "Magnifique !", "Parfait !", "Excellent !", "Impressionant !", "Top !", "Ok",
    "Chapeau !", "Sensass !", "Extraordinaire !", "Bazinga !", "Bingo !", "Houra !", "Kaboom !", "Mamma mia !",
    "Okey dokey !", "Ouah !", "Ouf ", "Oui !", "Ta da !", "Voilà !", "Waouh !", "Woo hoo !", "Yay !", "Youpi !", "Ça alors !"
    ];

  private readonly static string[] NegativeFeedback = [
    "Dommage...", "Noooon", "Raté", "Faux...", "Erreur...", "Zut.", "Zut alors.", "Aïe...", "Oh là là.", "Oh mince.",
    "Oups...", "Manqué...", "Hélas.", "Désolé...", "Oh non.", "Aouch...", "Bof.", "Boo hoo.", "C'est la vie.",
    "Eeeeh non.", "Huh huh.", "Mince.", "Olah.", "Oooh.", "Oulah.", "Oups.", "Outch.", "Pas facile, hein ?", "Patatras.",
    "Punaise.", "Tralala."
  ];

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
  
  public void GetExerciceAnswerSentence(ISentenceBuilder sentenceBuilder, bool isValidAnswer, string correctAnswer)
  {
    if (isValidAnswer)
    {
      sentenceBuilder.AppendInterjection(PositiveFeedback[_rand.Next(0, PositiveFeedback.Length)]);
      sentenceBuilder.AppendSimpleText(" C'est une bonne réponse !");
    }
    else
    {
      sentenceBuilder.AppendInterjection(NegativeFeedback[_rand.Next(0, NegativeFeedback.Length)]);
      sentenceBuilder.AppendSimpleText(" La bonne réponse était ");
      sentenceBuilder.AppendPause();
      sentenceBuilder.AppendSimpleText(correctAnswer + ".");
    }
  }

  public void GetEndOfExerciceCompletionSentence(ISentenceBuilder sentenceBuilder, int nbCorrectAnswers, int nbQuestionAsked, TimeSpan totalTime)
  {
    var level = Math.Max(1, Math.Round((double)nbCorrectAnswers / (double)nbQuestionAsked * 5));

    var assessmentsForLevel = LevelAssessment[(int)level];
    var assessment = assessmentsForLevel[_rand.Next(0, assessmentsForLevel.Length)];
    
    sentenceBuilder.AppendSimpleText($"Tu as {nbCorrectAnswers} ");
    sentenceBuilder.AppendPossiblePlural("bonne", nbCorrectAnswers);
    sentenceBuilder.AppendPossiblePlural(" réponse", nbCorrectAnswers);
    sentenceBuilder.AppendSimpleText($" sur {nbQuestionAsked} ");
    sentenceBuilder.AppendPossiblePlural("question", nbQuestionAsked);

    if (totalTime.TotalSeconds > 0 && totalTime.TotalSeconds < 300)
    {
      var time = GetTimeSpanDescription(totalTime);
      sentenceBuilder.AppendSimpleText($", en moins de {time}. ");
    }
    else
      sentenceBuilder.AppendSimpleText(". ");

    sentenceBuilder.AppendSimpleText($"{assessment}");
  }

  private static string GetTimeSpanDescription(TimeSpan timeSpan)
  {
    int minutes = timeSpan.Minutes;
    int seconds = timeSpan.Seconds;

    string minuteText = minutes > 0 ? $"{minutes} minute{(minutes > 1 ? "s" : "")}" : "";
    string secondText = seconds > 0 ? $"{seconds} seconde{(seconds > 1 ? "s" : "")}" : "";
    string inBetween = minutes > 0 && seconds > 0 ? " et " : " ";
    return $"{minuteText}{inBetween}{secondText}".Trim();
  }
}
