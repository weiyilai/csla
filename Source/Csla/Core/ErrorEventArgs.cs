﻿//-----------------------------------------------------------------------
// <copyright file="ErrorEventArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Event arguments for an unhandled async</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Event arguments for an unhandled async
  /// exception.
  /// </summary>
  public class ErrorEventArgs : EventArgs
  {
    /// <summary>
    /// Reference to the original sender of the event.
    /// </summary>
    public object OriginalSender { get; protected set; }
    /// <summary>
    /// Reference to the unhandled async exception object.
    /// </summary>
    public Exception Error { get; protected set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="originalSender">
    /// Reference to the original sender of the event.
    /// </param>
    /// <param name="error">
    /// Reference to the unhandled async exception object.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="originalSender"/> or <paramref name="error"/> is <see langword="null"/>.</exception>
    public ErrorEventArgs(object originalSender, Exception error)
    {
      OriginalSender = originalSender ?? throw new ArgumentNullException(nameof(originalSender));
      Error = error ?? throw new ArgumentNullException(nameof(error));
    }
  }
}