// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;

namespace CallAutomation.Extensions.Models;

internal sealed class CallAutomationCallbacks
{
    private readonly Dictionary<(string, Type), List<Delegate>> _callbackDelegates = new ();
    private readonly Dictionary<(string, Type), List<(MethodInfo, Type)>> _callbackHandlers = new ();

    public string RequestId { get; }

    public CallAutomationCallbacks(string requestId)
    {
        RequestId = requestId;
    }

    public void AddDelegateCallback<T>(Delegate callback)
    {
        if (!_callbackDelegates.ContainsKey((RequestId, typeof(T))))
        {
            // new map
            _callbackDelegates.Add((RequestId, typeof(T)), new List<Delegate> { callback });
            return;
        }

        // add map
        _callbackDelegates[(RequestId, typeof(T))].Add(callback);
    }

    public void AddHandlerCallback<T, THandler>(string methodName, params Type[] methodParameters)
    {
        var methodInfo = typeof(THandler).GetMethod(methodName, methodParameters);
        if (methodInfo is null)
            throw new ApplicationException($"Could not find a method matching the signature for handler: {typeof(THandler)}. There were {methodParameters.Length} input parameters to the method signature.");

        if (!_callbackHandlers.ContainsKey((RequestId, typeof(T))))
        {
            _callbackHandlers.Add((RequestId, typeof(T)), new List<(MethodInfo, Type)> { (methodInfo, typeof(THandler)) });
            return;
        }

        _callbackHandlers[(RequestId, typeof(T))].Add((methodInfo, typeof(THandler)));
    }

    public List<Delegate> GetCallbacks(Type type) => _callbackDelegates[(RequestId, type)];

    public List<(MethodInfo, Type)> GetHandlers(Type type) => _callbackHandlers[(RequestId, type)];
}