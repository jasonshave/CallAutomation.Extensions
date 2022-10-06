// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Reflection;
using Azure.Communication.CallAutomation;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions;

internal static class CallbackRegistry
{
    private static readonly ConcurrentDictionary<(string, Type), Delegate> _callbackDelegates = new();
    private static readonly ConcurrentDictionary<(string, Type), Func<ValueTask>> _asyncCallbackDelegates = new();
    private static readonly ConcurrentDictionary<(string, Type), (Type?, MethodInfo?)> _callbackHandlers = new();

    internal static void Register<TEvent>(string callbackId, Func<TEvent, CallConnection, CallMedia, CallRecording, ValueTask> request)
        where TEvent : CallAutomationEventBase
    {
        var added = _callbackDelegates.TryAdd((callbackId, typeof(TEvent)), request);
        if (!added)
        {
            throw new ApplicationException($"Unable to add delegate for callback ID:{callbackId}");
        }
    }

    internal static void Register<TEvent>(string callbackId, Func<ValueTask> request)
        where TEvent : CallAutomationEventBase
    {
        var added = _asyncCallbackDelegates.TryAdd((callbackId, typeof(TEvent)), request);
        if (!added)
        {
            throw new ApplicationException($"Unable to add delegate for callback ID:{callbackId}");
        }
    }

    internal static void Register<THandler, TEvent>(string callbackId)
        where THandler : CallAutomationHandler
    {
        var methodInfo = typeof(THandler).GetMethod($"On{typeof(TEvent).Name}", new[] { typeof(TEvent), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording) });
        if (methodInfo is null)
        {
            throw new ApplicationException(
                "Unable to register handler. " +
                $"Check the overload to make sure the method signature matches one of the available methods in {nameof(CallAutomationHandler)}");
        }

        var added = _callbackHandlers.TryAdd((callbackId, typeof(TEvent)), (typeof(THandler), methodInfo));
        if (!added)
        {
            throw new ApplicationException(
                $"Unable to add callback handler {typeof(THandler).Name} with callback ID {callbackId}");
        }
    }

    internal static Delegate? GetCallback(string callbackId, Type eventType, bool remove = default)
    {
        var found = _callbackDelegates.TryGetValue((callbackId, eventType), out Delegate? callback);
        if (found && remove)
        {
            TryRemoveCallback(callbackId, eventType);
            return callback;
        }

        // look for simple callback
        found = _asyncCallbackDelegates.TryGetValue((callbackId, eventType), out Func<ValueTask>? asyncCallback);
        if (found && remove) TryRemoveAsyncCallback(callbackId, eventType);
        return asyncCallback;
    }

    internal static (Type?, MethodInfo?) GetCallbackHandlerMethod(string callbackId, Type eventType, bool remove = default)
    {
        var found = _callbackHandlers.TryGetValue((callbackId, eventType), out (Type?, MethodInfo?) handlerTuple);
        if (found && remove) TryRemoveHandler(callbackId, eventType);
        return handlerTuple;
    }

    private static bool TryRemoveHandler(string callbackId, Type eventType) => _callbackHandlers.TryRemove((callbackId, eventType), out _);

    private static bool TryRemoveCallback(string callbackId, Type eventType) => _callbackDelegates.TryRemove((callbackId, eventType), out _);

    private static bool TryRemoveAsyncCallback(string callbackId, Type eventType) => _asyncCallbackDelegates.TryRemove((callbackId, eventType), out _);
}