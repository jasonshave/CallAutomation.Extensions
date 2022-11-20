// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;
using System.Reflection;
using System.Text.Json;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackBase : ICallAutomationHelperCallback
{
    private readonly Dictionary<(string, Type), List<Delegate>> _callbackDelegates = new();
    private readonly Dictionary<(string, Type), List<(MethodInfo, Type)>> _callbackHandlers = new();

    public IEnumerable<Type> Types { get; }

    public IOperationContext? OperationContext { get; protected set; }

    protected HelperCallbackBase(IEnumerable<Type> types, IOperationContext? context = null)
    {
        Types = types.ToList();
        OperationContext = context;
        CallbackRegistry.RegisterHelperCallback(this, Types);
    }

    public void AddDelegateCallback<T>(Delegate callback)
    {
        if (!_callbackDelegates.ContainsKey((OperationContext.RequestId, typeof(T))))
        {
            // new map
            _callbackDelegates.Add((OperationContext.RequestId, typeof(T)), new List<Delegate> { callback });
            return;
        }

        // add map
        _callbackDelegates[(OperationContext.RequestId, typeof(T))].Add(callback);
    }

    public void AddHandlerCallback<THandler, T>(string methodName, params Type[] methodParameters)
        where THandler : CallAutomationHandler
    {
        var methodInfo = typeof(THandler).GetMethod(methodName, methodParameters);
        if (methodInfo is null)
            throw new ApplicationException($"Could not find a method matching the signature for handler: {typeof(THandler).Name}. There were {methodParameters.Length} input parameters to the method signature.");

        if (!_callbackHandlers.ContainsKey((OperationContext.RequestId, typeof(T))))
        {
            _callbackHandlers.Add((OperationContext.RequestId, typeof(T)), new List<(MethodInfo, Type)> { (methodInfo, typeof(THandler)) });
            return;
        }

        _callbackHandlers[(OperationContext.RequestId, typeof(T))].Add((methodInfo, typeof(THandler)));
    }

    public IEnumerable<Delegate> GetDelegateCallbacks(Type type)
    {
        _callbackDelegates.TryGetValue((OperationContext.RequestId, type), out var callbacks);
        if (callbacks is null) return Enumerable.Empty<Delegate>();

        return callbacks;
    }

    public IEnumerable<(MethodInfo, Type)> GetHandlers(Type type)
    {
        _callbackHandlers.TryGetValue((OperationContext.RequestId, type), out var handlerTuple);
        if (handlerTuple is null) return Enumerable.Empty<(MethodInfo, Type)>();

        return handlerTuple;
    }

    protected string? GetSerializedContext()
    {
        return OperationContext is null
            ? null
            : JsonSerializer.Serialize(OperationContext);
    }
}