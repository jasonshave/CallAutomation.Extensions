// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;

namespace CallAutomation.Extensions;

internal static class CallbackRegistry
{
    private static readonly ConcurrentDictionary<(string, Type), ICallAutomationCallback<DtmfTone>> _recognizeDtmfCallbacks = new ();
    private static readonly ConcurrentDictionary<(string, Type), ICallAutomationCallback<Type>> _recognizeCallbacks = new ();

    private static readonly ConcurrentDictionary<(string, Type), Delegate> _callbackDelegates = new ();
    private static readonly ConcurrentDictionary<(string, Type), Func<ValueTask>> _asyncCallbackDelegates = new ();
    private static readonly ConcurrentDictionary<(string, Type), (Type?, MethodInfo?)> _callbackHandlers = new ();

    internal static void Register<T>(ICallAutomationCallback<T> callAutomationCallback, Type eventType)
    {
        if (typeof(T) == typeof(DtmfTone))
        {
            var added = _recognizeDtmfCallbacks.TryAdd((callAutomationCallback.RequestId, eventType), (ICallAutomationCallback<DtmfTone>)callAutomationCallback);
            if (!added)
            {
                throw new ApplicationException(
                    $"Unable to add DTMF callback for request ID: {callAutomationCallback.RequestId}");
            }
        }

        if (typeof(T) == typeof(Type))
        {
            var added = _recognizeCallbacks.TryAdd((callAutomationCallback.RequestId, eventType), (ICallAutomationCallback<Type>)callAutomationCallback);
            if (!added)
            {
                throw new ApplicationException(
                    $"Unable to add DTMF callback for request ID: {callAutomationCallback.RequestId}");
            }
        }
    }

    internal static void Register<TEvent>(string callbackId, Func<TEvent, CallConnection, CallMedia, CallRecording, ValueTask> request)
        where TEvent : CallAutomationEventBase
    {
        var added = _callbackDelegates.TryAdd((callbackId, typeof(TEvent)), request);
        if (!added)
        {
            throw new ApplicationException($"Unable to add delegate for request ID:{callbackId}");
        }
    }

    internal static void Register<TEvent>(string callbackId, Func<ValueTask> request)
        where TEvent : CallAutomationEventBase
    {
        var added = _asyncCallbackDelegates.TryAdd((callbackId, typeof(TEvent)), request);
        if (!added)
        {
            throw new ApplicationException($"Unable to add delegate for request ID:{callbackId}");
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
                $"Unable to add callback handler {typeof(THandler).Name} with request ID {callbackId}");
        }
    }

    internal static ICallAutomationCallback<T>? GetDtmfCallback<T>(string requestId, Type eventType, bool remove = default)
    {
        if (typeof(T) == typeof(DtmfTone))
        {
            var found = _recognizeDtmfCallbacks.TryGetValue((requestId, eventType), out ICallAutomationCallback<DtmfTone>? callback);
            if (found && remove)
            {
                _recognizeDtmfCallbacks.TryRemove((requestId, eventType), out _);
                return (ICallAutomationCallback<T>?)callback;
            }
        }

        if (typeof(T) == typeof(Type))
        {
            var found = _recognizeCallbacks.TryGetValue((requestId, eventType), out ICallAutomationCallback<Type>? callback);
            if (found && remove)
            {
                _recognizeCallbacks.TryRemove((requestId, eventType), out _);
                return (ICallAutomationCallback<T>?)callback;
            }
        }

        return null;
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

    internal static (Type?, MethodInfo?) GetCallbackHandler(string callbackId, Type eventType, bool remove = default)
    {
        var found = _callbackHandlers.TryGetValue((callbackId, eventType), out (Type?, MethodInfo?) handlerTuple);
        if (found && remove) TryRemoveHandler(callbackId, eventType);
        return handlerTuple;
    }

    private static bool TryRemoveHandler(string callbackId, Type eventType) => _callbackHandlers.TryRemove((callbackId, eventType), out _);

    private static bool TryRemoveCallback(string callbackId, Type eventType) => _callbackDelegates.TryRemove((callbackId, eventType), out _);

    private static bool TryRemoveAsyncCallback(string callbackId, Type eventType) => _asyncCallbackDelegates.TryRemove((callbackId, eventType), out _);
}