// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;
using System.Collections.Concurrent;

namespace CallAutomation.Extensions.Services;

internal static class CallbackRegistry
{
    private static readonly ConcurrentDictionary<(string, Type), ICallAutomationHelperCallback> _genericCallbacks = new();

    internal static void RegisterHelperCallback(ICallAutomationHelperCallback helperCallbacks, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var added = _genericCallbacks.TryAdd((helperCallbacks.HelperCallbacks.Context, type), helperCallbacks);
            if (!added)
            {
                throw new ApplicationException(
                    $"Unable to add callback for {type.Name} for request ID: {helperCallbacks.HelperCallbacks.Context}");
            }
        }
    }

    internal static ICallAutomationHelperCallback? GetHelperCallback(string requestId, Type type, bool remove = default)
    {
        var found = _genericCallbacks.TryGetValue((requestId, type), out var callback);
        if (found && remove)
        {
            _genericCallbacks.TryRemove((requestId, type), out _);
            return callback;
        }

        return null;
    }
}