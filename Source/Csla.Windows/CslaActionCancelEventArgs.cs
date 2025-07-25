﻿//-----------------------------------------------------------------------
// <copyright file="CslaActionCancelEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Event args providing information about</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace Csla.Windows
{
  /// <summary>
  /// Event args providing information about
  /// a canceled action.
  /// </summary>
  public class CslaActionCancelEventArgs : CancelEventArgs
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="cancel">
    /// Indicates whether a cancel should occur.
    /// </param>
    /// <param name="commandName">
    /// Name of the command.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="commandName"/> is <see langword="null"/>.</exception>
    public CslaActionCancelEventArgs(bool cancel, string commandName)
      : base(cancel)
    {
      CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
    }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string CommandName { get; }
  }
}