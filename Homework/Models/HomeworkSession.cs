namespace Homework.Models
{
  public class HomeworkSession : Dictionary<string, object>
  {
    public HomeworkSession() : base() { }
    public HomeworkSession(Dictionary<string, object>? session) : base(session ?? new Dictionary<string, object>()) { }
    public HomeworkSession(string? session) :base(CreateSessionFromCommaSeparatedKeyValues(session)) { }

    public static HomeworkSession CreateSessionFromCommaSeparatedKeyValues(string? str)
    {
      var _this = new HomeworkSession();

      if (string.IsNullOrWhiteSpace(str))
        return _this;

      foreach (var s in str
        .Split(',') // separate different properties
        .Where(s => !string.IsNullOrWhiteSpace(s)) //Filter out empty ones
        .Select(s => s.Split('='))//Separate key from value
        )
        _this.Add(s[0], s[1]);

      return _this;
    }

    public bool TryGetString(string key, out string value)
    {
      value = string.Empty;
      if (!this.TryGetValue(key, out var valueObject))
        return false;
      value = (valueObject is string ? valueObject as string : valueObject.ToString()) ?? string.Empty;
      return true;

    }
  }
}
