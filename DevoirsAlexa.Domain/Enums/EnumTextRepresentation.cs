
namespace DevoirsAlexa.Domain.Enums;


[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
internal sealed class TextRepresentationsAttribute : Attribute
{
  public string[] StringValue { get; }

  public TextRepresentationsAttribute(params string[] stringValue)
  {
    StringValue = stringValue;
  }
}

/// <summary>
/// An helper class to pass from string to Enum
/// Used especially within the session to transmit data properly
/// </summary>
public static class EnumHelper {

  /// <summary>
  /// Get the enum value from a string
  /// </summary>
  /// <typeparam name="TEnum">Which enum is concerned</typeparam>
  /// <param name="str">The data string stored in the session</param>
  /// <returns>The enum value parsed from the string and matched against potential synonyms</returns>
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

  /// <summary>
  /// Parse the string to get if true, false or unknown
  /// </summary>
  /// <param name="str"></param>
  /// <returns>Boolean value</returns>
  public static bool? ParseBooleanAnswer(this string str)
  {
    var ba = str?
      .ToLowerInvariant()
      .GetEnumFromTextRepresentations<BooleanAnswer>();

    return ba switch
    {
      BooleanAnswer.True => true,
      BooleanAnswer.False => false,
      _ => null
    };
  }
}
