﻿//-----------------------------------------------------------------------
// <copyright file="WindowsConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------

using Csla.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Implement extension methods for Windows Forms
  /// </summary>
  public static class WindowsConfigurationExtensions
  {
    /// <summary>
    /// Registers services necessary for Windows Forms
    /// environments.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions AddWindowsForms(this CslaOptions config)
    {
      return AddWindowsForms(config, null);
    }

    /// <summary>
    /// Registers services necessary for Windows Forms
    /// environments.
    /// </summary>
    /// <param name="config">CslaConfiguration object</param>
    /// <param name="options">XamlOptions action</param>
    /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
    public static CslaOptions AddWindowsForms(this CslaOptions config, Action<WindowsFormsOptions>? options)
    {
      var winFormsOptions = new WindowsFormsOptions();
      options?.Invoke(winFormsOptions);

      // use correct IContextManager
      config.Services.AddSingleton<Core.IContextManager, ApplicationContextManager>();

      // use correct mode for raising PropertyChanged events
      config.BindingOptions.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows;
      return config;
    }
  }
}
