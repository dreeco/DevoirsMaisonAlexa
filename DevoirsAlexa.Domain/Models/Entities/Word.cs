using DevoirsAlexa.Domain.Enums;

namespace DevoirsAlexa.Domain.Models.Entities;

/// <summary>
/// Represent a word for language exercises
/// </summary>
/// <param name="Text">The word itself</param>
/// <param name="Level">The class level associated</param>
public record Word(string Text, Levels Level);
