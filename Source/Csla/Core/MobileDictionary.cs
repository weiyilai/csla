﻿//-----------------------------------------------------------------------
// <copyright file="MobileDictionary.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a dictionary that can be serialized through</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Defines a dictionary that can be serialized through
  /// the SerializationFormatterFactory.GetFormatter().
  /// </summary>
  /// <typeparam name="K">Key value: any primitive or IMobileObject type.</typeparam>
  /// <typeparam name="V">Value: any primitive or IMobileObject type.</typeparam>
  [Serializable]
  public class MobileDictionary<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] K, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] V> : Dictionary<K, V>, IMobileObject where K: notnull
  {
    private bool _keyIsMobile;
    private bool _valueIsMobile;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public MobileDictionary()
    {
      DetermineTypes();
    }

    /// <summary>
    /// Creates an instance of the object based
    /// on the supplied dictionary, whose elements
    /// are copied to the new dictionary.
    /// </summary>
    /// <param name="capacity">The initial number of elements 
    /// the dictionary can contain.</param>
    public MobileDictionary(int capacity)
      : base(capacity)
    {
      DetermineTypes();
    }

    /// <summary>
    /// Creates an instance of the object based
    /// on the supplied dictionary, whose elements
    /// are copied to the new dictionary.
    /// </summary>
    /// <param name="comparer">The comparer to use when
    /// comparing keys.</param>
    public MobileDictionary(IEqualityComparer<K> comparer)
      : base(comparer)
    {
      DetermineTypes();
    }

    /// <summary>
    /// Creates an instance of the object based
    /// on the supplied dictionary, whose elements
    /// are copied to the new dictionary.
    /// </summary>
    /// <param name="dict">Source dictionary.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dict"/> is <see langword="null"/>.</exception>
    public MobileDictionary(Dictionary<K, V> dict)
      : base(dict)
    {
      DetermineTypes();
    }

    /// <summary>
    /// Gets a value indicating whether the
    /// dictionary contains the specified key
    /// value.
    /// </summary>
    /// <param name="key">Key value</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    public bool Contains(K key)
    {
      if (key is null)
        throw new ArgumentNullException(nameof(key));

      return ContainsKey(key);
    }

    private void DetermineTypes()
    {
      _keyIsMobile = typeof(IMobileObject).IsAssignableFrom(typeof(K));
      _valueIsMobile = typeof(IMobileObject).IsAssignableFrom(typeof(V));
    }

    #region IMobileObject Members

    private const string _keyPrefix = "k";
    private const string _valuePrefix = "v";

    void IMobileObject.GetState(SerializationInfo info)
    {
      info.AddValue("count", Keys.Count);
      GetState(info);
    }

    /// <summary>
    /// Add property values to the serialization stream.
    /// </summary>
    /// <param name="info"></param>
    protected virtual void GetState(SerializationInfo info)
    { }

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      int count = 0;
      foreach (var (key, value) in this)
      {
        if (_keyIsMobile)
        {
          SerializationInfo si = formatter.SerializeObject(key);
          info.AddChild(_keyPrefix + count, si.ReferenceId);
        }
        else
        {
          info.AddValue(_keyPrefix + count, key);
        }

        if (_valueIsMobile)
        {
          SerializationInfo si = formatter.SerializeObject(this[key]);
          info.AddChild(_valuePrefix + count, si.ReferenceId);
        }
        else
        {
          info.AddValue(_valuePrefix + count, value);
        }
        count++;
      }
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      SetState(info);
    }

    /// <summary>
    /// Set property values from serialization stream.
    /// </summary>
    /// <param name="info"></param>
    protected virtual void SetState(SerializationInfo info)
    { }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      int count = info.GetValue<int>("count");

      for (int index = 0; index < count; index++)
      {
        K key;
        if (_keyIsMobile)
          key = (K)(formatter.GetObject(info.Children[_keyPrefix + index].ReferenceId) ?? throw new InvalidOperationException());
        else
          key = info.GetValue<K>(_keyPrefix + index)!;

        V value;
        if (_valueIsMobile)
          value = (V)formatter.GetObject(info.Children[_valuePrefix + index].ReferenceId)!;
        else
          value = info.GetValue<V>(_valuePrefix + index)!;

        Add(key, value);
      }
    }

    #endregion
  }
}