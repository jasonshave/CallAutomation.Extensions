// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Reflection;
using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Services;

internal static class CallAutomationCallbacks
{
    //private readonly ConcurrentDictionary<(string RequestId, Type EventName), List<Delegate>> _callbackDelegates = new();
    private static readonly ConcurrentDictionary<(string RequestId, string EventName), List<(string HandlerName, string MethodName)>> _callbackHandlers = new();

    //public void AddDelegateCallback<T>(string requestId, Delegate callback)
    //    where T : CallAutomationEventBase
    //{
    //    if (_callbackDelegates.ContainsKey((requestId, typeof(T))))
    //    {

    //        // add map
    //        _callbackDelegates[(requestId, typeof(T))].Add(callback);
    //        return;
    //    }

    //    // new map
    //    if (!_callbackDelegates.TryAdd((requestId, typeof(T)), new List<Delegate> { callback }))
    //    {
    //        throw new ApplicationException(
    //            $"Unable to add delegate for {typeof(T).Name} for request ID: {requestId}");
    //    }
    //}

    public static void AddHandlerCallback<THandler, T>(string requestId, string methodName)
        where THandler : CallAutomationHandler
        where T : CallAutomationEventBase
    {
        //var methodInfo = typeof(THandler).GetMethod(methodName, methodParameters);
        //if (methodInfo is null)
        //    throw new ApplicationException($"Could not find a method matching the signature for handler: {typeof(THandler).Name}. There were {methodParameters.Length} input parameters to the method signature.");

        if (_callbackHandlers.ContainsKey((requestId, typeof(T).Name)))
        {
            _callbackHandlers[(requestId, typeof(T).Name)].Add((methodName, typeof(THandler).Name));
            return;
        }

        if (!_callbackHandlers.TryAdd((requestId, typeof(T).Name), new List<(string HandlerName, string MethodName)> { (typeof(THandler).FullName, methodName) }))
        {
            throw new ApplicationException(
                $"Unable to add callback for {typeof(T).Name} for request ID: {requestId}");
        }
    }


    //public IEnumerable<Delegate> GetDelegateCallbacks(string requestId, Type type)
    //{
    //    _callbackDelegates.TryGetValue((requestId, type), out var callbacks);
    //    return callbacks ?? Enumerable.Empty<Delegate>();
    //}

    public static IEnumerable<(string HandlerName, string MethodName)> GetHandlers(string requestId, Type type)
    {
        _callbackHandlers.TryGetValue((requestId, type.Name), out var handlerTuple);
        return handlerTuple ?? Enumerable.Empty<(string HandlerName, string MethodName)>();
    }
}