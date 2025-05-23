//-----------------------------------------------------------------------
// <copyright file="CommandBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which command </summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using Csla.Core;
using Csla.Properties;
using Csla.Reflection;
using Csla.Server;

namespace Csla
{
  /// <summary>
  /// This is the base class from which command 
  /// objects will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Command objects allow the execution of arbitrary server-side
  /// functionality. Most often, this involves the invocation of
  /// a stored procedure in the database, but can involve any other
  /// type of stateless, atomic call to the server instead.
  /// </para><para>
  /// To implement a command object, inherit from CommandBase and
  /// override the DataPortal_Execute method. In this method you can
  /// implement any server-side code as required.
  /// </para><para>
  /// To pass data to/from the server, use instance variables within
  /// the command object itself. The command object is instantiated on
  /// the client, and is passed by value to the server where the 
  /// DataPortal_Execute method is invoked. The command object is then
  /// returned to the client by value.
  /// </para>
  /// </remarks>
  [Serializable]
  public abstract class CommandBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]T> : ManagedObjectBase,
      IDataPortalTarget,
      IManageProperties,
      ICommandBase
    where T : CommandBase<T>
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public CommandBase()
    {
      Initialize();
    }

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region Identity

    int IBusinessObject.Identity => 0;

    #endregion

    #region Data Access

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [Delete]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during server-side processing.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during processing.</param>
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.CheckRules()
    { }

    Task IDataPortalTarget.CheckRulesAsync() => Task.CompletedTask;

    Task IDataPortalTarget.WaitForIdle(TimeSpan timeout) => Task.CompletedTask;
    Task IDataPortalTarget.WaitForIdle(CancellationToken ct) => Task.CompletedTask;

    void IDataPortalTarget.MarkAsChild()
    { }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return ObjectCloner.GetInstance(ApplicationContext).Clone(this)!;
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    public T Clone()
    {
      return (T)GetClone();
    }

    #endregion

    #region  Register Properties

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">
    /// Type of property.
    /// </typeparam>
    /// <param name="info">
    /// PropertyInfo object for the property.
    /// </param>
    /// <returns>
    /// The provided IPropertyInfo object.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> info)
    {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      return Core.FieldManager.PropertyInfoManager.RegisterProperty<P>(typeof(T), info);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string? propertyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string? propertyName, RelationshipTypes relationship)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, string.Empty, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, RelationshipTypes relationship)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, relationship);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string? propertyName, string? friendlyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P defaultValue)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName, P defaultValue)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName, defaultValue);
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyName">Property name from nameof()</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(string propertyName, string? friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));

      return RegisterProperty(Core.FieldManager.PropertyInfoFactory.Factory.Create<P>(typeof(T), propertyName, friendlyName, defaultValue, relationship));
    }

    /// <summary>
    /// Indicates that the specified property belongs
    /// to the business object type.
    /// </summary>
    /// <typeparam name="P">Type of property</typeparam>
    /// <param name="propertyLambdaExpression">Property Expression</param>
    /// <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    /// <param name="defaultValue">Default Value for the property</param>
    /// <param name="relationship">Relationship with property value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyLambdaExpression"/> is <see langword="null"/>.</exception>
    protected static PropertyInfo<P> RegisterProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(Expression<Func<T, object>> propertyLambdaExpression, string? friendlyName, P defaultValue, RelationshipTypes relationship)
    {
      if (propertyLambdaExpression is null)
        throw new ArgumentNullException(nameof(propertyLambdaExpression));

      PropertyInfo reflectedPropertyInfo = Reflect<T>.GetProperty(propertyLambdaExpression);
      return RegisterProperty<P>(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship);
    }

    #endregion

    #region IManageProperties Members

    bool IManageProperties.HasManagedProperties => FieldManager.HasFields;

    List<IPropertyInfo> IManageProperties.GetManagedProperties()
    {
      return FieldManager.GetRegisteredProperties();
    }

    object? IManageProperties.GetProperty(IPropertyInfo propertyInfo)
    {
      return ReadProperty(propertyInfo);
    }

    object? IManageProperties.ReadProperty(IPropertyInfo propertyInfo)
    {
      return ReadProperty(propertyInfo);
    }

    P? IManageProperties.ReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo) where P: default
    {
      return ReadProperty<P>(propertyInfo);
    }

    void IManageProperties.SetProperty(IPropertyInfo propertyInfo, object? newValue)
    {
      FieldManager.SetFieldData(propertyInfo, newValue);
    }

    void IManageProperties.LoadProperty(IPropertyInfo propertyInfo, object? newValue)
    {
      LoadProperty(propertyInfo, newValue);
    }

    void IManageProperties.LoadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, P? newValue) where P: default
    {
      LoadProperty<P>(propertyInfo, newValue);
    }

    bool IManageProperties.LoadPropertyMarkDirty(IPropertyInfo propertyInfo, object? newValue)
    {
      LoadProperty(propertyInfo, newValue);
      return false;
    }

    List<object> IManageProperties.GetChildren()
    {
      return FieldManager.GetChildren();
    }

    bool IManageProperties.FieldExists(IPropertyInfo property)
    {
      return FieldManager.FieldExists(property);
    }

    object IManageProperties.LazyGetProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      throw new NotImplementedException();
    }

    object IManageProperties.LazyGetPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      throw new NotImplementedException();
    }

    P IManageProperties.LazyReadProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Func<P> valueGenerator)
    {
      throw new NotImplementedException();
    }

    P IManageProperties.LazyReadPropertyAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] P>(PropertyInfo<P> propertyInfo, Task<P> factory)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}