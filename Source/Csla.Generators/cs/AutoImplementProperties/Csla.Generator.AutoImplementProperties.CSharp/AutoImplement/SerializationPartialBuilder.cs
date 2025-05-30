﻿//-----------------------------------------------------------------------
// <copyright file="SerializationPartialBuilder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Builds the text of a partial type to implement properties</summary>
//-----------------------------------------------------------------------

using System.CodeDom.Compiler;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{

  /// <summary>
  /// Builds the text of a partial type to implement the properties
  /// </summary>
  internal class SerializationPartialBuilder(bool nullable)
  {
    /// <summary>
    /// Build the text of a partial type that implements the properties of the target type
    /// </summary>
    /// <param name="typeDefinition">The definition of the type for which is required</param>
    /// <returns>Generated code to fulfil the required auto implementation of the properties</returns>
    internal GenerationResults BuildPartialTypeDefinition(ExtractedTypeDefinition typeDefinition)
    {
      using var stringWriter = new StringWriter();
      var textWriter = new IndentedTextWriter(stringWriter, "\t");

      textWriter.WriteLine("//<auto-generated/>");

      if (nullable)
        textWriter.WriteLine("#nullable enable");

      AppendUsingStatements(textWriter);

      AppendTypeDefinition(textWriter, typeDefinition);
      AppendBlockStart(textWriter);

      AppendProperties(textWriter, typeDefinition);

      AppendBlockEnd(textWriter);

      return new GenerationResults
      {
        FullyQualifiedName = typeDefinition.FullyQualifiedName,
        GeneratedSource = stringWriter.ToString()
      };
    }

    #region Private Helper Methods

    /// <summary>
    /// Append the start of a code block, indenting the writer
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the block start</param>
    private void AppendBlockStart(IndentedTextWriter textWriter)
    {
      textWriter.WriteLine('{');
      textWriter.Indent++;
    }

    /// <summary>
    /// Append the end of a code block, having first unindented the writer
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the block end</param>
    private void AppendBlockEnd(IndentedTextWriter textWriter)
    {
      textWriter.Indent--;
      textWriter.WriteLine('}');
    }

    /// <summary>
    /// Append the required using statements required on a partial class in
    /// order for it to compile the code we have generated
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the usings</param>
    private void AppendUsingStatements(IndentedTextWriter textWriter)
    {
      HashSet<string> requiredNamespaces;

      requiredNamespaces = GetRequiredNamespaces();

      foreach (string requiredNamespace in requiredNamespaces.Where(s => !string.IsNullOrWhiteSpace(s)))
      {
        textWriter.Write("using ");
        textWriter.Write(requiredNamespace);
        textWriter.WriteLine(';');
      }

      textWriter.WriteLine();
    }

    /// <summary>
    /// Retrieve all of the namespaces that are required for generation of the defined type
    /// </summary>
    /// <returns>A hashset of all of the namespaces required for generation</returns>
    private HashSet<string> GetRequiredNamespaces()
    {
      return ["System", "Csla"];
    }

    /// <summary>
    /// Append the type definition of the partial we are generating
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the type definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendTypeDefinition(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {
      if (!string.IsNullOrWhiteSpace(typeDefinition.Namespace))
      {
        textWriter.WriteLine($"namespace {typeDefinition.Namespace};");
        textWriter.WriteLine();
      }

      textWriter.Write(typeDefinition.Scope);
      textWriter.Write(" partial ");
      textWriter.Write(typeDefinition.TypeKind);
      textWriter.Write(' ');
      textWriter.Write(typeDefinition.TypeName);
      textWriter.WriteLine();
    }

    /// <summary>
    /// Append the definition of the GetChildren method required to fulfil the IMobileObject interface
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the method definition</param>
    /// <param name="typeDefinition">The definition of the type for which we are generating</param>
    private void AppendProperties(IndentedTextWriter textWriter, ExtractedTypeDefinition typeDefinition)
    {

      foreach (ExtractedPropertyDefinition propertyDefinition in typeDefinition.Properties)
      {
        AppendSerializeChildFragment(textWriter, propertyDefinition, typeDefinition);
      }
    }

    /// <summary>
    /// Append the code fragment necessary to serialize an individual child member
    /// </summary>
    /// <param name="textWriter">The IndentedTextWriter instance to which to append the fragment</param>
    /// <param name="propertyDefinition">The definition of the member we are writing for</param>
    /// <param name="typeDefinition"></param>
    private void AppendSerializeChildFragment(IndentedTextWriter textWriter, ExtractedPropertyDefinition propertyDefinition, ExtractedTypeDefinition typeDefinition)
    {
      var getter = GetGetterMethod(typeDefinition);
      var setter = GetSetterMethod(typeDefinition);
      if (string.IsNullOrEmpty(getter) || string.IsNullOrEmpty(setter))
        return;

      textWriter.WriteLine($"public static readonly PropertyInfo<{propertyDefinition.TypeDefinition.FullyQualifiedType}> {propertyDefinition.PropertyName}Property = RegisterProperty<{propertyDefinition.TypeDefinition.FullyQualifiedType}>(nameof({propertyDefinition.PropertyName}));");

      foreach (ExtractedAttributeDefinition attributeDefinition in propertyDefinition.AttributeDefinitions)
      {
        var constructorArguments = string.Join(", ", attributeDefinition.ConstructorArguments);
        var separator = attributeDefinition.ConstructorArguments.Any() && attributeDefinition.NamedProperties.Any() ? "," : "";
        var namedProperties = string.Join(", ", attributeDefinition.NamedProperties.Select(kv => $"{kv.Key}={kv.Value}"));
        textWriter.WriteLine($"[{attributeDefinition.AttributeName}({constructorArguments}{separator} {namedProperties})]");
      }

      textWriter.WriteLine($"{string.Join(" ", propertyDefinition.Modifiers)} {propertyDefinition.TypeDefinition.FullyQualifiedType} {propertyDefinition.PropertyName}");
      AppendBlockStart(textWriter);
      if (propertyDefinition.Getter)
      {
        textWriter.Write($"get => {getter}({propertyDefinition.PropertyName}Property)");
        if (!propertyDefinition.TypeDefinition.Nullable)
        {
          textWriter.Write('!');
        }

        textWriter.WriteLine(';');
      }

      if (propertyDefinition.Setter)
      {
        var setterseparator = propertyDefinition.SetterModifiers.Any() ? " " : "";
        textWriter.WriteLine($"{string.Join(" ", propertyDefinition.SetterModifiers)}{setterseparator}set => {setter}({propertyDefinition.PropertyName}Property, value);");
      }

      AppendBlockEnd(textWriter);
    }

    private string GetGetterMethod(ExtractedTypeDefinition typeDefinition)
    {
      if (typeDefinition.BaseClassTypeName.Contains("BusinessBase"))
      {
        return "GetProperty";
      }
      if (typeDefinition.BaseClassTypeName.Contains("ReadOnlyBase"))
      {
        return "GetProperty";
      }
      if (typeDefinition.BaseClassTypeName.Contains("CommandBase"))
      {
        return "ReadProperty";
      }
      return string.Empty;
    }
    private string GetSetterMethod(ExtractedTypeDefinition typeDefinition)
    {
      if (typeDefinition.BaseClassTypeName.Contains("BusinessBase"))
      {
        return "SetProperty";
      }
      if (typeDefinition.BaseClassTypeName.Contains("ReadOnlyBase"))
      {
        return "LoadProperty";
      }
      if (typeDefinition.BaseClassTypeName.Contains("CommandBase"))
      {
        return "LoadProperty";
      }
      return string.Empty;
    }
    #endregion

  }

}
