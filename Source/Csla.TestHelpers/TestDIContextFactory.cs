﻿//-----------------------------------------------------------------------
// <copyright file="TestDIContextFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory for DI context instances for use in tests</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using System.Security.Principal;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Csla.TestHelpers
{

  /// <summary>
  /// Factory for test DI contexts, for use in unit testing
  /// </summary>
  public static class TestDIContextFactory
  {

    /// <summary>
    /// Create a test DI context for testing with a default authenticated user
    /// </summary>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateDefaultContext(Action<IServiceCollection> configureServices = null)
    {
      ClaimsPrincipal principal;

      // Create a default security principal
      principal = CreateDefaultClaimsPrincipal();

      // Delegate to the other overload to create the context
      return CreateContext(principal, configureServices);
    }

    /// <summary>
    /// Create a test DI context for testing with a specific authenticated user
    /// </summary>
    /// <param name="principal">The principal which is to be set as the security context for Csla operations</param>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateContext(ClaimsPrincipal principal, Action<IServiceCollection> configureServices = null)
    {
      return CreateContext(null, principal, configureServices);
    }

    /// <summary>
    /// Create a test DI context for testing with a specific authenticated user
    /// </summary>
    /// <param name="customCslaOptions">The options action that is used by the consumer to configure Csla</param>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateContext(Action<CslaOptions> customCslaOptions)
    {
      ClaimsPrincipal principal;

      principal = CreateDefaultClaimsPrincipal();
      return CreateContext(customCslaOptions, principal);
    }

    /// <summary>
    /// Create a test DI context for testing with a specific authenticated user
    /// </summary>
    /// <param name="customCslaOptions">The options action that is used by the consumer to configure Csla</param>
    /// <param name="principal">The principal which is to be set as the security context for Csla operations</param>
    /// <returns>A TestDIContext that can be used to perform testing dependent upon DI</returns>
    public static TestDIContext CreateContext(Action<CslaOptions> customCslaOptions, ClaimsPrincipal principal, Action<IServiceCollection> configureServices = null)
    {
      IServiceProvider serviceProvider;
      ApplicationContext context;

      // Initialise DI
      var services = new ServiceCollection();

      // Add Csla
      services.TryAddSingleton<Server.Dashboard.IDashboard, Server.Dashboard.Dashboard>();
      services.AddSingleton<Core.IContextManager, ApplicationContextManagerUnitTests>();
      services.AddCsla(customCslaOptions);
      configureServices?.Invoke(services);

      serviceProvider = services.BuildServiceProvider();

      // Initialise CSLA security
      context = serviceProvider.GetRequiredService<ApplicationContext>();
      context.Principal = principal;

      return new TestDIContext(serviceProvider);
    }

    /// <summary>
    /// Create a default ClaimsPrincipal for use as the security context of Csla operations
    /// </summary>
    /// <returns>A default configured ClaimsPrincipal</returns>
    private static ClaimsPrincipal CreateDefaultClaimsPrincipal()
    {
      ClaimsIdentity identity;

      // Create a default security principal
      identity = new ClaimsIdentity(new GenericIdentity("Fred"));
      identity.AddClaim(new Claim(ClaimTypes.Role, "Users"));
      return new ClaimsPrincipal(identity);
    }

  }
}
