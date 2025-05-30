//-----------------------------------------------------------------------
// <copyright file="ObjectSchema.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object providing access to schema information for</summary>
//-----------------------------------------------------------------------
using System.Web.UI.Design;

namespace Csla.Web.Design
{

  /// <summary>
  /// Object providing access to schema information for
  /// a business object.
  /// </summary>
  /// <remarks>
  /// This object returns only one view, which corresponds
  /// to the business object used by data binding.
  /// </remarks>
  public class ObjectSchema : IDataSourceSchema
  {
    private string _typeName = "";
    private CslaDataSourceDesigner? _designer;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="designer">Data source designer object.</param>
    /// <param name="typeName">Type name for which the schema should be generated.</param>
    /// <exception cref="ArgumentNullException"><paramref name="typeName"/> is <see langword="null"/>.</exception>
    public ObjectSchema(CslaDataSourceDesigner? designer, string typeName)
    {
      _typeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
      _designer = designer;
    }


    /// <summary>
    /// Returns a single element array containing the
    /// schema for the CSLA .NET business object.
    /// </summary>
    public IDataSourceViewSchema[] GetViews()
    {
      return [new ObjectViewSchema(_designer, _typeName)];
    }
  }
}
