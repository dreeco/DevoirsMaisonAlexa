using Homework.Models;

namespace Homework.HomeworkExercises.MathExercices;

public abstract class BaseTableExercises
{
    protected char Operation { get; set; }
    protected string OperationText { get; set; }
    protected BaseTableExercises(char operation, string operationText)
    {
        if (!new char[] { '+', '/', '*', '-' }.Contains(operation))
            throw new ArgumentException(nameof(operation));
        Operation = operation;

        if (string.IsNullOrWhiteSpace(operationText))
            throw new ArgumentNullException(nameof(operationText));

        OperationText = operationText;
    }

    public Question NextQuestion(int min, int max, IEnumerable<string> alreadyAsked)
    {

        var random = new Random();
        int x;
        int y;
        string key;
        var n = 0;
        do
        {
            x = random.Next(min, max);
            y = random.Next(min, max);
            key = $"{x}{Operation}{y}";
        }
        while (n++ < 100 && alreadyAsked.Contains(key));

        return new Question(key, $"Combien font {key.Replace(Operation.ToString(), $" {OperationText} ")} ?");
    }

    public int? GetCorrectAnswer(string questionKey)
    {
        var parts = questionKey.Split(Operation);
        int? previous = null;

        foreach (var current in parts.Select(p => int.Parse(p)))
        {
            if (previous == null)
            {
                previous = current;
                continue;
            }

            switch (Operation)
            {
                case '+':
                    previous += current;
                    break;
                case '*':
                    previous *= current;
                    break;
                case '/':
                    previous /= current;
                    break;
                case '-':
                    previous -= current;
                    break;
            }
        }
        return previous;
    }

    public AnswerValidation ValidateAnswer(string questionKey, string answer)
    {
        var resultNumber = GetCorrectAnswer(questionKey);
        if (resultNumber == null)
            return new AnswerValidation(false, "Impossible de calculer la bonne réponse");

        if (!int.TryParse(answer, out var answerNumber))
#pragma warning disable CS8604 // The if null prevents null.
            return new AnswerValidation(false, resultNumber.ToString());
#pragma warning restore CS8604

        return new AnswerValidation(resultNumber == answerNumber, resultNumber?.ToString() ?? string.Empty);
    }

}
