
namespace DevoirsAlexa.Domain;

public interface ISentenceBuilder
{
  public void AppendSimpleText(string text);
  public void AppendInterjection(string text);
  public void AppendPause(TimeSpan? timespan = null);
  public void AppendPossiblePlural(string text, int nb);
}
