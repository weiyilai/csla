#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
//-----------------------------------------------------------------------
// <copyright file="EnterpriseServicesSettings.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.EnterpriseServices;

// EnterpriseServices settings
[assembly: ApplicationActivation(ActivationOption.Library)]
[assembly: ApplicationName("CSLA .NET DataPortal")]
[assembly: Description("CSLA .NET Serviced DataPortal")]
[assembly: ApplicationAccessControl(false)]

#endif