// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;
using System.Collections.Concurrent;

namespace CallAutomation.Extensions.Services;

internal static class CallbackRegistry
{
    private static readonly ConcurrentDictionary<string, ICallAutomationHelperCallback> _genericCallbacks = new();

    internal static void RegisterHelperCallback(ICallAutomationHelperCallback helperCallbacks)
    {
        var added = _genericCallbacks.TryAdd(helperCallbacks.HelperCallbacks.RequestId, helperCallbacks);
        if (!added)
        {
            throw new ApplicationException(
                $"Unable to add callback for request ID: {helperCallbacks.HelperCallbacks.RequestId}");
        }
    }

    internal static ICallAutomationHelperCallback? GetHelperCallback(string requestId, Type type, bool remove = default)
    {
        var found = _genericCallbacks.TryGetValue(requestId, out var callback);
        if (found && remove)
        {
            _genericCallbacks.TryRemove(requestId, out _);
            return callback;
        }

        return null;
    }
}