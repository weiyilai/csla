﻿//-----------------------------------------------------------------------
// <copyright file="PropertyDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a single property of a type for source generation</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization.Discovery
{

  /// <summary>
  /// Extract the definition of a single property of a type for which source generation is being performed
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  internal static class PropertyDefinitionExtractor
  {

    /// <summary>
    /// Extract information about a single property from its declaration in the syntax tree
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of ExtractedPropertyDefinition containing the data extracted from the syntax tree</returns>
    public static ExtractedPropertyDefinition ExtractPropertyDefinition(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      ExtractedPropertyDefinition propertyDefinition = new ExtractedPropertyDefinition
      {
        PropertyName = GetPropertyName(propertyDeclaration),
        TypeDefinition = new ExtractedMemberTypeDefinition
        {
          IsAutoSerializable = extractionContext.IsTypeAutoSerializable(propertyDeclaration.Type),
          ImplementsIMobileObject = extractionContext.DoesTypeImplementIMobileObject(propertyDeclaration.Type),
          Nullable = GetFieldTypeNullable(propertyDeclaration),
          GloballyFullyQualifiedType = extractionContext.GetFullyQualifiedType(propertyDeclaration.Type)
        }
      };

      return propertyDefinition;
    }

    #region Private Helper Methods

    /// <summary>
    /// Determines whether the field type is nullable.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the field declaration.</param>
    /// <returns><c>true</c> if the field type is nullable; otherwise, <c>false</c>.</returns>
    private static bool GetFieldTypeNullable(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Type is NullableTypeSyntax;
    }

    /// <summary>
    /// Extract the name of the property for which we are building information
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the property for which we are extracting information</returns>
    private static string GetPropertyName(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Identifier.ValueText;
    }

    /// <summary>
    /// Extract the type name of the property for which we are building information
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The type name of the property for which we are extracting information</returns>
    private static string GetPropertyTypeName(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Type.ToString();
    }

    #endregion

  }
}
