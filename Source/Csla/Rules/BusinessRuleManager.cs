﻿//-----------------------------------------------------------------------
// <copyright file="BusinessRuleManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Manages the list of rules for a business type.</summary>
//-----------------------------------------------------------------------

#if NET8_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif

namespace Csla.Rules
{
  /// <summary>
  /// Manages the list of rules for a business type.
  /// </summary>
  public class BusinessRuleManager
  {
#if NET8_0_OR_GREATER
    private static readonly Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, Tuple<string?, BusinessRuleManager>>> _perTypeRules = new();
#else
    private static readonly Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, BusinessRuleManager>> _perTypeRules =
      new Lazy<System.Collections.Concurrent.ConcurrentDictionary<RuleSetKey, BusinessRuleManager>>();
#endif

    internal static BusinessRuleManager GetRulesForType(Type type, string? ruleSet)
    {
      if (ruleSet == ApplicationContext.DefaultRuleSet)
        ruleSet = null;

      var key = new RuleSetKey(type, ruleSet);

#if NET8_0_OR_GREATER
      var rulesInfo = _perTypeRules.Value
        .GetOrAdd(
          key,
          _ => AssemblyLoadContextManager.CreateCacheInstance(type, new BusinessRuleManager(), OnAssemblyLoadContextUnload)
        );

      return rulesInfo.Item2;
#else
      return _perTypeRules.Value.GetOrAdd(key, _ => { return new BusinessRuleManager(); });
#endif
    }

    /// <summary>
    /// Remove/delete all the rules for the given type.
    /// </summary>
    /// <param name="type">The type.</param>
    internal static void CleanupRulesForType(Type type)
    {
      lock (_perTypeRules)
      {
        // the first RuleSet is already added to list when this check is executed so so if count > 1 then we have already initialized type rules.
        var typeRules = _perTypeRules.Value.Where(value => value.Key.Type == type);
        foreach (var key in typeRules)
          _perTypeRules.Value.TryRemove(key.Key, out _);
      }
    }

    internal static BusinessRuleManager GetRulesForType(Type type)
    {
      return GetRulesForType(type, null);
    }

    private class RuleSetKey
    {
      public Type Type { get; }
      public string? RuleSet { get; }

      public RuleSetKey(Type type, string? ruleSet)
      {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        RuleSet = ruleSet;
      }

      public override bool Equals(object? obj)
      {
        if (!(obj is RuleSetKey other))
          return false;
        else
          return Type.Equals(other.Type) && RuleSet == other.RuleSet;
      }

      public override int GetHashCode()
      {
        return (Type.FullName + RuleSet).GetHashCode();
      }
    }

    /// <summary>
    /// Gets the list of rule objects for the business type.
    /// </summary>
    public List<IBusinessRuleBase> Rules { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the rules have been
    /// initialized.
    /// </summary>
    public bool Initialized { get; set; }

    private BusinessRuleManager()
    {
      Rules = [];
    }

#if NET8_0_OR_GREATER
    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      lock (_perTypeRules)
        AssemblyLoadContextManager.RemoveFromCache((IDictionary<RuleSetKey, Tuple<string?, BusinessRuleManager>?>)_perTypeRules.Value, context, true);
    }
#endif
  }
}
