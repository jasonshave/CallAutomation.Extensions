// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Services;

internal sealed class CallAutomationCallbacks
{
    private readonly Dictionary<(string, Type), List<Delegate>> _callbackDelegates = new();
    private readonly Dictionary<(string, Type), List<(MethodInfo, Type)>> _callbackHandlers = new();

    public IOperationContext Context { get; }

    public CallAutomationCallbacks(IOperationContext context)
    {
        Context = context;
    }

    public void AddDelegateCallback<T>(Delegate callback)
    {
        if (!_callbackDelegates.ContainsKey((Context.RequestId, typeof(T))))
        {
            // new map
            _callbackDelegates.Add((Context.RequestId, typeof(T)), new List<Delegate> { callback });
            return;
        }

        // add map
        _callbackDelegates[(Context.RequestId, typeof(T))].Add(callback);
    }

    public void AddHandlerCallback<THandler, T>(string methodName, params Type[] methodParameters)
        where THandler : CallAutomationHandler
    {
        var methodInfo = typeof(THandler).GetMethod(methodName, methodParameters);
        if (methodInfo is null)
            throw new ApplicationException($"Could not find a method matching the signature for handler: {typeof(THandler).Name}. There were {methodParameters.Length} input parameters to the method signature.");

        if (!_callbackHandlers.ContainsKey((Context.RequestId, typeof(T))))
        {
            _callbackHandlers.Add((Context.RequestId, typeof(T)), new List<(MethodInfo, Type)> { (methodInfo, typeof(THandler)) });
            return;
        }

        _callbackHandlers[(Context.RequestId, typeof(T))].Add((methodInfo, typeof(THandler)));
    }

    public IEnumerable<Delegate> GetDelegateCallbacks(Type type)
    {
        _callbackDelegates.TryGetValue((Context.RequestId, type), out var callbacks);
        if (callbacks is null) return Enumerable.Empty<Delegate>();

        return callbacks;
    }

    public IEnumerable<(MethodInfo, Type)> GetHandlers(Type type)
    {
        _callbackHandlers.TryGetValue((Context.RequestId, type), out var handlerTuple);
        if (handlerTuple is null) return Enumerable.Empty<(MethodInfo, Type)>();

        return handlerTuple;
    }
}