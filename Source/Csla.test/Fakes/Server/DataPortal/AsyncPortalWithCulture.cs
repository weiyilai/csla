﻿//-----------------------------------------------------------------------
// <copyright file="AsyncPortalWithCulture.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Testing.Business.DataPortal
{
  [Serializable]
  public class AsyncPortalWithCulture : CommandBase<AsyncPortalWithCulture>
  {
    public string CurrentUICulture
    {
      get
      {
        return ReadProperty(CurrentUICultureProperty);
      }
      set
      {
        LoadProperty(CurrentUICultureProperty, value);
      }
    }
    public static PropertyInfo<string> CurrentUICultureProperty = 
      RegisterProperty<string>(new PropertyInfo<string>("CurrentUICulture"));

    public string CurrentCulture
    {
      get
      {
        return ReadProperty(CurrentCultureProperty);
      }
      set
      {
        LoadProperty(CurrentCultureProperty, value);
      }
    }
    public static PropertyInfo<string> CurrentCultureProperty =
      RegisterProperty<string>(new PropertyInfo<string>("CurrentCulture"));

    [RunLocal]
    [Create]
    private void Create()
    { }

    [Execute]
    protected void DataPortal_Execute()
    {
      CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
      CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Name;
    }
  }
}