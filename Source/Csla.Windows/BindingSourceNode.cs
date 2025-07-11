﻿//-----------------------------------------------------------------------
// <copyright file="BindingSourceNode.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Maintains a reference to a BindingSource object</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Core;

namespace Csla.Windows
{
  /// <summary>
  /// Maintains a reference to a BindingSource object
  /// on the form.
  /// </summary>
  public class BindingSourceNode
  {
    internal BindingSource Source { get; }

    internal List<BindingSourceNode> Children { get; } = [];

    internal BindingSourceNode? Parent { get; set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="source">
    /// BindingSource object to be managed.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public BindingSourceNode(BindingSource source)
    {
      Source = source ?? throw new ArgumentNullException(nameof(source));
      Source.CurrentChanged += BindingSource_CurrentChanged;
    }

    private void BindingSource_CurrentChanged(object? sender, EventArgs e)
    {
      if (Children.Count > 0)
        foreach (BindingSourceNode child in Children)
          child.Source.EndEdit();
    }

    internal void Unbind(bool cancel)
    {
      if (Children.Count > 0)
        foreach (BindingSourceNode child in Children)
          child.Unbind(cancel);

      IEditableObject? current = Source.Current as IEditableObject;

      if (!(Source.DataSource is BindingSource))
        Source.DataSource = null;

      if (current != null)
      {
        if (cancel)
          current.CancelEdit();
        else
          current.EndEdit();
      }

      if (Source.DataSource is BindingSource && Parent is not null)
        Source.DataSource = Parent.Source;
    }

    internal void EndEdit()
    {
      if (Children.Count > 0)
        foreach (BindingSourceNode child in Children)
          child.EndEdit();

      Source.EndEdit();
    }

    internal void SetEvents(bool value)
    {
      Source.RaiseListChangedEvents = value;

      if (Children.Count > 0)
        foreach (BindingSourceNode child in Children)
          child.SetEvents(value);
    }

    internal void ResetBindings(bool refreshMetadata)
    {
      if (Children.Count > 0)
        foreach (BindingSourceNode child in Children)
          child.ResetBindings(refreshMetadata);

      Source.ResetBindings(refreshMetadata);
    }

    /// <summary>
    /// Binds a business object to the BindingSource.
    /// </summary>
    /// <param name="objectToBind">
    /// Business object.
    /// </param>
    public void Bind(object? objectToBind)
    {
      if (objectToBind is ISupportUndo root)
        root.BeginEdit();

      Source.DataSource = objectToBind;
      SetEvents(true);
      ResetBindings(false);
    }

    /// <summary>
    /// Applies changes to the business object.
    /// </summary>
    public void Apply()
    {
      SetEvents(false);

      ISupportUndo? root = Source.DataSource as ISupportUndo;

      Unbind(false);
      EndEdit();

      root?.ApplyEdit();
    }

    /// <summary>
    /// Cancels changes to the business object.
    /// </summary>
    /// <param name="businessObject"></param>
    public void Cancel(object? businessObject)
    {
      SetEvents(false);

      ISupportUndo? root = Source.DataSource as ISupportUndo;

      Unbind(true);

      root?.CancelEdit();

      Bind(businessObject);
    }

    /// <summary>
    /// Disconnects from the BindingSource object.
    /// </summary>
    public void Close()
    {
      SetEvents(false);
      Unbind(true);
    }

  }
}
