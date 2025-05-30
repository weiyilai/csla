﻿namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{
  /// <summary>
  /// The definition of a member's type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedMemberTypeDefinition : IEquatable<ExtractedMemberTypeDefinition>
  {
    /// <summary>
    /// Gets or sets a value indicating whether the type is nullable.
    /// </summary>
    public bool Nullable { get; internal set; }

    /// <summary>
    /// The globally fully qualified type name.
    /// </summary>
    public string FullyQualifiedType { get; set; } = "";

    /// <summary>
    /// Determines whether the current <see cref="ExtractedMemberTypeDefinition"/> object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(ExtractedMemberTypeDefinition? other)
    {
      if (other == null)
        return false;

      return FullyQualifiedType == other.FullyQualifiedType && Nullable == other.Nullable;
    }

    /// <summary>
    /// Determines whether the current <see cref="ExtractedMemberTypeDefinition"/> object is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>

    public override bool Equals(object? obj)
    {
      if (obj is ExtractedMemberTypeDefinition other)
        return Equals(other);

      return false;
    }
    /// <summary>
    /// Calculates the hash code for the <see cref="ExtractedMemberTypeDefinition"/> object.
    /// </summary>
    /// <returns>The calculated hash code.</returns>
    public override int GetHashCode()
    {
      return HashCode.Combine(FullyQualifiedType, Nullable);
    }
  }
}
