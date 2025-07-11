﻿//-----------------------------------------------------------------------
// <copyright file="XamlConfigurationExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods for .NET Core configuration</summary>
//-----------------------------------------------------------------------
using Csla.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Csla.Configuration;

/// <summary>
/// Implement extension methods for Xaml
/// </summary>
public static class XamlConfigurationExtensions
{
  /// <summary>
  /// Registers services necessary for Xaml-based
  /// environments.
  /// </summary>
  /// <param name="config">CslaConfiguration object</param>
  /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
  public static CslaOptions AddXaml(this CslaOptions config)
  {
    return AddXaml(config, null);
  }

  /// <summary>
  /// Registers services necessary for Xaml-based
  /// environments.
  /// </summary>
  /// <param name="config">CslaConfiguration object</param>
  /// <param name="options">XamlOptions action</param>
  /// <exception cref="ArgumentNullException"><paramref name="config"/> is <see langword="null"/>.</exception>
  public static CslaOptions AddXaml(this CslaOptions config, Action<XamlOptions>? options)
  {
    if (config is null)
      throw new ArgumentNullException(nameof(config));

    var xamlOptions = new XamlOptions();
    options?.Invoke(xamlOptions);

    // use correct IContextManager
    config.Services.AddSingleton<Core.IContextManager, ApplicationContextManager>();

    // use correct mode for raising PropertyChanged events
    config.BindingOptions.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Xaml;

#if !MAUI
    config.Services.AddTransient(typeof(ViewModel<>), typeof(ViewModel<>));
#endif

    return config;
  }

#if !MAUI
  /// <summary>
  /// Initializes CSLA for use by Xaml apps.
  /// </summary>
  /// <param name="host"></param>
  /// <exception cref="ArgumentNullException"><paramref name="host"/> is <see langword="null"/>.</exception>
  public static IHost UseCsla(this IHost host)
  {
    if (host is null)
      throw new ArgumentNullException(nameof(host));

    // create instance of ApplicationContext so the
    // Csla.Xaml.ApplicationContextManager gets a static
    // reference for use by UI helpers.
#pragma warning disable IDE0059 // Unnecessary assignment of a value
    var context = host.Services.GetService(typeof(ApplicationContext));
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    return host;
  }
#endif
}
