using DevoirsAlexa.Domain;
using DevoirsAlexa.Domain.Enums;
using DevoirsAlexa.Domain.Models.Entities;
using System.Data;

namespace DevoirsAlexa.Infrastructure;

/// <summary>
/// Implementation of the <see cref="IWordsRepository"/> interface
/// </summary>
public class WordsRepository : IWordsRepository
{
  /// <inheritdoc/>
  public IEnumerable<Word> GetWordsForComparison(Levels level, int nbWords)
  {
    var words = new List<Word>();
    var compatibleWords = AllWords.Where(w => w.Level <= level).ToArray();

    if (nbWords > compatibleWords.Count()) 
      throw new ArgumentOutOfRangeException(nameof(nbWords));

    for (var i = 0; i < nbWords; i++)
    {
      var nextWordIndex = Random.Shared.Next(0, compatibleWords.Count() - 1);
      words.Add(compatibleWords[nextWordIndex]);
      compatibleWords = compatibleWords.Where(w => !words.Any(w2 => w2 == w)).ToArray();
    }

    return words;
  }

  internal static string[] CPWords => ["copain", "mouton", "avion", "bateau", "bébé", "main", "doigt", "bol", "bruit", "bus", "canard", "chien", "cheval", "ciel", "classe", "colline", "crayon", "vendredi", "doux", "drôle", "mardi", "enfant", "samedi", "froid", "gant", "gâteau", "grand", "guitare", "hiver", "image", "jouet", "jour", "loup", "maison", "lundi", "manteau", "matin", "mer", "miel", "monstre", "noir", "nuit", "pluie", "parc", "petit", "pleurer", "jeudi", "poisson", "poule", "premier", "dimanche", "propre", "rêve", "rire", "rond", "route", "rouge", "sac", "saison", "sapin", "sec", "serpent", "souris", "stylo", "sucre", "mercredi", "tapis", "tasse", "tigre", "tout", "train", "rat", "trottinette", "trou", "vélo", "vent", "ville", "voiture", "voleur", "pomme", "fraise", "reine", "ami", "cadeau", "fleur", "arbre", "fusée", "ballon", "bonbon", "chat", "école", "soleil", "porte", "rideau", "papa", "maman", "bois", "pain", "frère", "lit"];
  internal static string[] CE1Words => ["abeille", "aigle", "allumer", "pastèque", "basket", "beurre", "botte", "branche", "cabane", "carotte", "cerf", "chapeau", "chemin", "cochon", "couleur", "déjeuner", "écharpe", "étoile", "fabrique", "fenêtre", "fleurir", "fourmi", "garage", "glace", "grandir", "grenouille", "habit", "herbe", "histoire", "idée", "incroyable", "insecte", "joli", "lapin", "légume", "lumière", "machine", "marcher", "médecine", "melon", "méthode", "miracle", "montagne", "musique", "nager", "ouvrir", "papillon", "pause", "peindre", "personnage", "placer", "planète", "plaisir", "poivron", "policier", "pollution", "ananas", "prince", "promesse", "ramasser", "ranger", "repas", "gazeux", "robuste", "rouler", "saut", "secrétaire", "sentiment", "bientôt", "tableau", "terminer", "tour", "travail", "triste", "multiple", "chance", "simple", "courage", "plutôt", "sourire", "talent", "aujourd'hui", "mode", "jardin", "accord", "adresse", "dessin", "page", "patience", "goût", "confort", "jaune", "mixte", "toucher", "passer", "prendre", "calme", "fête", "groupe", "juste"];
  internal static string[] CE2Words => ["accrocher", "aventurier", "album", "animal", "apprécier", "assister", "autoroute", "boutique", "capturer", "cercle", "chocolat", "clavier", "compléter", "concert", "confiture", "culture", "délicieux", "détecter", "discuter", "durée", "écouter", "électricité", "envoyer", "enveloppe", "erreur", "fantastique", "festin", "forêt", "fragile", "frontière", "galerie", "grammaire", "hasard", "imaginer", "important", "incapable", "inviter", "isoler", "justice", "levier", "liberté", "magie", "mesurer", "modèle", "montagneux", "naturel", "nourrir", "observer", "obtenir", "offrir", "opérer", "origine", "passerelle", "peintre", "pirate", "planche", "plante", "popularité", "précis", "préparer", "présent", "protéger", "rare", "recherche", "réparer", "respect", "résultat", "rivière", "scène", "secret", "secouer", "simplifier", "absolument", "après", "sujet", "supporter", "surprise", "taquiner", "théorie", "trajet", "unique", "utilisateur", "valise", "valeur", "ventilateur", "vérifier", "village", "visiter", "vocabulaire", "voyager", "zone", "abricot", "brillant", "capitaine", "croissant", "essence", "général", "moteur", "spectacle", "station"];
  internal static string[] CM1Words => ["abandonner", "abonnement", "adaptation", "admiration", "adulte", "améliorer", "amitié", "ancienneté", "anticipation", "appareil", "applaudissement", "architecture", "assistance", "atmosphère", "attacher", "avalanche", "aventure", "bénéfice", "calculateur", "candidature", "célébration", "cérémonie", "circulation", "citoyen", "collection", "combinaison", "commencement", "communication", "compétition", "compliment", "conquête", "conservation", "conseil", "construction", "coopération", "encourager", "création", "critique", "découverte", "délégation", "décrire", "décoration", "défaite", "définition", "déménagement", "démonstration", "déviation", "déplacement", "dérivation", "description", "direction", "dispositif", "efficacité", "égoïsme", "emballage", "émeraude", "émotion", "encouragement", "engagement", "énigme", "ensemble", "enthousiasme", "environnement", "équipement", "équilibre", "parfum", "estimer", "établissement", "évaluation", "évolution", "exclusion", "exemplaire", "exposition", "fabrication", "facteur", "fascination", "ordinateur", "fonctionnaire", "fonctionnement", "fortification", "gouvernement", "gratitude", "habileté", "harmonieux", "hospitalité", "idéal", "illusion", "imagination", "impératif", "implication", "incitation", "inspiration", "installation", "instruction", "intervention", "intuition", "irrigation", "jardinage", "journalisme", "justification"];
  internal static string[] CM2Words => ["absolution", "abolition", "abondance", "astérisque", "administration", "affection", "ambition", "analyse", "anecdote", "anxiété", "architecte", "articulation", "ascension", "aspiration", "assurance", "stratosphère", "barrière", "bénévolat", "capacité", "capital", "changement", "civilité", "climat", "collaboration", "commande", "commentaire", "communauté", "comparaison", "complaisance", "composition", "compromis", "concentration", "concrétisation", "confidentialité", "confirmation", "connaissance", "déshumidification", "consultation", "contribution", "conviction", "correspondance", "crédibilité", "créativité", "obélisque", "cumul", "décision", "déclaration", "disqualifié", "déduction", "délibération", "monstrueux", "déontologie", "éléctricien", "développement", "diagnostic", "dialogue", "différence", "diplomatie", "discussion", "diversité", "documentation", "durabilité", "éducation", "maçonnerie", "élection", "éloquence", "élucider", "environnemental", "équilibriste", "solution", "exclusivité", "manifeste", "position", "fertilité", "formation", "moyen-âge", "franchise", "imprimante", "facilité", "fonctionnalité", "fraternité", "généralité", "générosité", "gestion", "estampiller", "héritage", "hospitalisé", "humanisme", "hypothèse", "processus", "immobilité", "importance", "impression", "inclusion", "indifférence", "initiative", "institution", "intelligence", "interruption", "justifier"];

  internal static IEnumerable<Word> AllWords = 
    CPWords.Select(w => new Word(w, Levels.CP))
    .Concat(CE1Words.Select(w => new Word(w, Levels.CE1)))
    .Concat(CE2Words.Select(w => new Word(w, Levels.CE2)))
    .Concat(CM1Words.Select(w => new Word(w, Levels.CM1)))
    .Concat(CM2Words.Select(w => new Word(w, Levels.CM2)));
}
