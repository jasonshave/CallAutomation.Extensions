// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Reflection;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Services;

internal class CallAutomationCallbacks : ICallbacksHandler
{
    private readonly ConcurrentDictionary<(string RequestId, string EventName), List<Delegate>> _callbackDelegates = new();
    private readonly ConcurrentDictionary<(string RequestId, string EventName), List<(string HandlerName, string MethodName)>> _callbackHandlers = new();

    public void AddDelegateCallback<T>(string requestId, Delegate callback)
    {
        if (_callbackDelegates.ContainsKey((requestId, typeof(T).Name)))
        {
            // add map
            _callbackDelegates[(requestId, typeof(T).Name)].Add(callback);
            return;
        }

        // new map
        if(!_callbackDelegates.TryAdd((requestId, typeof(T).Name), new List<Delegate> { callback }))
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
            _callbackHandlers[(requestId, typeof(T).Name)].Add((typeof(THandler).FullName, methodName));
            return;
        }

        if (!_callbackHandlers.TryAdd((requestId, typeof(T).Name), new List<(string HandlerName, string MethodName)> { (typeof(THandler).FullName, methodName) }))
        {
            throw new ApplicationException(
                $"Unable to add callback for {typeof(T).Name} for request ID: {requestId}");
        }
    }

    public IEnumerable<Delegate> GetDelegateCallbacks(string requestId, Type type)
    {
        _callbackDelegates.TryGetValue((requestId, type.Name), out var callbacks);
        if (callbacks is null) return Enumerable.Empty<Delegate>();

        return callbacks;
    }

    public IEnumerable<(string HandlerName, string MethodName)> GetHandlers(string requestId, Type type)
    {
        _callbackHandlers.TryRemove((requestId, type.Name), out var handlerTuple);
        return handlerTuple ?? Enumerable.Empty<(string HandlerName, string MethodName)>();
    }
}