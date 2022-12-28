// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;

namespace CallAutomation.Extensions.Services;

internal class CallAutomationCallbacks
{
    private readonly ConcurrentDictionary<(string RequestId, Type Type), List<Delegate>> _callbackDelegates = new();
    private readonly ConcurrentDictionary<(string RequestId, string EventName), List<(Type HandlerType, string MethodName)>> _callbackHandlers = new();

    public string RequestId { get; }

    public CallAutomationCallbacks(string requestId)
    {
        RequestId = requestId;
    }

    public void AddDelegateCallback<T>(string requestId, Delegate callback)
    {
        if (_callbackDelegates.ContainsKey((requestId, typeof(T))))
        {
            // add map
            _callbackDelegates[(requestId, typeof(T))].Add(callback);
            return;
        }

        // new map
        if (!_callbackDelegates.TryAdd((requestId, typeof(T)), new List<Delegate> { callback }))
        {
            throw new ApplicationException(
                $"Unable to add delegate for {typeof(T).Name} for request ID: {requestId}");
        }
    }

    public void AddHandlerCallback<THandler, T>(string requestId, string methodName)
        where THandler : CallAutomationHandler
    {
        if (_callbackHandlers.ContainsKey((requestId, typeof(T).Name)))
        {
            _callbackHandlers[(requestId, typeof(T).Name)].Add((typeof(THandler), methodName));
            return;
        }

        if (!_callbackHandlers.TryAdd((requestId, typeof(T).Name), new List<(Type HandlerType, string MethodName)> { (typeof(THandler), methodName) }))
        {
            throw new ApplicationException(
                $"Unable to add callback for {typeof(T).Name} for request ID: {requestId}");
        }
    }

    public IEnumerable<Delegate> GetDelegateCallbacks(string requestId, Type type, bool remove = default)
    {
        _callbackDelegates.TryGetValue((requestId, type), out var callbacks);
        if (callbacks is null) return Enumerable.Empty<Delegate>();
        if (remove) _callbackDelegates.TryRemove((requestId, type), out _);

        return callbacks;
    }

    public IEnumerable<(Type HandlerType, string MethodName)> GetHandlers(string requestId, Type type, bool remove = default)
    {
        _callbackHandlers.TryRemove((requestId, type.Name), out var handlerTuple);
        if (handlerTuple is null) return Enumerable.Empty<(Type HandlerType, string MethodName)>();
        if (remove) _callbackHandlers.TryRemove((requestId, type.Name), out _);

        return handlerTuple;
    }
}