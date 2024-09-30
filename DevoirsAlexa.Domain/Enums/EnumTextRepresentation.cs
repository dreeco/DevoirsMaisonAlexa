
namespace DevoirsAlexa.Domain.Enums;


[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class TextRepresentationsAttribute : Attribute
{
  public string[] StringValue { get; }

  public TextRepresentationsAttribute(params string[] stringValue)
  {
    StringValue = stringValue;
  }
}

public static class EnumHelper {

  public static TEnum? GetEnumFromTextRepresentations<TEnum>(this string str) where TEnum : struct, Enum
  {
    foreach (var e in Enum.GetValues<TEnum>())
    {
      if (str.Equals(e.ToString(), StringComparison.InvariantCultureIgnoreCase))
        return e;

      var textRepresentations = e
      .GetType()
      .GetField(e.ToString())?
      .GetCustomAttributes(typeof(TextRepresentationsAttribute), false)
      .FirstOrDefault() as TextRepresentationsAttribute;

      if (textRepresentations?.StringValue == null)
        continue;
      else if (textRepresentations.StringValue.Any(representation => str.Contains(representation, StringComparison.InvariantCultureIgnoreCase)))
        return e;
    }
    return null;
  }
}
