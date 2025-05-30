﻿//-----------------------------------------------------------------------
// <copyright file="INotifyBusy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Interface defining an object that notifies when it</summary>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Csla.Core
{
  /// <summary>
  /// Helper class for busy related functionality spread across different business type implementations.
  /// </summary>
  public static class BusyHelper 
  {
    internal static async Task WaitForIdleAsTimeout(Func<CancellationToken, Task> operation, Type source, string methodName, TimeSpan timeout)
    {
      try
      {
        using var cts = timeout.ToCancellationTokenSource();
        await operation(cts.Token);
      }
      catch (TaskCanceledException tcex)
      {
        throw new TimeoutException($"{source.GetType().FullName}.{methodName} - {timeout}.", tcex);
      }
    }

    /// <summary>
    /// Waits for the specified <see cref="INotifyBusy"/> object to become idle within the specified timeout.
    /// </summary>
    /// <param name="source">The <see cref="INotifyBusy"/> object to wait for.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <param name="methodName">The name of the calling method.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="TimeoutException">Thrown when the specified timeout is exceeded.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static async Task WaitForIdle(INotifyBusy source, TimeSpan timeout, [CallerMemberName] string methodName = "")
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      try
      {
        using var cts = timeout.ToCancellationTokenSource();
        await WaitForIdle(source, cts.Token, methodName);
      }
      catch (TaskCanceledException tcex)
      {
        throw new TimeoutException($"{source.GetType().FullName}.{methodName} - {timeout}.", tcex);
      }
    }

    /// <summary>
    /// Waits for the specified <see cref="INotifyBusy"/> object to become idle.
    /// </summary>
    /// <param name="source">The <see cref="INotifyBusy"/> object to wait for.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <param name="methodName">The name of the calling method.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
    public static async Task WaitForIdle(INotifyBusy source, CancellationToken ct, [CallerMemberName] string methodName = "")
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (!source.IsBusy)
      {
        return;
      }

      var tcs = new TaskCompletionSource<object?>();
      try
      {
        source.BusyChanged += ObserverForIsBusyChange;

        if (!source.IsBusy)
        {
          return;
        }
#if NET8_0_OR_GREATER
        var finishedTask = await tcs.Task.WaitAsync(ct).ConfigureAwait(false);
#else
        ct.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
        var finishedTask = await tcs.Task.ConfigureAwait(false);
#endif
      }
      finally
      {
        source.BusyChanged -= ObserverForIsBusyChange;
      }

      void ObserverForIsBusyChange(object sender, BusyChangedEventArgs e)
      {
        if (!source.IsBusy && !e.Busy)
        {
          tcs.TrySetResult(null);
        }
      }
    }
  }
}